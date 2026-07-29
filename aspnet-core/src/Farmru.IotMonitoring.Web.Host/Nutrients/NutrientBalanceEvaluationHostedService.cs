using Abp.Dependency;
using Farmru.IotMonitoring.Services.Nutrients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Web.Host.Nutrients
{
    /// <summary>
    /// Daily nutrient balance evaluation, following the same BackgroundService shape as
    /// OperationalMonitoringHostedService (Phase 1 Technical Design Section 5.2, Sprint0-001).
    /// </summary>
    public class NutrientBalanceEvaluationHostedService : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromHours(24);
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NutrientBalanceEvaluationHostedService> _logger;

        public NutrientBalanceEvaluationHostedService(
            IServiceProvider serviceProvider,
            ILogger<NutrientBalanceEvaluationHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(90), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var resolver = scope.ServiceProvider.GetRequiredService<IIocResolver>();
                        var engine = resolver.Resolve<INutrientBalanceEvaluationEngine>();
                        await engine.RunFullEvaluationCycleAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Nutrient balance evaluation cycle failed.");
                }

                await Task.Delay(Interval, stoppingToken);
            }
        }
    }
}
