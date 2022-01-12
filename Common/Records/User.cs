using Gas.Services.Cosmos;

namespace Gas.Common.Records;

public class User : Record
{
    public List<Device>? Devices { get; set; }
}