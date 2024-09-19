namespace CantinaAPI.Models
{
    public class OrderNotification
    {
        public int orderId { get; set; }
        public string status { get; set; }
        public OrderNotification( int orderId, string status)
        {
            this.orderId = orderId;
            this.status = status;
        }
    }
}
