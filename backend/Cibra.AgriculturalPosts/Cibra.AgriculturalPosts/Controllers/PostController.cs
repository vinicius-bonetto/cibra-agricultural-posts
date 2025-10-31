using Microsoft.AspNetCore.Mvc;
using Cibra.AgriculturalPosts.Application.Commands;
using Cibra.AgriculturalPosts.Application.DTOs;
using Cibra.AgriculturalPosts.Application.Queries;

namespace Cibra.AgriculturalPosts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PostsController : ControllerBase
{
    private readonly ILogger<PostsController> _logger;

    public PostsController(ILogger<PostsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all posts with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PostResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<PostResponse>>> GetAll(
        [FromServices] GetAllPostsQueryHandler handler,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetAllPostsQuery(page, pageSize);
            var result = await handler.Handle(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all posts");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Get post by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PostResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostResponse>> GetById(
        [FromServices] GetPostByIdQueryHandler handler,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetPostByIdQuery(id);
            var result = await handler.Handle(query, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Post {id} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting post {PostId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Get posts by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(List<PostResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PostResponse>>> GetByUserId(
        [FromServices] GetUserPostsQueryHandler handler,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetUserPostsQuery(userId);
            var result = await handler.Handle(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts for user {UserId}", userId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Create a new post (AI analysis runs automatically in background)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PostResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PostResponse>> Create(
        [FromServices] CreatePostCommandHandler handler,
        [FromBody] CreatePostRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest(new { error = "Content is required" });
            }

            // For demo purposes, using a fixed user ID. In production, get from auth token
            var userId = "user-demo-001";

            var command = new CreatePostCommand(userId, request.Content, request.Location);
            var result = await handler.Handle(command, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating post");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Update post content
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PostResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostResponse>> Update(
        [FromServices] UpdatePostCommandHandler handler,
        Guid id,
        [FromBody] UpdatePostRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new UpdatePostCommand(id, request.Content);
            var result = await handler.Handle(command, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Post {id} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating post {PostId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Mention AI assistant for specific analysis (@AssistenteIA)
    /// </summary>
    [HttpPost("{id:guid}/mention")]
    [ProducesResponseType(typeof(AIInteractionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AIInteractionDto>> MentionAI(
        [FromServices] ProcessAIMentionCommandHandler handler,
        Guid id,
        [FromBody] AIMentionRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return BadRequest(new { error = "Query is required" });
            }

            var command = new ProcessAIMentionCommand(id, request.Query);
            var result = await handler.Handle(command, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = $"Post {id} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing AI mention for post {PostId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}