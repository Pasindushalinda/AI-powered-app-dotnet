namespace server.Domain;

public class Review
{
    public int Id { get; set; }
    public string Author { get; set; } = string.Empty;
    public short Rating { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
