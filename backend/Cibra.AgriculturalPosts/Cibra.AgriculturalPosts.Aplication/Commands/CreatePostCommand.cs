using Cibra.AgriculturalPosts.Application.DTOs;
using Cibra.AgriculturalPosts.Domain.Entities;
using Cibra.AgriculturalPosts.Domain.Interfaces;

namespace Cibra.AgriculturalPosts.Application.Commands;

// Command
public record CreatePostCommand(string UserId, string Content, string? Location) : ICommand<PostResponse>;

public record UpdatePostCommand(Guid Id, string Content) : ICommand<PostResponse>;

public record DeletePostCommand(Guid Id) : ICommand<bool>;

public record ProcessAIMentionCommand(Guid PostId, string Query) : ICommand<AIInteractionDto>;

// Command Handlers
public class CreatePostCommandHandler : ICommandHandler<CreatePostCommand, PostResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAIService _aiService;

    public CreatePostCommandHandler(IUnitOfWork unitOfWork, IAIService aiService)
    {
        _unitOfWork = unitOfWork;
        _aiService = aiService;
    }

    public async Task<PostResponse> Handle(CreatePostCommand command, CancellationToken cancellationToken)
    {
        var post = new Post(command.UserId, command.Content, command.Location);

        // Create post first
        await _unitOfWork.Posts.CreateAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Analyze with AI asynchronously (fire and forget with proper error handling)
        _ = Task.Run(async () =>
        {
            try
            {
                var analysis = await _aiService.AnalyzePostAsync(post.Content, cancellationToken);
                post.SetAnalysis(analysis);

                var interaction = new AIInteraction
                {
                    UserQuery = "Auto-analysis on post creation",
                    AIResponse = analysis.RawAIResponse,
                    Type = InteractionType.AutoAnalysis,
                    TokensUsed = EstimateTokens(post.Content + analysis.RawAIResponse)
                };

                post.AddInteraction(interaction);
                await _unitOfWork.Posts.UpdateAsync(post, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the post creation
                Console.WriteLine($"AI Analysis failed: {ex.Message}");
            }
        }, cancellationToken);

        return MapToResponse(post);
    }

    private static int EstimateTokens(string text) => text.Length / 4;

    private static PostResponse MapToResponse(Post post)
    {
        return new PostResponse(
            post.Id,
            post.UserId,
            post.Content,
            post.CreatedAt,
            post.UpdatedAt,
            post.Status.ToString(),
            post.Analysis != null ? new PostAnalysisDto(
                post.Analysis.CultureType,
                post.Analysis.Stage.ToString(),
                post.Analysis.Problems.Select(p => new IdentifiedProblemDto(
                    p.Type.ToString(),
                    p.Description,
                    p.Severity
                )).ToList(),
                post.Analysis.Recommendations,
                post.Analysis.ConfidenceScore,
                post.Analysis.AnalyzedAt
            ) : null,
            post.Interactions.Select(i => new AIInteractionDto(
                i.Id,
                i.UserQuery,
                i.AIResponse,
                i.Type.ToString(),
                i.CreatedAt,
                i.TokensUsed
            )).ToList(),
            post.Tags,
            post.Location
        );
    }
}

public class UpdatePostCommandHandler : ICommandHandler<UpdatePostCommand, PostResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePostCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PostResponse> Handle(UpdatePostCommand command, CancellationToken cancellationToken)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Post {command.Id} not found");

        post.UpdateContent(command.Content);
        await _unitOfWork.Posts.UpdateAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(post);
    }

    private static PostResponse MapToResponse(Post post)
    {
        return new PostResponse(
            post.Id,
            post.UserId,
            post.Content,
            post.CreatedAt,
            post.UpdatedAt,
            post.Status.ToString(),
            post.Analysis != null ? new PostAnalysisDto(
                post.Analysis.CultureType,
                post.Analysis.Stage.ToString(),
                post.Analysis.Problems.Select(p => new IdentifiedProblemDto(
                    p.Type.ToString(),
                    p.Description,
                    p.Severity
                )).ToList(),
                post.Analysis.Recommendations,
                post.Analysis.ConfidenceScore,
                post.Analysis.AnalyzedAt
            ) : null,
            post.Interactions.Select(i => new AIInteractionDto(
                i.Id,
                i.UserQuery,
                i.AIResponse,
                i.Type.ToString(),
                i.CreatedAt,
                i.TokensUsed
            )).ToList(),
            post.Tags,
            post.Location
        );
    }
}

public class ProcessAIMentionCommandHandler : ICommandHandler<ProcessAIMentionCommand, AIInteractionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAIService _aiService;

    public ProcessAIMentionCommandHandler(IUnitOfWork unitOfWork, IAIService aiService)
    {
        _unitOfWork = unitOfWork;
        _aiService = aiService;
    }

    public async Task<AIInteractionDto> Handle(ProcessAIMentionCommand command, CancellationToken cancellationToken)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(command.PostId, cancellationToken)
            ?? throw new KeyNotFoundException($"Post {command.PostId} not found");

        var response = await _aiService.ProcessUserMentionAsync(
            command.Query,
            post.Content,
            post.Interactions,
            cancellationToken
        );

        var interaction = new AIInteraction
        {
            UserQuery = command.Query,
            AIResponse = response,
            Type = InteractionType.UserMention,
            TokensUsed = EstimateTokens(command.Query + response)
        };

        post.AddInteraction(interaction);
        await _unitOfWork.Posts.UpdateAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AIInteractionDto(
            interaction.Id,
            interaction.UserQuery,
            interaction.AIResponse,
            interaction.Type.ToString(),
            interaction.CreatedAt,
            interaction.TokensUsed
        );
    }

    private static int EstimateTokens(string text) => text.Length / 4;
}

// Base interfaces
public interface ICommand<TResponse> { }

public interface ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken);
}