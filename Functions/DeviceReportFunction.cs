using Azure.Messaging.EventHubs;
using Gas.Common.Extensions;
using Gas.Common.Items;
using Gas.Common.Static;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gas.Functions;

public class DeviceReportFunction
{
    private readonly Container container;

    public DeviceReportFunction(IConfiguration config, CosmosClient cosmosClient)
    {
        container = cosmosClient.GetContainer(config.GetDatabaseId(), config.GetReportContainerId());
    }

    [FunctionName("DeviceReportFunction")]
    public async Task RunAsync([EventHubTrigger("%EventHubName%", Connection = "EventHubConnection")] IEnumerable<EventData> messages, ILogger log, CancellationToken cancelToken)
    {
        var tasks = new List<Task>();
        foreach (var message in messages)
        {
            var item = message.EventBody.ToObjectFromJson<DeviceReport>(JsonOptions.DefaultSerialization);

            item.Id = Guid.NewGuid().ToString();
            item.DeviceId = (string)message.SystemProperties["iothub-connection-device-id"];
            item.ProcessedDate = DateTime.UtcNow;

            log.LogInformation($"Creating new report with id: {item.Id}");
            tasks.Add(container.CreateItemAsync(item, new PartitionKey(item.DeviceId), new ItemRequestOptions { EnableContentResponseOnWrite = false }, cancelToken));
        }
        await Task.WhenAll(tasks);
    }
}
