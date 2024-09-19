using DataAccessLayer.Models;

namespace DataAccessLayer.Repository
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task UpdateStock(int productId, int quantity);
    }
}
