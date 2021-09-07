using System;
using System.Threading.Tasks;
using Czf.Repository.Contracts;
using Czf.Repository.DatabaseInfo.Smo;
using Czf.Service.Contracts;
using Czf.Service.DatabaseInfo.DataSetBuilder;
using Czf.Service.DataSetOperations.Sql;
using Czf.Service.DependencySortService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static Czf.Repository.DatabaseInfo.Smo.SmoDatabaseInfoRepository;
using static Czf.Service.DataSetOperations.Sql.SqlDataSetSchemaInitializationService;

namespace SqlTestDataGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var config = hostContext.Configuration;
                    services.AddOptions();
                    services.Configure<SmoDatabaseInfoRepositoryOptions>(config.GetSection(SmoDatabaseInfoRepositoryOptions.SmoDatabaseInfoRepositoryOptionsKey));
                    services.Configure<HostServiceConfig>(config);
                    services.Configure<SqlDataSetSchemaInitializationServiceOptions>(config.GetSection(SqlDataSetSchemaInitializationServiceOptions.SqlDataSetSchemaInitializationServiceOptionsKey));
                    services.AddSingleton<IDataSetSchemaInitializationService, SqlDataSetSchemaInitializationService>();
                    services.AddSingleton<IDataAdapterFactory, SqlDataAdapterFactory>();
                    services.AddSingleton<IDatabaseInfoDatasetBuilder, DatabaseInfoDataSetBuilder>();
                    services.AddSingleton<IDatabaseInfoRepository, SmoDatabaseInfoRepository>();
                    services.AddSingleton<IDependencySortService, QuikGraphDependencySortService>();
                    services.AddHostedService<DatabasePopulationServiceHost>();
                })
                .RunConsoleAsync();


            //await host.RunAsync();

        }
        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.Sources.Clear();
                config
                .AddJsonFile("appsettings.json")
                .AddCommandLine(args);
            });

    }
    public class HostServiceConfig 
    {
        public string DatabaseName { get; set; }
    }

}
