namespace Gas.Common.Items;

public class DeviceReport : Item
{
    public string? DeviceId { get; set; }
    public DateTime ProcessedDate { get; set; }
    public dynamic? Data { get; set; }
}