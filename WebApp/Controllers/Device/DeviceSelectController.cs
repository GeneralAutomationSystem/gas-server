using Gas.Common.Models.Device;
using Gas.Services.Cosmos;
using Gas.Services.Devices;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Gas.WebApp.Controllers;

[Route("Device")]
public class DeviceSelectController : BaseController
{
    public DeviceSelectController(ILogger<DeviceSelectController> logger, ICosmosService dbService, IDeviceService deviceService) : base(logger, dbService, deviceService) { }

    public async Task<IActionResult> IndexAsync()
    {
        var model = await NewBaseModel<BaseModel>("pepa", null);
        return View(model);
    }
}
