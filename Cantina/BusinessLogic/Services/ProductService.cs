using BusinessLogic.Extensions;
using BusinessLogic.Models;
using DataAccessLayer.Repository;

namespace BusinessLogic.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task AddProduct(ProductDomain productModel)
        {
            try
            {
                await _productRepository.Add(productModel.ToProductDAL());
            }
            catch (System.Exception ex)
            {
                throw new ApplicationException("A apărut o eroare la adăugarea produsului", ex);
            }
        }

        public async Task<List<ProductDomain>> GetAll()
        {
            var products = await _productRepository.GetAll();
            return products.Select(p => p.ToProductDomain()).ToList();
        }

        public async Task UpdateProduct(int id, ProductDomain productModel)
        {
            await _productRepository.Update(id, productModel.ToProductDAL());
        }

        public async Task UpdateStock(int id, int stock)
        {
            await _productRepository.UpdateStock(id, stock);
        }

        public async Task DeleteProduct(int id)
        {
            await _productRepository.Delete(id);
        }

        public async Task<ProductDomain> GetProduct(int id)
        {
            var product = await _productRepository.GetById(id);

            return product?.ToProductDomain();
        }

        public async Task ChangeStatus(int id, bool status)
        {
            var product = await _productRepository.GetById(id);
            product.IsActive = status;
            await _productRepository.Update(id, product);
        }
    }
}
