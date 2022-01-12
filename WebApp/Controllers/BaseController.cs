using Gas.Common.Models.Device;
using Gas.Services.Cosmos;
using Gas.Services.Devices;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace Gas.WebApp.Controllers;


public class BaseController : Controller
{
    protected readonly ILogger<BaseController> logger;
    protected readonly ICosmosService dbService;
    protected readonly IDeviceService deviceService;

    public BaseController(ILogger<BaseController> logger, ICosmosService dbService, IDeviceService deviceService)
    {
        this.logger = logger;
        this.dbService = dbService;
        this.deviceService = deviceService;
    }

    protected async Task<T?> NewBaseModel<T>(string userPrincipalName, string? selectedDeviceId) where T : BaseModel, new()
    {
        var model = new T()
        {
            UserDevices = (await dbService.ReadItemAsync<Common.Records.User>("users", userPrincipalName, new PartitionKey(userPrincipalName))).Devices,
        };
        model.SelectedDevice = model?.UserDevices?.FirstOrDefault(d => d.Id == selectedDeviceId);
        return model;
    }

    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
