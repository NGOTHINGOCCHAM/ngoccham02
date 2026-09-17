namespace DemoMVC.Models;

public class CatalogItem
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public bool OnSale { get; set; }
}
