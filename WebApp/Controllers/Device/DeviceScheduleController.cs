using System.Collections.Specialized;
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

[Route("Device/{deviceId}/Schedule/{scheduleId:int}")]
public class DeviceScheduleController : BaseController
{
    private readonly RegistryManager registryManager;
    public DeviceScheduleController(ILogger<DeviceScheduleController> logger, IConfiguration config, CosmosClient cosmosClient, RegistryManager registryManager) : base(logger, config, cosmosClient)
    {
        this.registryManager = registryManager;
    }


    [HttpGet]
    public async Task<IActionResult> IndexAsync(string deviceId, int scheduleId)
    {
        var model = new ScheduleModel();
        await FillBaseModel(model, deviceId);

        if (model?.UserDevices == null || !model.UserDevices.Select(d => d.Id).Contains(deviceId))
        {
            return RedirectToAction("Index", "DeviceSelect");
        }


        var twin = await registryManager.GetTwinAsync(deviceId);
        if (twin?.Properties?.Desired == null)
        {
            throw new ArgumentNullException(nameof(twin), "Can not get device twin.");
        }

        var desired = JsonSerializer.Deserialize<Desired>(twin.Properties.Desired.ToJson(), Common.Static.JsonOptions.DefaultSerialization);

        var schedule = desired?.Schedules?.ElementAtOrDefault(scheduleId);
        if (schedule == null)
        {
            throw new ArgumentNullException(nameof(schedule), "Can not get schedule from device twin.");
        }

        model.Period = schedule.Period;
        model.Intervals = new Dictionary<string, Models.Interval>();
        var i = 0;
        foreach (var interval in schedule.Intervals)
        {
            model.Intervals[i.ToString()] = new Models.Interval(interval.Start, interval.End);
            i++;
        }
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> IndexAsync(string deviceId, int scheduleId, ScheduleModel model)
    {
        await FillBaseModel(model, deviceId);
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var twin = await registryManager.GetTwinAsync(deviceId);
        if (twin?.Properties?.Desired == null)
        {
            throw new ArgumentNullException(nameof(twin), "Can not get device twin.");
        }
        var desired = JsonSerializer.Deserialize<Desired>(twin.Properties.Desired.ToJson(), Common.Static.JsonOptions.DefaultSerialization);
        var schedules = desired?.Schedules?.ToArray();
        if (schedules == null || schedules[scheduleId] == null)
        {
            throw new ArgumentNullException(nameof(schedules), "Can not get schedule from device twin.");
        }

        var schedule = new DeviceSchedule
        {
            PinNumber = schedules[scheduleId].PinNumber,
            Period = model.Period,
        };

        foreach (var interval in model.Intervals.Values)
        {
            schedule.Intervals.Add(new Common.Models.Device.Interval
            {
                Start = interval.StartInSeconds,
                End = interval.EndInSeconds,
            });
        }
        schedule.Transform();

        schedules[scheduleId] = schedule;

        var json = JsonSerializer.Serialize(schedules, Common.Static.JsonOptions.DefaultSerialization);
        var twinPatch = @"{ properties : { desired : { schedules : " + json + "}}}";

        await registryManager.UpdateTwinAsync(deviceId, twinPatch, twin.ETag);
        return RedirectToAction("Index");
    }
}
