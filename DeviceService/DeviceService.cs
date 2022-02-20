using System.Text.Json;
using Gas.Common.Static;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;

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

    public async Task UpdateTwinAsync<T>(string deviceId, DataType type, T data, string objName)
    {
        if (type == DataType.Reported)
        {
            return;
        }


        var patch = new Twin();
        var twinData = new TwinCollection(JsonSerializer.Serialize(data, JsonOptions.DefaultSerialization));

        if (type == DataType.Tags)
        {
            patch.Tags[objName] = twinData;
        }
        else if (type == DataType.Desired)
        {
            patch.Properties.Desired[objName] = twinData;
        }

        var twin = await registryManager.GetTwinAsync(deviceId);
        await registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);
    }
}
