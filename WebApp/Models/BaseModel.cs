namespace Gas.WebApp.Models;

public class BaseModel
{
    public Device? SelectedDevice { get; set; }
    public List<Device> UserDevices { get; set; } = new();
}