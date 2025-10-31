using Cibra.AgriculturalPosts.Application.DTOs;
using Cibra.AgriculturalPosts.Domain.Entities;
using Cibra.AgriculturalPosts.Domain.Interfaces;

namespace Cibra.AgriculturalPosts.Application.Queries;

// Queries
public record GetPostByIdQuery(Guid Id) : IQuery<PostResponse>;

public record GetAllPostsQuery(int Page = 1, int PageSize = 10) : IQuery<PagedResponse<PostResponse>>;

public record GetUserPostsQuery(string UserId) : IQuery<List<PostResponse>>;

// Query Handlers
public class GetPostByIdQueryHandler : IQueryHandler<GetPostByIdQuery, PostResponse>
{
    private readonly IPostRepository _repository;

    public GetPostByIdQueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<PostResponse> Handle(GetPostByIdQuery query, CancellationToken cancellationToken)
    {
        var post = await _repository.GetByIdAsync(query.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Post {query.Id} not found");

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

public class GetAllPostsQueryHandler : IQueryHandler<GetAllPostsQuery, PagedResponse<PostResponse>>
{
    private readonly IPostRepository _repository;

    public GetAllPostsQueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResponse<PostResponse>> Handle(GetAllPostsQuery query, CancellationToken cancellationToken)
    {
        var posts = await _repository.GetAllAsync(query.Page, query.PageSize, cancellationToken);
        var totalCount = await _repository.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        var postResponses = posts.Select(MapToResponse).ToList();

        return new PagedResponse<PostResponse>(
            postResponses,
            query.Page,
            query.PageSize,
            totalCount,
            totalPages
        );
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

public class GetUserPostsQueryHandler : IQueryHandler<GetUserPostsQuery, List<PostResponse>>
{
    private readonly IPostRepository _repository;

    public GetUserPostsQueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PostResponse>> Handle(GetUserPostsQuery query, CancellationToken cancellationToken)
    {
        var posts = await _repository.GetByUserIdAsync(query.UserId, cancellationToken);
        return posts.Select(MapToResponse).ToList();
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

// Base interfaces
public interface IQuery<TResponse> { }

public interface IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}