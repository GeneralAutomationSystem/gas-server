namespace Gas.CosmosDb;

public class CosmosDbConfigurationBuilder : ICosmosDbConfigurationBuilder
{
    private readonly CosmosDbConfiguration databaseConfig;

    public CosmosDbConfigurationBuilder(string connectionString, string databaseName)
    {
        databaseConfig = new CosmosDbConfiguration(connectionString, databaseName);
    }

    public ICosmosDbConfigurationBuilder AddContainer(string containerName, string? containerKey = null)
    {
        if (containerKey == null)
        {
            containerKey = containerName;
        }
        databaseConfig.Containers.Add(containerKey, databaseConfig.Client.GetContainer(databaseConfig.DatabaseName, containerName));
        return this;
    }

    public CosmosDbConfiguration Build()
    {
        return databaseConfig;
    }
}