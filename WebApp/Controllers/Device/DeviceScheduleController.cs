using System.Text.Json;
using Gas.Common.Models.Device;
using Gas.Services.Cosmos;
using Gas.Services.Devices;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gas.WebApp.Controllers;

[Route("Device/{id}/Schedule")]
public class DeviceScheduleController : BaseController
{
    public DeviceScheduleController(ILogger<DeviceScheduleController> logger, ICosmosService dbService, IDeviceService deviceService) : base(logger, dbService, deviceService) { }


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

        var schedule = JsonSerializer.Deserialize<DeviceSchedule>(model.Schedule, Globals.Json.Options);
        if (schedule == null)
        {
            return RedirectToAction("Index");
        }
        schedule.Transform();
        await deviceService.UpdateTwinAsync(id, DataType.Tags, schedule);

        return RedirectToAction("Index");
    }
}
