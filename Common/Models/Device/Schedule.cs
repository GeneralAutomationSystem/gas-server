namespace Gas.Common.Models.Device;

public class DeviceSchedule
{
    public int PinNumber { get; set; }
    public List<Interval> Intervals { get; set; } = new();
    public int Period { get; set; }

    public void Transform()
    {
        if (Intervals.Count == 0)
        {
            return;
        }

        var tempIntervals = new List<Interval>();
        foreach (var interval in Intervals)
        {
            interval.Trim(0, Period);

            if (interval.Size > 0)
            {
                tempIntervals.Add(interval);
            }
            else if (interval.Size < 0)
            {
                var intervalToAdd = interval.ZeroEnd();
                if (intervalToAdd.Size > 0)
                {
                    tempIntervals.Add(intervalToAdd);
                }

                intervalToAdd = interval.StartMax(Period);
                if (intervalToAdd.Size > 0)
                {
                    tempIntervals.Add(intervalToAdd);
                }
            }
            else
            {
                // Skip zerosized intervals
            }
        }

        if (tempIntervals.Count == 0)
        {
            Intervals = new();
            return;
        }

        tempIntervals.Sort((a, b) => a.Start - b.Start);


        Intervals = tempIntervals.Take(1).ToList();

        foreach (var mergingInterval in tempIntervals.Skip(1))
        {
            var last = Intervals.Last();
            if (mergingInterval.Start <= last.End)
            {
                last.End = Math.Max(last.End, mergingInterval.End);
            }
            else
            {
                Intervals.Add(mergingInterval);
            }
        }
    }

}
