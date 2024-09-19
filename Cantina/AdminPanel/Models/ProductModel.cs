namespace AdminPanel.Models;

public class ProductModel
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public string? description { get; set; } = string.Empty;
    public decimal price { get; set; } = 0;
    public int stock { get; set; } = 0;
    public string unit { get; set; } = string.Empty;
    public bool active { get; set; } = false;
}
