using Microsoft.Azure.Cosmos;

namespace Gas.Services.Cosmos;

public class CosmosService : ICosmosService
{
    private readonly CosmosConfiguration databaseConfig;

    public CosmosService(string connectionString, string databaseName, Action<CosmosDbConfigurationBuilder> builderAction)
    {
        var builder = new CosmosDbConfigurationBuilder(connectionString, databaseName);
        builderAction(builder);
        databaseConfig = builder.Build();
    }

    public async Task<T> ReadItemAsync<T>(string containerKey, string itemId, PartitionKey partitionKey, CancellationToken cancelToken = default)
    {
        var container = databaseConfig.GetContainer(containerKey);
        return await container.ReadItemAsync<T>(itemId, partitionKey, cancellationToken: cancelToken);
    }

    public async Task<List<T>> ReadItemsAsync<T>(string containerKey, QueryDefinition query, CancellationToken cancelToken = default)
    {
        var container = databaseConfig.GetContainer(containerKey);
        var items = new List<T>();
        using var feed = container.GetItemQueryIterator<T>(query);
        while (feed.HasMoreResults)
        {
            var batch = (await feed.ReadNextAsync(cancelToken)).Resource;
            items.AddRange(batch);
        }
        return items;
    }

    public async Task<(List<T>, string)> ReadItemsAsync<T>(string containerKey, QueryDefinition query, string continuationToken, CancellationToken cancelToken = default)
    {
        var container = databaseConfig.GetContainer(containerKey);
        using var feed = container.GetItemQueryIterator<T>(query, continuationToken);

        var response = await feed.ReadNextAsync(cancelToken);
        continuationToken = response.ContinuationToken;
        var items = new List<T>(response.Resource);

        return (items, continuationToken);
    }

    public async Task CreateItemAsync<T>(string containerKey, T item, PartitionKey partitionKey, CancellationToken cancelToken = default)
    {
        var container = databaseConfig.GetContainer(containerKey);
        await container.CreateItemAsync(item, partitionKey, cancellationToken: cancelToken);
    }

    public async Task UpsertItemAsync<T>(string containerKey, T item, PartitionKey partitionKey, CancellationToken cancelToken = default)
    {
        var container = databaseConfig.GetContainer(containerKey);
        await container.UpsertItemAsync(item, partitionKey, cancellationToken: cancelToken);
    }

    public async Task UpsertItemsAsync<T>(string containerKey, IEnumerable<T> items, PartitionKey partitionKey, CancellationToken cancelToken = default)
    {
        var container = databaseConfig.GetContainer(containerKey);
        var tasks = new List<Task>();
        foreach (var item in items)
        {
            tasks.Add(container.UpsertItemAsync(item, partitionKey, cancellationToken: cancelToken));
        }
        await Task.WhenAll(tasks);
    }

    public async Task ReplaceItemAsync<T>(string containerKey, T item, string itemId, PartitionKey partitionKey, CancellationToken cancelToken = default)
    {
        var container = databaseConfig.GetContainer(containerKey);
        await container.ReplaceItemAsync(item, itemId, partitionKey, cancellationToken: cancelToken);
    }
}
