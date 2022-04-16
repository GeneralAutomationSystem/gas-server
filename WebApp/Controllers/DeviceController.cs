using System.Text.Json;
using Adamijak.Azure.Cosmos.Extensions;
using Gas.Common.Extensions;
using Gas.Common.Items;
using Gas.Common.Models.Device;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Devices;

namespace Gas.WebApp.Controllers;

[Route("[controller]")]
public class DeviceController : BaseController
{
    private readonly RegistryManager registryManager;
    private readonly Container reportContainer;

    public DeviceController(ILogger<DeviceController> logger, IConfiguration config, CosmosClient cosmosClient, RegistryManager registryManager) : base(logger, config, cosmosClient)
    {
        this.registryManager = registryManager;
        reportContainer = cosmosClient.GetContainer(config.GetDatabaseId(), config.GetReportContainerId());
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> SelectAsync()
    {
        var model = new BaseModel();
        await FillBaseModel(model, null);
        return View(model);
    }

    [HttpGet("{deviceId}/[action]")]
    public async Task<IActionResult> StatusAsync(string deviceId)
    {
        var model = new StatusModel();
        await FillBaseModel(model, deviceId);

        if (model?.UserDevices == null || !model.UserDevices.Select(d => d.Id).Contains(deviceId))
        {
            return RedirectToAction("Index", "DeviceSelect");
        }

        var query = new QueryDefinition("SELECT * FROM c where c.deviceId = @deviceId and c.dateTime > @date")
            .WithParameter("@deviceId", deviceId)
            .WithParameter("@date", DateTime.UtcNow.AddDays(-7));

        var reports = await reportContainer.GetItemsAsync<DeviceReport>(query);

        model.SystemTemperatures = reports.Select(r => (r.DateTime, (r.SystemTemperature0 + r.SystemTemperature1) / 2)).ToList();
        model.Rssis = reports.Select(r => (r.DateTime, r.Rssi)).ToList();

        return View(model);
    }


    [HttpGet("{deviceId}/[action]/{scheduleId:int}")]
    public async Task<IActionResult> ScheduleAsync(string deviceId, int scheduleId)
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
        model.Intervals = schedule.Intervals.Select(i => new Models.Interval(i.Start % model.Period, i.End % model.Period)).ToList();

        return View(model);
    }


    [HttpPost("{deviceId}/[action]/{scheduleId:int}")]
    public async Task<IActionResult> ScheduleAsync(string deviceId, int scheduleId, ScheduleModel model)
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

        schedule.Intervals = model.Intervals.Select(i => new Common.Models.Device.Interval
        {
            Start = i.StartInSeconds,
            End = i.EndInSeconds,
        }).ToList();

        schedule.Transform();

        schedules[scheduleId] = schedule;

        var json = JsonSerializer.Serialize(schedules, Common.Static.JsonOptions.DefaultSerialization);
        var twinPatch = @"{ properties : { desired : { schedules : " + json + "}}}";

        await registryManager.UpdateTwinAsync(deviceId, twinPatch, twin.ETag);
        return RedirectToAction("Schedule");
    }
}
