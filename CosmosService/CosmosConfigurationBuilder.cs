namespace Gas.Services.Cosmos;

public class CosmosDbConfigurationBuilder : ICosmosConfigurationBuilder
{
    private readonly CosmosConfiguration databaseConfig;

    public CosmosDbConfigurationBuilder(string connectionString, string databaseName)
    {
        databaseConfig = new CosmosConfiguration(connectionString, databaseName);
    }

    public ICosmosConfigurationBuilder AddContainer(string containerName, string? containerKey = null)
    {
        if (containerKey == null)
        {
            containerKey = containerName;
        }
        databaseConfig.Containers.Add(containerKey, databaseConfig.Client.GetContainer(databaseConfig.DatabaseName, containerName));
        return this;
    }

    public CosmosConfiguration Build()
    {
        return databaseConfig;
    }
}