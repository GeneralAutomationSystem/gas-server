using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using Gas.Common.Models.Device;
using Gas.Common.Static;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Devices;

namespace Gas.WebApp.Controllers;

[Route("Device/{id}/Schedule")]
public class DeviceScheduleController : BaseController
{
    private readonly RegistryManager registryManager;
    public DeviceScheduleController(ILogger<DeviceScheduleController> logger, IConfiguration config, CosmosClient cosmosClient, RegistryManager registryManager) : base(logger, config, cosmosClient)
    {
        this.registryManager = registryManager;
    }


    [HttpGet]
    public async Task<IActionResult> IndexAsync(string id)
    {
        var model = await NewBaseModel<ScheduleModel>("pepa", id);

        if (model?.UserDevices == null || !model.UserDevices.Select(d => d.Id).Contains(id))
        {
            return RedirectToAction("Index", "DeviceSelect");
        }


        var twin = await registryManager.GetTwinAsync(id);
        var desired = JsonSerializer.Deserialize<Desired>(twin?.Properties?.Desired.ToJson(), Common.Static.JsonOptions.DefaultSerialization);

        var schedule = desired?.Schedules?.ElementAt(0);

        model.Period = schedule?.Period;
        model.Intervals = new Dictionary<string, Models.Interval>();
        var i = 0;
        foreach (var interval in schedule.Intervals)
        {
            model.Intervals[i.ToString()] = new Models.Interval
            {
                StartDay = interval.Start / 86400,
                StartTime = TimeSpan.FromSeconds(interval.Start).ToString(@"hh\:mm\:ss"),
                EndDay = interval.End / 86400,
                EndTime = TimeSpan.FromSeconds(interval.End).ToString(@"hh\:mm\:ss"),
            };
            i++;
        }
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> IndexAsync(string id, ScheduleModel model)
    {
        if (!model.Periods.Select(i => i.Value).Contains(model.Period.ToString()))
        {
            return RedirectToAction("Index");
        }

        if (model.Intervals == null)
        {
            model.Intervals = new();
        }

        var twin = await registryManager.GetTwinAsync(id);
        var desired = JsonSerializer.Deserialize<Desired>(twin?.Properties?.Desired.ToJson(), Common.Static.JsonOptions.DefaultSerialization);
        var schedules = desired?.Schedules?.ToArray();
        var schedule = new DeviceSchedule();

        schedule.Period = model.Period.Value;
        foreach (var (key, value) in model.Intervals)
        {
            schedule.Intervals.Add(new Common.Models.Device.Interval
            {
                Start = value.StartDay.Value * 86400 + (int)TimeSpan.Parse(value.StartTime).TotalSeconds,
                End = value.EndDay.Value * 86400 + (int)TimeSpan.Parse(value.EndTime).TotalSeconds,
            });
        }
        schedule.Transform();

        schedules[0] = schedule;

        var json = JsonSerializer.Serialize(schedules, Common.Static.JsonOptions.DefaultSerialization);
        var twinPatch = @"{ properties : { desired : { schedules : " + json + "}}}";

        await registryManager.UpdateTwinAsync(id, twinPatch, twin.ETag);
        return RedirectToAction("Index");
    }
}
