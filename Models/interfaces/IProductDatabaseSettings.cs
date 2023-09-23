namespace ToyEcommerceASPNET.Models.interfaces;

public interface IProductDatabaseSettings
{
    string CollectionName { get; set; }
    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
}