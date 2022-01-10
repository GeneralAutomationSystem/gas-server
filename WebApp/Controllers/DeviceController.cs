using System.Diagnostics;
using Gas.CosmosDb;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Gas.WebApp.Controllers;

[Route("[controller]")]
public class DeviceController : Controller
{
    private readonly ILogger<DeviceController> logger;
    private readonly ICosmosDbService dbService;


    public DeviceController(ILogger<DeviceController> logger, ICosmosDbService dbService)
    {
        this.logger = logger;
        this.dbService = dbService;
    }

    private async Task<BaseModel> NewBaseModel(string userPrincipalName, string? selectedDeviceId)
    {
        var model = new BaseModel
        {
            UserDevices = (await dbService.ReadItemAsync<Models.User>("users", userPrincipalName, new PartitionKey(userPrincipalName))).Devices,
        };
        model.SelectedDevice = model.UserDevices.FirstOrDefault(d => d.Id == selectedDeviceId);
        return model;
    }

    [HttpGet("Select")]
    public async Task<IActionResult> SelectAsync()
    {
        var model = await NewBaseModel("pepa", null);
        return View(model);
    }

    [Route("[action]/{id?}")]
    public async Task<IActionResult> StatusAsync(string? id)
    {
        if (id == null)
        {
            return RedirectToAction("Select");
        }
        var model = await NewBaseModel("pepa", id);

        if (!model.UserDevices.Select(d => d.Id).Contains(id))
        {
            return RedirectToAction("Select");
        }

        return View(model);
    }

    [Route("[action]/{id?}")]
    public async Task<IActionResult> ScheduleAsync(string? id)
    {
        if (id == null)
        {
            return RedirectToAction("Select");
        }
        var model = await NewBaseModel("pepa", id);

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
        var model = await NewBaseModel("pepa", id);

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
