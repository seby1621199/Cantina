using BusinessLogic.Models;

namespace BusinessLogic.Services
{
    public interface IOrderService
    {
        Task<int> AddOrder(string userEmail, List<Items> orderItems, bool selfPickup);
        Task ChangeToSelfPickup(int orderId, string? email);
        Task DeleteOrder(string userEmail, int orderId);
        Task<OrderModel> GetOrderById(int id);
        Task<List<OrderModel>> GetOrders(string userEmail);
    }
}
