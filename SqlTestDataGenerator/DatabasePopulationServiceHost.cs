using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Czf.Repository.Contracts;
using Czf.Service.Contracts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace SqlTestDataGenerator
{
    public class DatabasePopulationServiceHost : BackgroundService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IDatabaseInfoRepository _databaseInfoRepository;
        private readonly IDatabaseInfoDatasetBuilder _databaseInfoDatasetBuilder;
        private readonly IDependencySortService _dependencySortService;
        private readonly IOptions<HostServiceConfig> _options;

        public DatabasePopulationServiceHost(IHostApplicationLifetime applicationLifetime,
            IOptions<HostServiceConfig> options,
            IDatabaseInfoRepository databaseInfoRepository,
            IDatabaseInfoDatasetBuilder databaseInfoDatasetBuilder,
            IDependencySortService dependencySortService)
        {
            _applicationLifetime = applicationLifetime;
            _options = options;
            _databaseInfoRepository = databaseInfoRepository;
            _databaseInfoDatasetBuilder = databaseInfoDatasetBuilder;
            _dependencySortService = dependencySortService;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                IDatabaseInfo targetDatabaseInfo =  _databaseInfoRepository.GetDatabaseInfo(_options.Value.DatabaseName);
                DataSet targetDataSet = _databaseInfoDatasetBuilder.CreateDataSet(targetDatabaseInfo, stoppingToken);
                foreach(var t in _dependencySortService.SortDataTables(targetDataSet))
                {
                    Console.WriteLine(t.TableName);
                }
            }
            finally
            {

                _applicationLifetime.StopApplication();
            } 
            return Task.CompletedTask;
        }
    }
}