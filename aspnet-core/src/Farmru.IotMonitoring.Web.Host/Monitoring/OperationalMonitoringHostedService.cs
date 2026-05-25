using Abp.Dependency;
using Abp.Domain.Uow;
using Farmru.IotMonitoring.Monitoring;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Web.Host.Monitoring
{
    public class OperationalMonitoringHostedService : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OperationalMonitoringHostedService> _logger;

        public OperationalMonitoringHostedService(
            IServiceProvider serviceProvider,
            ILogger<OperationalMonitoringHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(45), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var resolver = scope.ServiceProvider.GetRequiredService<IIocResolver>();
                        var uowManager = resolver.Resolve<IUnitOfWorkManager>();
                        using (var uow = uowManager.Begin())
                        {
                            var engine = resolver.Resolve<IOperationalMonitoringEngine>();
                            await engine.RunFullMonitoringCycleAsync();
                            await uow.CompleteAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Operational monitoring background cycle failed.");
                }

                await Task.Delay(Interval, stoppingToken);
            }
        }
    }
}
