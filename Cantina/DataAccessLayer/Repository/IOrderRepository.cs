using DataAccessLayer.Models;

namespace DataAccessLayer.Repository
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<List<DetailedOrder>> GetOrdersByUser(string userId);
        Task<List<DetailedOrder>> GetFiveAwaitingCourierAssignment();
        Task<List<Order>> DeliveryGetOrdersByStatus(string deliveryId, string status);
        Task UpdateStatus(int orderId, string status);
        Task<bool> CheckAlreadyDelivery(string deliveryId);
        Task<DetailedOrder> GetDetailedOrder(int id);
        Task<List<DetailedOrder>> GetAllOrdersByStatus(string status);
        Task<bool> SetDeliveryPerson(string deliveryPersonId, int orderId);
        Task<string> GetVerificationCode(int orderId);
        Task UpdateLocation(int orderId, string location);
        Task<string> GetDeliveryStatus(string deliveryId);
        Task<string> GetUserId(int orderId);
    }
}
