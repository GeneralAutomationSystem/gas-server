using System.Diagnostics;
using Gas.Common.Extensions;
using Gas.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Gas.WebApp.Controllers;

public class HomeController : BaseController
{
    private readonly ILogger<HomeController> logger;
    private readonly Container container;

    public HomeController(ILogger<HomeController> logger, IConfiguration config, CosmosClient cosmosClient) : base(logger, config, cosmosClient)
    {
        this.logger = logger;
        container = cosmosClient.GetContainer(config.GetDatabaseId(), config.GetUsersContainerId());
    }

    public async Task<IActionResult> Index()
    {
        var model = new BaseModel();
        await FillBaseModel(model, null);
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
