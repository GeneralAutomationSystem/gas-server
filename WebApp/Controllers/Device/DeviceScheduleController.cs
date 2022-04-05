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
        var model = await NewBaseModel<ScheduleModel>("pepa", deviceId);

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
    public async Task<IActionResult> IndexAsync(string deviceId, int scheduleId, ScheduleModel model)
    {
        if (!model.Periods.Select(i => i.Value).Contains(model.Period.ToString()))
        {
            return RedirectToAction("Index");
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
            if (interval.StartDay == null || interval.StartTime == null || interval.EndDay == null || interval.EndTime == null)
            {
                continue;
            }
            schedule.Intervals.Add(new Common.Models.Device.Interval
            {
                Start = interval.StartDay.Value * 86400 + (int)TimeSpan.Parse(interval.StartTime).TotalSeconds,
                End = interval.EndDay.Value * 86400 + (int)TimeSpan.Parse(interval.EndTime).TotalSeconds,
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
