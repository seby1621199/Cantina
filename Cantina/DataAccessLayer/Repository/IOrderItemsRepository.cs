using DataAccessLayer.Models;

namespace DataAccessLayer.Repository
{
    public interface IOrderItemsRepository : IBaseRepository<OrderItems>
    {
        Task<List<OrderItems>> GetAllByOrderId(int orderId);
    }
}
