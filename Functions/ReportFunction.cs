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
        var data = message.EventBody.ToObjectFromJson<dynamic>(JsonOptions.DefaultSerialization);

        var item = new DeviceReport
        {
            Id = (string)message.SystemProperties["iothub-connection-device-id"],
            Date = DateTime.UtcNow,
            Data = data,
        };

        log.LogInformation($"Creating new report with id: {item.Id}");
        container.CreateItemAsync(item, new PartitionKey(item.Id), new ItemRequestOptions { EnableContentResponseOnWrite = false }, cancelToken);
    }
}
