namespace Gas.Services.Device;

public interface IDeviceService
{
    Task<string?> GetTwinAsync(string deviceId);
}