namespace Gas.Services.Cosmos;
public interface ICosmosConfigurationBuilder
{
    ICosmosConfigurationBuilder AddContainer(string containerName, string? containerKey = null);
    CosmosConfiguration Build();
}