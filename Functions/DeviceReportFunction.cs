using System.Text.Json;
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
    public async Task RunAsync([EventHubTrigger("%EventHubName%", Connection = "EventHubConnection")] EventData message, ILogger log, CancellationToken cancelToken)
    {
        var data = message.EventBody.ToObjectFromJson<dynamic>(JsonOptions.DefaultSerialization);

        var item = new DeviceReport
        {
            Id = Guid.NewGuid().ToString(),
            DeviceId = (string)message.SystemProperties["iothub-connection-device-id"],
            ProcessedDate = DateTime.UtcNow,
            Data = data,
        };

        var itemStream = new MemoryStream();
        await JsonSerializer.SerializeAsync(itemStream, item, JsonOptions.DefaultSerialization, cancelToken);

        log.LogInformation($"Creating new report with id: {item.Id}");
        await container.CreateItemStreamAsync(itemStream, new PartitionKey(item.DeviceId), new ItemRequestOptions { EnableContentResponseOnWrite = false }, cancelToken);
    }
}
