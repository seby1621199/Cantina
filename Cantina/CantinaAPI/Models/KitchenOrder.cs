namespace CantinaAPI.Models
{
    public class KitchenOrder
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; } = 0;
        public List<KitchenProduct> Products { get; set; } = new List<KitchenProduct>();
    }
}
