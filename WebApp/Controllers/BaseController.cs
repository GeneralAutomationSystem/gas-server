using Gas.Common.Extensions;
using Gas.Common.Models.Device;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Gas.WebApp.Controllers;


public class BaseController : Controller
{
    protected readonly ILogger<BaseController> logger;
    protected readonly Container container;

    public BaseController(ILogger<BaseController> logger, IConfiguration config, CosmosClient cosmosClient)
    {
        this.logger = logger;
        container = cosmosClient.GetContainer(config.GetDatabaseId(), config.GetUsersContainerId());
    }

    protected async Task<T?> NewBaseModel<T>(string userPrincipalName, string? selectedDeviceId) where T : BaseModel, new()
    {
        var model = new T()
        {
            UserDevices = (await container.ReadItemAsync<Common.Items.User>(userPrincipalName, new PartitionKey(userPrincipalName))).Resource.Devices,
        };
        model.SelectedDevice = model?.UserDevices?.FirstOrDefault(d => d.Id == selectedDeviceId);
        return model;
    }

    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
