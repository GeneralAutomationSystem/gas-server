namespace Gas.CosmosDb;
public interface ICosmosDbConfigurationBuilder
{
    ICosmosDbConfigurationBuilder AddContainer(string containerName, string? containerKey = null);
    CosmosDbConfiguration Build();
}