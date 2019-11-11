using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using Orleans.Grains;
using Orleans.Hosting;
using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Orleans.Server
{
    class Program
    {
        static Task Main(string[] args)
        {
            Console.Title = typeof(Program).Name;

            return Host.CreateDefaultBuilder()
                .UseOrleans(builder => 
                {
                    builder
                    .UseLocalhostClustering()
                    .AddMemoryGrainStorageAsDefault()
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "Hello.Orleans";
                        options.ServiceId = "Hello.Orleans";
                    })
                    .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                    .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ISessionControlGrain).Assembly).WithReferences());
                })
                .ConfigureServices(services=> 
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .ConfigureLogging(builder=> 
                {
                    builder.AddConsole();
                })
                .RunConsoleAsync();
        }
    }
}
