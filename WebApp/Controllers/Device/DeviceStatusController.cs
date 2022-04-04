using Gas.Common.Extensions;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Devices;

namespace Gas.WebApp.Controllers;

[Route("Device/{id}/Status")]
public class DeviceStatusController : BaseController
{
    private readonly Container reportContainer;
    private readonly RegistryManager registryManager;
    public DeviceStatusController(ILogger<DeviceStatusController> logger, IConfiguration config, CosmosClient cosmosClient, RegistryManager registryManager) : base(logger, config, cosmosClient)
    {
        reportContainer = cosmosClient.GetContainer(config.GetDatabaseId(), config.GetReportContainerId());
        this.registryManager = registryManager;
    }

    public async Task<IActionResult> IndexAsync(string id)
    {
        var model = await NewBaseModel<StatusModel>("pepa", id);

        if (model?.UserDevices == null || !model.UserDevices.Select(d => d.Id).Contains(id))
        {
            return RedirectToAction("Index", "DeviceSelect");
        }

        var systemTemperatures = new List<(string, int)>();
        for (var i = 0; i < 1000; i++)
        {
            systemTemperatures.Add((i.ToString(), i));
        }

        model.SystemTemperatures = systemTemperatures;

        return View(model);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> DataAsync(string id)
    {
        var model = await NewBaseModel<StatusModel>("pepa", id);

        if (model?.UserDevices == null || !model.UserDevices.Select(d => d.Id).Contains(id))
        {
            return RedirectToAction("Index", "DeviceSelect");
        }


        return View("Index", model);
    }
}
