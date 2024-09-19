namespace AdminPanel.Models;

public class ProductUpdate
{
    public string name { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public decimal price { get; set; } = 0;
    public int stock { get; set; } = 0;
    public bool active { get; set; } = false;
    public string unit { get; set; } = string.Empty;
}
