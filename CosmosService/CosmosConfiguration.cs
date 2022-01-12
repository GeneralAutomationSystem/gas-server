using Microsoft.Azure.Cosmos;

namespace Gas.Services.Cosmos;

public class CosmosConfiguration
{
    public string ConnectionString;
    public string DatabaseName;
    public CosmosClient Client;
    public Dictionary<string, Container> Containers = new();

    public CosmosConfiguration(string connectionString, string databaseName)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException($"'{nameof(connectionString)}' cannot be null or whitespace.", nameof(connectionString));
        }

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException($"'{nameof(databaseName)}' cannot be null or whitespace.", nameof(databaseName));
        }

        ConnectionString = connectionString;
        DatabaseName = databaseName;

        Client = new CosmosClient(connectionString, new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase },
            MaxRetryAttemptsOnRateLimitedRequests = 64, // TODO: get better limits
            MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromMinutes(2), // TODO: get better limits
        });
    }

    public Container GetContainer(string containerKey)
    {
        var container = Containers.GetValueOrDefault(containerKey);
        if (container == null)
        {
            throw new KeyNotFoundException($"Can not get container with {nameof(containerKey)}: {containerKey}.");
        }
        return container;
    }
}