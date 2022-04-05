using System.Dynamic;

namespace Gas.Common.Items;

public class Device : Item
{
    public string? Name { get; set; }
    public int SchedulesCount { get; set; }
}