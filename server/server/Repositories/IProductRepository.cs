using server.Domain;

namespace server.Repositories;

public interface IProductRepository
{
    Task<Product?> GetProductAsync(int productId);
}
