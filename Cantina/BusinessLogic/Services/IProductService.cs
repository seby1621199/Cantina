using BusinessLogic.Models;

namespace BusinessLogic.Services
{
    public interface IProductService
    {
        Task AddProduct(ProductDomain productModel);
        Task<List<ProductDomain>> GetAll();
        Task UpdateProduct(int id, ProductDomain productModel);
        Task DeleteProduct(int id);
        Task UpdateStock(int id, int stock);
        Task<ProductDomain> GetProduct(int id);
        Task ChangeStatus(int id, bool status);
    }
}
