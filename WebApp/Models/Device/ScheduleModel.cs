using System;
using System.ComponentModel.DataAnnotations;
using Gas.Common.Models.Device;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gas.WebApp.Models;

public class ScheduleModel : BaseModel, IValidatableObject
{
    public int Period { get; set; } = 60;
    public List<Interval> Intervals { get; set; } = new();
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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Periods.Select(i => i.Value).Contains(Period.ToString()))
        {
            yield return new ValidationResult("Period has to be 1 minute or 1 hour or 1 day or 1 week");
        }

        if (Intervals.Any(i => Period <= i.StartInSeconds))
        {
            yield return new ValidationResult("Start of interval can not be higher than Period.");
        }

        if (Intervals.Any(i => Period <= i.EndInSeconds))
        {
            yield return new ValidationResult("End of interval can not be higher than Period.");
        }

        if (Intervals.Any(i => i.StartInSeconds < 0))
        {
            yield return new ValidationResult("Start of interval can not be lower than Period.");
        }

        if (Intervals.Any(i => i.EndInSeconds < 0))
        {
            yield return new ValidationResult("End of interval can not be lower than Period.");
        }
    }
}

public class Interval
{
    public int StartDay { get; set; }
    public string StartTime { get; set; } = "00:00:00";
    public int EndDay { get; set; }
    public string EndTime { get; set; } = "00:00:00";
    public Interval() { }
    public Interval(int startSeconds, int endSeconds)
    {
        StartDay = DayFromSeconds(startSeconds);
        StartTime = TimeFromSeconds(startSeconds);

        EndDay = DayFromSeconds(endSeconds);
        EndTime = TimeFromSeconds(endSeconds);
    }

    public int StartInSeconds { get => InSeconds(StartDay, StartTime); }
    public int EndInSeconds { get => InSeconds(EndDay, EndTime); }


    private int DayFromSeconds(int value) => value / 86400;
    private string TimeFromSeconds(int value) => TimeSpan.FromSeconds(value).ToString(@"hh\:mm\:ss");

    private int InSeconds(int day, string time) => day * 86400 + (int)TimeSpan.Parse(time).TotalSeconds;
}