using System.ComponentModel.Design;
using Adamijak.Azure.Cosmos.Extensions;
using Gas.Common.Extensions;
using Gas.Common.Items;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Devices;

namespace Gas.WebApp.Controllers;

[Route("Device/{deviceId}/Status")]
public class DeviceStatusController : BaseController
{
    private readonly Container reportContainer;
    private readonly RegistryManager registryManager;
    public DeviceStatusController(ILogger<DeviceStatusController> logger, IConfiguration config, CosmosClient cosmosClient, RegistryManager registryManager) : base(logger, config, cosmosClient)
    {
        reportContainer = cosmosClient.GetContainer(config.GetDatabaseId(), config.GetReportContainerId());
        this.registryManager = registryManager;
    }

    public async Task<IActionResult> IndexAsync(string deviceId)
    {
        var model = new StatusModel();
        await FillBaseModel(model, deviceId);

        if (model?.UserDevices == null || !model.UserDevices.Select(d => d.Id).Contains(deviceId))
        {
            return RedirectToAction("Index", "DeviceSelect");
        }

        var query = new QueryDefinition("SELECT * FROM c where c.deviceId = @deviceId and c.dateTime > @date")
            .WithParameter("@deviceId", deviceId)
            .WithParameter("@date", DateTime.UtcNow.AddDays(-7));

        var reports = await reportContainer.GetItemsAsync<DeviceReport>(query);

        model.SystemTemperatures = reports.Select(r => (r.DateTime, (r.SystemTemperature0+r.SystemTemperature1)/2)).ToList();
        model.Rssis = reports.Select(r => (r.DateTime, r.Rssi)).ToList();

        return View(model);
    }
}
