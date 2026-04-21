using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Domain;

namespace server.Repositories;

public class ProductRepository(AppDbContext db) : IProductRepository
{
    public Task<Product?> GetProductAsync(int productId) =>
        db.Products.FirstOrDefaultAsync(p => p.Id == productId);
}
