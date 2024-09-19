namespace BusinessLogic.Models
{
    public class DeliveryDetailsOrder
    {
        public int Id { get; set; }
        public string VerificationCode { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; 
    }
}
