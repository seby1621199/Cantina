using BusinessLogic.Models;
using DataAccessLayer.Models;

namespace BusinessLogic.Services
{
    public interface IKitchenService
    {
        Task StartPackaging(int orderId);
        Task<List<OrderUserDetails>> GetAllOrdersKitchen();
        Task<List<OrderUserDetails>> GetPickupOrders();
        Task<OrderUserDetails> GetOrderUserDetailsById(int orderId);
        Task<string> FinishPackaging(int orderId);
        Task CancelOrder(int orderId);
    }
}
