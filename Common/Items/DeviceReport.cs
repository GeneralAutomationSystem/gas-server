namespace Gas.Common.Items;

public class DeviceReport : Item
{
    public string? DeviceId { get; set; }
    public DateTime DateTime { get; set; }
    public DateTime ProcessedDate { get; set; }
    public int Rssi { get; set; }
    public int SystemTemperature0 { get; set; }
    public int SystemTemperature1 { get; set; }
}