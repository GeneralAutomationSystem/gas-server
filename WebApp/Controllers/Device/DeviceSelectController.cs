using Gas.Services.Devices;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Gas.WebApp.Controllers;

[Route("Device")]
public class DeviceSelectController : BaseController
{
    public DeviceSelectController(ILogger<DeviceSelectController> logger, IConfiguration config, CosmosClient cosmosClient, IDeviceService deviceService) : base(logger, config, cosmosClient, deviceService) { }

    public async Task<IActionResult> IndexAsync()
    {
        var model = await NewBaseModel<BaseModel>("pepa", null);
        return View(model);
    }
}
