using Azure.Messaging.EventHubs;
using Gas.Common.Extensions;
using Gas.Common.Items;
using Gas.Common.Static;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gas.Functions;

public class ReportFunction
{
    private readonly Container container;

    public ReportFunction(IConfiguration config, CosmosClient cosmosClient)
    {
        container = cosmosClient.GetContainer(config.GetDatabaseId(), config.GetReportContainerId());
    }

    [FunctionName("ReportFunction")]
    public void Run([EventHubTrigger("%EventHubName%", Connection = "EventHubConnection")] EventData message, ILogger log, CancellationToken cancelToken)
    {
        var item = message.EventBody.ToObjectFromJson<Report>(JsonOptions.DefaultSerialization);

        item.DeviceId = (string)message.SystemProperties["iothub-connection-device-id"];
        item.Id = Guid.NewGuid().ToString();

        log.LogInformation($"Creating new report with id: {item.Id}, deviceId: {item.DeviceId}");
        container.CreateItemAsync(item, new PartitionKey(item.DeviceId), new ItemRequestOptions { EnableContentResponseOnWrite = false }, cancelToken);
    }
}
