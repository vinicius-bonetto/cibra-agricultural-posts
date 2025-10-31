namespace Cibra.AgriculturalPosts.Domain.Entities;

public class Post
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public PostStatus Status { get; private set; }

    // AI Analysis
    public PostAnalysis? Analysis { get; private set; }
    public List<AIInteraction> Interactions { get; private set; }

    // Metadata
    public List<string> Tags { get; private set; }
    public string? Location { get; private set; }

    private Post()
    {
        Interactions = new List<AIInteraction>();
        Tags = new List<string>();
    }

    public Post(string userId, string content, string? location = null)
    {
        Id = Guid.NewGuid();
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Location = location;
        CreatedAt = DateTime.UtcNow;
        Status = PostStatus.Draft;
        Interactions = new List<AIInteraction>();
        Tags = new List<string>();
    }

    public void UpdateContent(string content)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAnalysis(PostAnalysis analysis)
    {
        Analysis = analysis ?? throw new ArgumentNullException(nameof(analysis));
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddInteraction(AIInteraction interaction)
    {
        Interactions.Add(interaction ?? throw new ArgumentNullException(nameof(interaction)));
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        Status = PostStatus.Published;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        Status = PostStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum PostStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2
}