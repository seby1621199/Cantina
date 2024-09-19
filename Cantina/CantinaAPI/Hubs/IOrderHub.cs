using CantinaAPI.Models;

namespace CantinaAPI.Hubs
{
    public interface IOrderHub
    {
        Task SendOrderNotification(List<OrderItemRequest> message);
    }
}
