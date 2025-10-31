namespace Cibra.AgriculturalPosts.Application.DTOs;

public record CreatePostRequest(
    string Content,
    string? Location
);

public record UpdatePostRequest(
    string Content
);

public record PostResponse(
    Guid Id,
    string UserId,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string Status,
    PostAnalysisDto? Analysis,
    List<AIInteractionDto> Interactions,
    List<string> Tags,
    string? Location
);

public record PostAnalysisDto(
    string CultureType,
    string Stage,
    List<IdentifiedProblemDto> Problems,
    List<string> Recommendations,
    double ConfidenceScore,
    DateTime AnalyzedAt
);

public record IdentifiedProblemDto(
    string Type,
    string Description,
    string Severity
);

public record AIInteractionDto(
    Guid Id,
    string UserQuery,
    string AIResponse,
    string Type,
    DateTime CreatedAt,
    int TokensUsed
);

public record AIMentionRequest(
    Guid PostId,
    string Query
);

public record AIAnalysisResponse(
    bool Success,
    PostAnalysisDto? Analysis,
    string? ErrorMessage
);

public record PagedResponse<T>(
    List<T> Data,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);