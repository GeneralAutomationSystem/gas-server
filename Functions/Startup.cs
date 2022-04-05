using System;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
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
        builder.Services.AddSingleton(new CosmosClientBuilder(config.GetValue<string>("CosmosConnectionString"))
                                        .WithSerializerOptions(new CosmosSerializationOptions
                                        {
                                            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                                        })
                                        .WithThrottlingRetryOptions(TimeSpan.FromMinutes(1), 32)
                                        .Build());
    }
}