using Gas.Common.Items;

namespace Gas.WebApp.Models;

public class BaseModel
{
    public string? UserPrincipalName { get; set; }
    public Device? SelectedDevice { get; set; }
    public List<Device> UserDevices { get; set; } = new();
}