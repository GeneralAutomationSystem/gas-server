using System;
using System.Threading;
using Azure.Messaging.EventHubs;
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

    public ReportFunction(CosmosClient cosmosClient, IConfiguration config)
    {
        container = cosmosClient.GetContainer(config.GetValue<string>("DatabaseName"), config.GetValue<string>("ReportsContainer"));
    }

    [FunctionName("ReportFunction")]
    public void Run([EventHubTrigger("%EventHubName%", Connection = "EventHubConnection")] EventData message, ILogger log, CancellationToken cancelToken)
    {
        var item = message.EventBody.ToObjectFromJson<Report>(JsonSerializerOptions.Default);

        item.DeviceId = (string)message.SystemProperties["iothub-connection-device-id"];
        item.Id = Guid.NewGuid().ToString();

        container.CreateItemAsync(item, new PartitionKey(item.DeviceId), new ItemRequestOptions { EnableContentResponseOnWrite = false }, cancelToken);
    }
}
