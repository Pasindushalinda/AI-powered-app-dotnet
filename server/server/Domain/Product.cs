namespace server.Domain;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public float Price { get; set; }

    public ICollection<Review> Reviews { get; set; } = [];
    public Summary? Summary { get; set; }
}
