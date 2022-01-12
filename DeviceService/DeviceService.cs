using System.Text.Json;
using Gas.Globals;
using Microsoft.Azure.Devices;


namespace Gas.Services.Devices;

public enum DataType
{
    Tags, Desired, Reported
}
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

    public async Task UpdateTwinAsync<T>(string deviceId, DataType type, T data)
    {
        if (type == DataType.Reported)
        {
            return;
        }

        var dataString = JsonSerializer.Serialize(data, Json.Options);
        var typeString = type == DataType.Tags ? "tags" : "desired";

        var twin = await registryManager.GetTwinAsync(deviceId);
        var patch = $"{{ {typeString}: {{ schedule : {dataString} }} }}";

        await registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);
    }
}
