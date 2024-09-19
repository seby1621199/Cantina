using DataAccessLayer.Models;

namespace DataAccessLayer.Repository
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(CantinaContext context) : base(context)
        {
        }

        public async Task UpdateStock(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            product.Stock = quantity;
            await _context.SaveChangesAsync();
        }
    }
}
