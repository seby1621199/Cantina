namespace CantinaAPI.Handlers
{
    public interface INotificationOrderHandler
    {
        Task SendOrderNotification(int orderid);
        Task SendFinishKitchenNotification(int orderid);
        Task SendStartDeliveryNotification(int orderid);
        Task SendChangedOrderStatusNotification(int orderId, string status);
    }
}
