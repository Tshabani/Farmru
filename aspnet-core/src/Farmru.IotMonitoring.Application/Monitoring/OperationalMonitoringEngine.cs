using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Farmru.IotMonitoring.Domains.Alerts.Services;
using Farmru.IotMonitoring.Domains.Geo.Services;
using Farmru.IotMonitoring.Domains.Incidents.Services;
using Farmru.IotMonitoring.Domains.Monitoring;
using Farmru.IotMonitoring.MultiTenancy;
using Farmru.IotMonitoring.Services.Monitoring.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Monitoring
{
    public class OperationalMonitoringEngine : IOperationalMonitoringEngine, ITransientDependency
    {
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<MonitoringExecutionHistory, Guid> _historyRepository;
        private readonly ITelemetryAlertEvaluationService _telemetryAlertEvaluationService;
        private readonly IGeoFenceEvaluationService _geoFenceEvaluationService;
        private readonly IOperationalRealtimeNotifier _operationalNotifier;
        private readonly IIncidentSlaEvaluationService _incidentSlaEvaluationService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public OperationalMonitoringEngine(
            IRepository<Tenant> tenantRepository,
            IRepository<MonitoringExecutionHistory, Guid> historyRepository,
            ITelemetryAlertEvaluationService telemetryAlertEvaluationService,
            IGeoFenceEvaluationService geoFenceEvaluationService,
            IOperationalRealtimeNotifier operationalNotifier,
            IIncidentSlaEvaluationService incidentSlaEvaluationService,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _tenantRepository = tenantRepository;
            _historyRepository = historyRepository;
            _telemetryAlertEvaluationService = telemetryAlertEvaluationService;
            _geoFenceEvaluationService = geoFenceEvaluationService;
            _operationalNotifier = operationalNotifier;
            _incidentSlaEvaluationService = incidentSlaEvaluationService;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task RunFullMonitoringCycleAsync()
        {
            var tenantIds = await _tenantRepository.GetAll()
                .Where(t => t.IsActive)
                .Select(t => t.Id)
                .ToListAsync();

            foreach (var tenantId in tenantIds)
            {
                using (var uow = _unitOfWorkManager.Begin())
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await RunTenantCycleAsync(tenantId);
                    await uow.CompleteAsync();
                }
            }
        }

        private async Task RunTenantCycleAsync(int tenantId)
        {
            var history = MonitoringExecutionHistory.Start(tenantId, MonitoringJobType.FullCycle);
            await _historyRepository.InsertAsync(history);

            var aggregate = new MonitoringJobResult();
            try
            {
                var offline = await RunJobAsync(tenantId, MonitoringJobType.DeviceOffline,
                    () => _telemetryAlertEvaluationService.RunOfflineMonitoringForTenantAsync(tenantId));
                aggregate.Add(offline);

                var telemetry = await RunJobAsync(tenantId, MonitoringJobType.TelemetryHealth,
                    () => _telemetryAlertEvaluationService.RunTelemetryHealthMonitoringForTenantAsync(tenantId));
                aggregate.Add(telemetry);

                var escalation = await RunJobAsync(tenantId, MonitoringJobType.AlertEscalation,
                    () => _telemetryAlertEvaluationService.RunAlertEscalationForTenantAsync(tenantId));
                aggregate.Add(escalation);

                await _geoFenceEvaluationService.SyncGeoFencesForTenantAsync(tenantId);

                await RunJobAsync(tenantId, MonitoringJobType.IncidentSla,
                    async () =>
                    {
                        await _incidentSlaEvaluationService.EvaluateTenantIncidentsAsync(tenantId);
                        return new MonitoringEvaluationResult();
                    });

                history.Complete(
                    true,
                    aggregate.AlertsGenerated,
                    aggregate.AlertsResolved,
                    aggregate.DevicesEvaluated,
                    aggregate.EscalationsPerformed,
                    JsonSerializer.Serialize(new { offline, telemetry, escalation }));

                await NotifySummaryAsync(tenantId, aggregate, true);
            }
            catch (Exception ex)
            {
                history.Complete(false, 0, 0, 0, 0, null, ex.Message);
                await NotifySummaryAsync(tenantId, aggregate, false);
            }

            // No explicit UpdateAsync needed — EF change tracking picks up the Complete() call
            // since history is already tracked as Added. SaveChanges handles INSERT + UPDATE.
            await _unitOfWorkManager.Current.SaveChangesAsync();
        }

        private async Task<MonitoringJobResult> RunJobAsync(
            int tenantId,
            MonitoringJobType jobType,
            Func<Task<MonitoringEvaluationResult>> action)
        {
            var jobHistory = MonitoringExecutionHistory.Start(tenantId, jobType);
            await _historyRepository.InsertAsync(jobHistory);

            try
            {
                var result = await action();
                var mapped = Map(result);
                jobHistory.Complete(
                    true,
                    mapped.AlertsGenerated,
                    mapped.AlertsResolved,
                    mapped.DevicesEvaluated,
                    mapped.EscalationsPerformed);
                // EF change tracking detects property changes on tracked entity — no UpdateAsync needed
                return mapped;
            }
            catch (Exception ex)
            {
                jobHistory.Complete(false, 0, 0, 0, 0, null, ex.Message);
                // EF change tracking detects property changes on tracked entity — no UpdateAsync needed
                throw;
            }
        }

        private async Task NotifySummaryAsync(int tenantId, MonitoringJobResult result, bool succeeded)
        {
            if (_operationalNotifier == null)
            {
                return;
            }

            await _operationalNotifier.NotifyExecutionSummaryAsync(tenantId, new MonitoringExecutionSummaryDto
            {
                CompletedAt = DateTime.UtcNow,
                AlertsGenerated = result.AlertsGenerated,
                AlertsResolved = result.AlertsResolved,
                DevicesEvaluated = result.DevicesEvaluated,
                EscalationsPerformed = result.EscalationsPerformed,
                Succeeded = succeeded
            });
        }

        private static MonitoringJobResult Map(MonitoringEvaluationResult result) =>
            new MonitoringJobResult
            {
                AlertsGenerated = result.AlertsGenerated,
                AlertsResolved = result.AlertsResolved,
                DevicesEvaluated = result.DevicesEvaluated,
                EscalationsPerformed = result.EscalationsPerformed
            };
    }
}
