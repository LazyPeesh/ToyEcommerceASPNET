namespace ToyEcommerceASPNET.Models.interfaces;

public interface IDatabaseSettings
{

    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
    string ProductCollectionName { get; set; }
    string UserCollectionName { get; set; }
    string OrderCollectionName { get; set; }
    string CategoryCollectionName { get; set; }
}