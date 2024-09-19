namespace BusinessLogic.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public List<ItemModel> orderItems { get; set; } = new List<ItemModel>();
        public decimal TotalPrice => orderItems.Sum(x => x.UnitPrice * x.Quantity);
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public string VerificationCode { get; set; }
        public string UserId { get; set; }
    }
}
