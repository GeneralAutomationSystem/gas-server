using System.Diagnostics;
using Gas.Services.CosmosDb;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Gas.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> logger;
    private readonly ICosmosDbService dbService;

    public HomeController(ILogger<HomeController> logger, ICosmosDbService dbService)
    {
        this.logger = logger;
        this.dbService = dbService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new BaseModel
        {
            UserDevices = (await dbService.ReadItemAsync<Models.User>("users", "pepa", new PartitionKey("pepa"))).Devices,
        };
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
