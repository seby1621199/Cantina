using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository
{
    public class OrderItemsRepository : BaseRepository<OrderItems>, IOrderItemsRepository
    {
        public OrderItemsRepository(CantinaContext context) : base(context)
        {
        }

        public async Task<List<OrderItems>> GetAllByOrderId(int orderId)
        {
            var orderItems = await _context.OrderItems.Where(x => x.OrderId == orderId).ToListAsync();
            foreach (var orderItem in orderItems)
            {
                orderItem.Product = await _context.Products.FirstOrDefaultAsync(x => x.Id == orderItem.ProductId) ?? throw new Exception("No product.");

            }
            return orderItems;
        }
    }


}
