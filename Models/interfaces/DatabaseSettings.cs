namespace ToyEcommerceASPNET.Models.interfaces;

public class DatabaseSettings : IDatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ProductCollectionName { get; set; } = string.Empty;
    public string UserCollectionName { get; set; } = string.Empty;
    public string CartCollectionName { get; set; } = string.Empty;

}