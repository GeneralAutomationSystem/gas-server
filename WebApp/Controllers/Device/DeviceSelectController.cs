using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Devices;

namespace Gas.WebApp.Controllers;

[Route("Device")]
public class DeviceSelectController : BaseController
{
    public DeviceSelectController(ILogger<DeviceSelectController> logger, IConfiguration config, CosmosClient cosmosClient) : base(logger, config, cosmosClient)
    {
    }

    public async Task<IActionResult> IndexAsync()
    {
        var model = new BaseModel();
        await FillBaseModel(model,null);
        return View(model);
    }
}
