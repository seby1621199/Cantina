using DataAccessLayer.Models;

public class Order : IEntity
{
    public int Id { get; set; }
    public DateTime OrderDate { get; } = DateTime.Now;
    public string UserId { get; set; }
    public string Location { get; set; }
    public string Status { get; set; } = "Pending";
    public string VerificationCode { get; set; } = Guid.NewGuid().ToString().Substring(0, 8);
    public string? DeliveryPersonId { get; set; }

    public User User { get; set; }
    public User? DeliveryPerson { get; set; }
}
