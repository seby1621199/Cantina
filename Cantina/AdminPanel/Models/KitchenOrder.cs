namespace AdminPanel.Models
{
    public class KitchenOrder
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string phoneNumber { get; set; } = string.Empty;
        public string verificationCode { get; set; } = string.Empty;
        public string status { get; set; }= string.Empty;
        public decimal totalPrice { get; set; } = 0;
        public List<KitchenProduct> products { get; set; } = new List<KitchenProduct>();
    }
}
