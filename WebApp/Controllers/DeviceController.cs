using Gas.Services.CosmosDb;
using Gas.Services.Device;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Gas.WebApp.Controllers;

[Route("[controller]")]
public class DeviceController : Controller
{
    private readonly ILogger<DeviceController> logger;
    private readonly ICosmosDbService dbService;
    private readonly IDeviceService deviceService;

    public DeviceController(ILogger<DeviceController> logger, ICosmosDbService dbService, IDeviceService deviceService)
    {
        this.logger = logger;
        this.dbService = dbService;
        this.deviceService = deviceService;
    }

    private async Task<T> NewBaseModel<T>(string userPrincipalName, string? selectedDeviceId) where T : BaseModel, new()
    {
        var model = new T()
        {
            UserDevices = (await dbService.ReadItemAsync<Models.User>("users", userPrincipalName, new PartitionKey(userPrincipalName))).Devices,
        };
        model.SelectedDevice = model.UserDevices.FirstOrDefault(d => d.Id == selectedDeviceId);
        return model;
    }

    [HttpGet("Select")]
    public async Task<IActionResult> SelectAsync()
    {
        var model = await NewBaseModel<BaseModel>("pepa", null);
        return View(model);
    }

    [Route("[action]/{id?}")]
    public async Task<IActionResult> StatusAsync(string? id)
    {
        if (id == null)
        {
            return RedirectToAction("Select");
        }
        var model = await NewBaseModel<DeviceStatusModel>("pepa", id);

        if (!model.UserDevices.Select(d => d.Id).Contains(id))
        {
            return RedirectToAction("Select");
        }

        model.Twin = await deviceService.GetTwinAsync(id);

        return View(model);
    }

    [Route("[action]/{id?}")]
    public async Task<IActionResult> ScheduleAsync(string? id)
    {
        if (id == null)
        {
            return RedirectToAction("Select");
        }
        var model = await NewBaseModel<BaseModel>("pepa", id);

        if (!model.UserDevices.Select(d => d.Id).Contains(id))
        {
            return RedirectToAction("Select");
        }

        return View(model);
    }

    [Route("[action]/{id?}")]
    public async Task<IActionResult> SettingsAsync(string? id)
    {
        if (id == null)
        {
            return RedirectToAction("Select");
        }
        var model = await NewBaseModel<BaseModel>("pepa", id);

        if (!model.UserDevices.Select(d => d.Id).Contains(id))
        {
            return RedirectToAction("Select");
        }

        return View(model);
    }

    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
