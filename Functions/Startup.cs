using Gas.Services.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Gas.Functions.Startup))]
namespace Gas.Functions;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var config = builder.GetContext().Configuration;

        builder.Services.AddSingleton<ICosmosService>(new CosmosService(
            config.GetConnectionString("Cosmos"),
            config.GetValue<string>("DatabaseName"),
            (b) => b.AddContainer(config.GetValue<string>("ReportsContainer"))));
    }
}