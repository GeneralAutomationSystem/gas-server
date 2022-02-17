using System;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading;
using Azure.Identity;
using Azure.Messaging.EventHubs;
using Gas.Services.Cosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;


namespace Gas.Functions;
public class Report : Record
{
    public string DeviceId { get; set; }
    public long Rssi { get; set; }
}
public class ReportFunction
{
    private readonly ICosmosService cosmosService;
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };


    public ReportFunction(ICosmosService cosmosService)
    {
        this.cosmosService = cosmosService;
    }

    [FunctionName("ReportFunction")]
    public void Run([IoTHubTrigger("%EventHubName%", Connection = "EventHubConnection")] EventData message, ILogger log, CancellationToken cancelToken)
    {
        var json = message.EventBody.ToObjectFromJson<Report>(options);

        json.DeviceId = (string)message.SystemProperties["iothub-connection-device-id"];
        json.Id = Guid.NewGuid().ToString();

        cosmosService.CreateItemAsync("reports", json, new PartitionKey(json.DeviceId), cancelToken);
    }
}
