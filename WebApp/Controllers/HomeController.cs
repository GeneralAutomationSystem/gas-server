using System.Diagnostics;
using Gas.Common.Extensions;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Gas.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> logger;
    private readonly Container container;

    public HomeController(ILogger<HomeController> logger, IConfiguration config, CosmosClient cosmosClient)
    {
        this.logger = logger;
        container = cosmosClient.GetContainer(config.GetDatabaseId(), config.GetUsersContainerId());
    }

    public async Task<IActionResult> Index()
    {
        var model = new BaseModel
        {
            UserDevices = (await container.ReadItemAsync<Common.Items.User>("pepa", new PartitionKey("pepa"))).Resource.Devices,
        };
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
