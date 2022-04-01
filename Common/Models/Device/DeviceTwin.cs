namespace Gas.Common.Models.Device;

public class DeviceTwin
{
    public string? DeviceId { get; set; }
    public Properties? Properties { get; set; }
}

public class Properties
{
    public Desired? Desired { get; set; }
    public Reported? Reported { get; set; }
}

public class Desired
{
    public IEnumerable<DeviceSchedule>? Schedules { get; set; }
}

public class Reported
{
    public IEnumerable<DeviceSchedule>? Schedules { get; set; }
}