using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace SqlTestDataGenerator
{
    public class DatabasePopulationServiceHost : BackgroundService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;

        public DatabasePopulationServiceHost(IHostApplicationLifetime applicationLifetime)
        {
            _applicationLifetime = applicationLifetime;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {

            }
            finally
            {

                _applicationLifetime.StopApplication();
            } 
            return Task.CompletedTask;
        }
    }
}