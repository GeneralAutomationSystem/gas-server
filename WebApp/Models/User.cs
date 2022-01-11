using Gas.Services.CosmosDb;

namespace Gas.WebApp.Models;

public class User : Record
{
    public List<Device> Devices { get; set; } = new();
}