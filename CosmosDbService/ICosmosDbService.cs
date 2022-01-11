using Microsoft.Azure.Cosmos;

namespace Gas.Services.CosmosDb;
public interface ICosmosDbService
{
    Task<T> ReadItemAsync<T>(string containerKey, string itemId, PartitionKey partitionKey, CancellationToken cancelToken = default);
    Task<List<T>> ReadItemsAsync<T>(string containerKey, QueryDefinition query, CancellationToken cancelToken = default);
    Task<(List<T>, string)> ReadItemsAsync<T>(string containerKey, QueryDefinition query, string continuationToken, CancellationToken cancelToken = default);
    Task CreateItemAsync<T>(string containerKey, T item, PartitionKey partitionKey, CancellationToken cancelToken = default);
    Task UpsertItemAsync<T>(string containerKey, T item, PartitionKey partitionKey, CancellationToken cancelToken = default);
    Task UpsertItemsAsync<T>(string containerKey, IEnumerable<T> items, PartitionKey partitionKey, CancellationToken cancelToken = default);
    Task ReplaceItemAsync<T>(string containerKey, T item, string itemId, PartitionKey partitionKey, CancellationToken cancelToken = default);
}