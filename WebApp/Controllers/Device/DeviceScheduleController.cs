using Gas.Common.Models.Device;
using Gas.Services.Cosmos;
using Gas.Services.Devices;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace Gas.WebApp.Controllers;

[Route("Device/Schedule")]
public class DeviceScheduleController : BaseController
{
    public DeviceScheduleController(ILogger<DeviceScheduleController> logger, ICosmosService dbService, IDeviceService deviceService) : base(logger, dbService, deviceService) { }


    [HttpGet("{id?}")]
    public async Task<IActionResult> IndexAsync(string? id)
    {
        if (id == null)
        {
            return RedirectToAction();
        }
        var model = await NewBaseModel<ScheduleModel>("pepa", id);

        if (model?.UserDevices == null || !model.UserDevices.Select(d => d.Id).Contains(id))
        {
            return RedirectToAction("Index", "DeviceSelect");
        }
        model.Twin = await deviceService.GetTwinAsync(id);


        return View(model);
    }

    [HttpPost("{id?}"), ActionName("Index")]
    public async Task<IActionResult> PostIndexAsync(string? id, ScheduleModel model)
    {
        var schedule = JsonConvert.DeserializeObject<DeviceSchedule>(model.Schedule);
        if (schedule == null)
        {
            return RedirectToAction("Index");
        }
        schedule.Transform();
        await deviceService.UpdateTwinAsync(id, DataType.Tags, schedule);

        return RedirectToAction("Index");
    }
}
