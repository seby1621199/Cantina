namespace BusinessLogic.Models;

public class ProductDomain
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    private bool active;
    public bool Active
    {
        get
        {
            if (Stock > 0)
            {
                return active;
            }
            else
            {
                return false;
            }
        }
        set
        {
            active = value;
        }
    }
    public string Unit { get; set; }
}
