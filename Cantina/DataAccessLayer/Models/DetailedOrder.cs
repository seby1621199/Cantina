namespace DataAccessLayer.Models
{
    public class DetailedOrder : Order
    {
        public List<OrderItems> orderItems { get; set; } = new List<OrderItems>();
    }
}
