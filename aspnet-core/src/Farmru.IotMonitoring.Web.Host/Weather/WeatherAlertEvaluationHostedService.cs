using Abp.Dependency;
using Abp.Domain.Uow;
using Farmru.IotMonitoring.Domains.Weather.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Farmru.IotMonitoring.MultiTenancy;

namespace Farmru.IotMonitoring.Web.Host.Weather
{
    /// <summary>
    /// Evaluates active WeatherAlertRules on a fixed interval, following the same shape
    /// as OperationalMonitoringHostedService (Phase 1 Technical Design Section 5.2,
    /// Sprint0-001). Safe to run without a concrete IWeatherProvider bound: it only reads
    /// WeatherObservation/WeatherForecastDaily rows already persisted, so it is a no-op
    /// (evaluates zero rules' worth of data) until WeatherSyncHostedService exists.
    /// </summary>
    public class WeatherAlertEvaluationHostedService : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(15);
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WeatherAlertEvaluationHostedService> _logger;

        public WeatherAlertEvaluationHostedService(
            IServiceProvider serviceProvider,
            ILogger<WeatherAlertEvaluationHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var resolver = scope.ServiceProvider.GetRequiredService<IIocResolver>();
                        var uowManager = resolver.Resolve<IUnitOfWorkManager>();
                        var tenantRepository = resolver.Resolve<IRepository<Tenant>>();
                        var evaluationService = resolver.Resolve<IWeatherAlertEvaluationService>();

                        var tenantIds = tenantRepository.GetAll().Where(t => t.IsActive).Select(t => t.Id).ToList();
                        foreach (var tenantId in tenantIds)
                        {
                            using (var uow = uowManager.Begin())
                            using (uowManager.Current.SetTenantId(tenantId))
                            {
                                await evaluationService.EvaluateForTenantAsync(tenantId);
                                await uow.CompleteAsync();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Weather alert evaluation cycle failed.");
                }

                await Task.Delay(Interval, stoppingToken);
            }
        }
    }
}
