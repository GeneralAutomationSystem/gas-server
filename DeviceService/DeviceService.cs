using Microsoft.Azure.Devices;

namespace Gas.Services.Device;
public class DeviceService : IDeviceService
{
    private readonly RegistryManager registryManager;
    public DeviceService(string connectionString)
    {
        registryManager = RegistryManager.CreateFromConnectionString(connectionString);
    }

    public async Task<string?> GetTwinAsync(string deviceId)
    {
        var twin = await registryManager.GetTwinAsync(deviceId);
        return twin?.ToJson();
    }
}
