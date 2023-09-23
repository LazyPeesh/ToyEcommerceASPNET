namespace ToyEcommerceASPNET.Models.interfaces;

public class ProductDatabaseSettings : IProductDatabaseSettings
{
    public string CollectionName { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}