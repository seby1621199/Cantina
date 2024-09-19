namespace BusinessLogic.Models
{
    public class PickUpOrderModel
    {
        public int Id { get; set; }
        public List<ItemModel> orderItems { get; set; } = new List<ItemModel>();
        public decimal TotalPrice => orderItems.Sum(x => x.UnitPrice * x.Quantity);
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
    }
}
