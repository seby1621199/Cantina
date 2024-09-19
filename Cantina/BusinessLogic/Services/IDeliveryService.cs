using BusinessLogic.Models;

namespace BusinessLogic.Services
{
    public interface IDeliveryService
    {
        Task<List<DeliveryDetailsOrder>> GetFiveAwaitingCourierAssignment(string DeliveryPersonId);
        Task<bool> CheckAlreadyInDelivery(string deliveryId);
        Task ChangeOrderStatus(int orderId, string status);
        Task FinishOrder(int id, string deliveryPersonId);
        Task StartDelivery(string userId);
        Task<string> GetStatus(string? userId);
        Task<List<DeliveryDetailsOrder>> GetOrdersByStatus(string userId, string status);
        Task FinishSelfPickUpOrder(int Id);
        Task<string> GetVerificationCode(int orderId, string userId);
        //Task<List<DeliveryDetailsOrder>> GetOrders(string DeliveryPersonId);
    }
}
