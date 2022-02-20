using Gas.Services.Devices;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Gas.WebApp.Controllers;

[Route("Device/{id}/Status")]
public class DeviceStatusController : BaseController
{
    public DeviceStatusController(ILogger<DeviceStatusController> logger, IConfiguration config, CosmosClient cosmosClient, IDeviceService deviceService) : base(logger, config, cosmosClient, deviceService) { }

    public async Task<IActionResult> IndexAsync(string id)
    {
        var model = await NewBaseModel<StatusModel>("pepa", id);

        if (model?.UserDevices == null || !model.UserDevices.Select(d => d.Id).Contains(id))
        {
            return RedirectToAction("Index", "DeviceSelect");
        }

        model.Twin = await deviceService.GetTwinAsync(id);

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
