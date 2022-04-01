using System;
using Gas.Common.Models.Device;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gas.WebApp.Models;

public class ScheduleModel : BaseModel
{
    public int? Period { get; set; }
    public Dictionary<string, Interval>? Intervals { get; set; }
    public readonly List<SelectListItem> DaysInWeek = new()
    {
        new SelectListItem("Monday", "0"),
        new SelectListItem("Tuesday", "1"),
        new SelectListItem("Wednesday", "2"),
        new SelectListItem("Thursday", "3"),
        new SelectListItem("Friday", "4"),
        new SelectListItem("Saturday", "5"),
        new SelectListItem("Sunday", "6"),
    };

    public readonly List<SelectListItem> Periods = new()
    {
        new SelectListItem("1 minute", "60"),
        new SelectListItem("1 hour", "3600"),
        new SelectListItem("1 day", "86400"),
        new SelectListItem("1 week", "604800"),
    };
}

public class Interval
{
    public int? StartDay { get; set; }
    public string? StartTime { get; set; }
    public int? EndDay { get; set; }
    public string? EndTime { get; set; }
}