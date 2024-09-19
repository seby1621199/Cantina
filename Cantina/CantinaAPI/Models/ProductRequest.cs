namespace CantinaAPI.Models;

public class ProductRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public bool Active { get; set; }
    public string Unit { get; set; }
}
