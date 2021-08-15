using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SqlTestDataGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<HostServiceConfig>(hostContext.Configuration);
                    services.AddHostedService<DatabasePopulationServiceHost>();
                })
                .Build();


            await host.RunAsync();

        }
        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args);

    }
    public class HostServiceConfig { }

}
