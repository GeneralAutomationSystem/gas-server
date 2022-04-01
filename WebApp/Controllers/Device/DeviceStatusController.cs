using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Devices;

namespace Gas.WebApp.Controllers;

[Route("Device/{id}/Status")]
public class DeviceStatusController : BaseController
{
    private readonly RegistryManager registryManager;
    public DeviceStatusController(ILogger<DeviceStatusController> logger, IConfiguration config, CosmosClient cosmosClient, RegistryManager registryManager) : base(logger, config, cosmosClient)
    {
        this.registryManager = registryManager;
    }

    public async Task<IActionResult> IndexAsync(string id)
    {
        var model = await NewBaseModel<StatusModel>("pepa", id);

        if (model?.UserDevices == null || !model.UserDevices.Select(d => d.Id).Contains(id))
        {
            return RedirectToAction("Index", "DeviceSelect");
        }

        model.Twin = (await registryManager.GetTwinAsync(id)).ToJson();

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

        model.Twin = "data";

        return View("Index", model);
    }
}
