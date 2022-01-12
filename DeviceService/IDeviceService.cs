namespace Gas.Services.Devices;

public interface IDeviceService
{
    Task<string?> GetTwinAsync(string deviceId);
    Task UpdateTwinAsync<T>(string deviceId, DataType type, T data, string objName);
}