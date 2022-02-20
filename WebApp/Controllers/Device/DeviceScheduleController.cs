using System.Text.Json;
using Gas.Common.Models.Device;
using Gas.Common.Static;
using Gas.Services.Devices;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Gas.WebApp.Controllers;

[Route("Device/{id}/Schedule")]
public class DeviceScheduleController : BaseController
{
    public DeviceScheduleController(ILogger<DeviceScheduleController> logger, IConfiguration config, CosmosClient cosmosClient, IDeviceService deviceService) : base(logger, config, cosmosClient, deviceService) { }


    [HttpGet]
    public async Task<IActionResult> IndexAsync(string id)
    {
        var model = await NewBaseModel<ScheduleModel>("pepa", id);

        if (model?.UserDevices == null || !model.UserDevices.Select(d => d.Id).Contains(id))
        {
            return RedirectToAction("Index", "DeviceSelect");
        }
        model.Twin = await deviceService.GetTwinAsync(id);


        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> IndexAsync(string id, ScheduleModel model)
    {
        if (model.Schedule == null)
        {
            return RedirectToAction("Index");
        }

        var schedule = JsonSerializer.Deserialize<DeviceSchedule>(model.Schedule, Common.Static.JsonOptions.DefaultSerialization);

        if (schedule == null)
        {
            return RedirectToAction("Index");
        }
        schedule.Transform();
        await deviceService.UpdateTwinAsync(id, Services.Devices.DataType.Desired, schedule, "schedule");

        return RedirectToAction("Index");
    }
}
