using BusinessLogic.Models;

namespace CantinaAPI.Models
{
    public class DetailedOrder
    {
        public int Id { get; set; }
        public List<ItemModel> orderItems { get; set; } = new List<ItemModel>();
        public decimal TotalPrice => orderItems.Sum(x => x.UnitPrice * x.Quantity);
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
    }
}
