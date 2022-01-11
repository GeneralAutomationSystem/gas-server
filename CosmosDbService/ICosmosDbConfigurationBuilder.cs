namespace Gas.Services.CosmosDb;
public interface ICosmosDbConfigurationBuilder
{
    ICosmosDbConfigurationBuilder AddContainer(string containerName, string? containerKey = null);
    CosmosDbConfiguration Build();
}