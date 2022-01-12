using Gas.Common.Models.Device;
using Gas.Services.Cosmos;
using Gas.Services.Devices;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Gas.WebApp.Controllers;

[Route("Device/Status")]
public class DeviceStatusController : BaseController
{
    public DeviceStatusController(ILogger<DeviceStatusController> logger, ICosmosService dbService, IDeviceService deviceService) : base(logger, dbService, deviceService) { }

    [HttpGet("{id?}")]
    public async Task<IActionResult> IndexAsync(string? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index", "DeviceSelect");
        }
        var model = await NewBaseModel<StatusModel>("pepa", id);

        if (!model.UserDevices.Select(d => d.Id).Contains(id))
        {
            return RedirectToAction("Index", "DeviceSelect");
        }

        model.Twin = await deviceService.GetTwinAsync(id);

        return View(model);
    }
}
