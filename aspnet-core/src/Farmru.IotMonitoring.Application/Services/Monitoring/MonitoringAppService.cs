using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.Linq.Extensions;
using Abp.UI;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Alerts;
using Farmru.IotMonitoring.Domains.Alerts.Services;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Monitoring;
using Farmru.IotMonitoring.Domains.Nodes;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Services.Monitoring.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Monitoring
{
    [AbpAuthorize(PermissionNames.Pages_Monitoring)]
    public class MonitoringAppService : IotMonitoringAppServiceBase, IMonitoringAppService
    {
        private readonly IRepository<Node, Guid> _nodeRepository;
        private readonly IRepository<Alert, Guid> _alertRepository;
        private readonly IRepository<AlertThresholdConfiguration, Guid> _thresholdRepository;
        private readonly IRepository<Facility, Guid> _facilityRepository;
        private readonly IRepository<MonitoringExecutionHistory, Guid> _historyRepository;
        private readonly ITelemetryAlertEvaluationService _telemetryAlertEvaluationService;

        public MonitoringAppService(
            IRepository<Node, Guid> nodeRepository,
            IRepository<Alert, Guid> alertRepository,
            IRepository<AlertThresholdConfiguration, Guid> thresholdRepository,
            IRepository<Facility, Guid> facilityRepository,
            IRepository<MonitoringExecutionHistory, Guid> historyRepository,
            ITelemetryAlertEvaluationService telemetryAlertEvaluationService)
        {
            _nodeRepository = nodeRepository;
            _alertRepository = alertRepository;
            _thresholdRepository = thresholdRepository;
            _facilityRepository = facilityRepository;
            _historyRepository = historyRepository;
            _telemetryAlertEvaluationService = telemetryAlertEvaluationService;
        }

        public async Task<MonitoringDashboardDto> GetDashboard()
        {
            var tenantId = AbpSession.GetTenantId();
            await _telemetryAlertEvaluationService.RunOfflineMonitoringForTenantAsync(tenantId);

            var config = await GetThresholdEntityAsync(tenantId, null);
            var nodes = await _nodeRepository.GetAll()
                .Where(n => n.IsActive && n.DeviceStatus != DeviceOperationalStatus.Decommissioned)
                .ToListAsync();

            var offlineTimeout = config.OfflineTimeoutMinutes;
            var online = nodes.Count(n => n.IsOnline(offlineTimeout));
            var offline = nodes.Count - online;
            var stale = nodes.Count(n => n.TelemetryQuality == TelemetryQualityStatus.Stale || n.TelemetryQuality == TelemetryQualityStatus.Missing);

            var activeAlerts = await _alertRepository.CountAsync(a => a.IsActive && !a.IsResolved);
            var criticalAlerts = await _alertRepository.CountAsync(a => a.IsActive && !a.IsResolved && a.Severity == AlertSeverity.Critical);
            var escalated = await _alertRepository.CountAsync(a => a.IsActive && !a.IsResolved && a.EscalationLevel > 0);

            var lastExecution = await _historyRepository.GetAll()
                .Where(h => h.TenantId == tenantId && h.JobType == MonitoringJobType.FullCycle)
                .OrderByDescending(h => h.StartedAt)
                .FirstOrDefaultAsync();

            return new MonitoringDashboardDto
            {
                OnlineDevices = online,
                OfflineDevices = offline,
                StaleTelemetryDevices = stale,
                ActiveAlerts = activeAlerts,
                CriticalAlerts = criticalAlerts,
                EscalatedAlerts = escalated,
                MonitoringEnabled = config.MonitoringEnabled,
                LastExecutionAt = lastExecution?.CompletedAt ?? lastExecution?.StartedAt,
                LastExecutionSucceeded = lastExecution?.Succeeded ?? false,
                LastExecution = lastExecution == null ? null : MapHistory(lastExecution)
            };
        }

        public async Task<PagedResultDto<MonitoringExecutionHistoryDto>> GetExecutionHistory(PagedMonitoringHistoryRequestDto input)
        {
            var query = _historyRepository.GetAll().AsQueryable();

            if (input.JobType.HasValue)
            {
                query = query.Where(h => h.JobType == input.JobType.Value);
            }

            if (input.SucceededOnly == true)
            {
                query = query.Where(h => h.Succeeded);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(string.IsNullOrWhiteSpace(input.Sorting) ? "StartedAt desc" : input.Sorting)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<MonitoringExecutionHistoryDto>(
                total,
                items.Select(MapHistory).ToList());
        }

        public async Task<MonitoringConfigurationDto> GetConfiguration(Guid? facilityId = null)
        {
            var config = await GetThresholdEntityAsync(AbpSession.GetTenantId(), facilityId);
            return MapConfig(config);
        }

        [AbpAuthorize(PermissionNames.Pages_Monitoring_Manage)]
        public async Task<MonitoringConfigurationDto> UpdateConfiguration(UpdateMonitoringConfigurationInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            Facility facility = null;
            if (input.FacilityId.HasValue)
            {
                facility = await _facilityRepository.FirstOrDefaultAsync(input.FacilityId.Value);
            }

            var config = await _thresholdRepository.GetAll()
                .Include(t => t.Facility)
                .FirstOrDefaultAsync(t =>
                    t.TenantId == tenantId &&
                    ((input.FacilityId == null && t.FacilityId == null) || t.FacilityId == input.FacilityId));

            try
            {
                if (config == null)
                {
                    config = AlertThresholdConfiguration.CreateDefault(tenantId, facility);
                    ApplyUpdate(config, input);
                    await _thresholdRepository.InsertAsync(config);
                }
                else
                {
                    ApplyUpdate(config, input);
                    await _thresholdRepository.UpdateAsync(config);
                }

                await CurrentUnitOfWork.SaveChangesAsync();
                return MapConfig(config);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private static void ApplyUpdate(AlertThresholdConfiguration config, UpdateMonitoringConfigurationInput input)
        {
            config.Update(
                input.MinimumBatteryPercent,
                input.CriticalBatteryPercent,
                input.MaximumTemperature,
                input.MinimumTemperature,
                input.MinimumMoisturePercent,
                input.OfflineTimeoutMinutes,
                input.AutoResolveWhenNormalized,
                input.StaleTelemetryThresholdMinutes,
                input.EscalationTimeoutMinutes,
                input.AnomalySensitivityPercent,
                input.MonitoringEnabled);
        }

        private async Task<AlertThresholdConfiguration> GetThresholdEntityAsync(int tenantId, Guid? facilityId)
        {
            if (facilityId.HasValue)
            {
                var facilityConfig = await _thresholdRepository.GetAll()
                    .Include(t => t.Facility)
                    .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.FacilityId == facilityId.Value);
                if (facilityConfig != null)
                {
                    return facilityConfig;
                }
            }

            var tenantConfig = await _thresholdRepository.GetAll()
                .Include(t => t.Facility)
                .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.FacilityId == null);

            return tenantConfig ?? AlertThresholdConfiguration.CreateDefault(tenantId);
        }

        private static MonitoringConfigurationDto MapConfig(AlertThresholdConfiguration config) =>
            new MonitoringConfigurationDto
            {
                Id = config.Id,
                Facility = config.Facility != null
                    ? new EntityWithDisplayNameDto<Guid?> { Id = config.Facility.Id, DisplayText = config.Facility.Name }
                    : null,
                MinimumBatteryPercent = config.MinimumBatteryPercent,
                CriticalBatteryPercent = config.CriticalBatteryPercent,
                MaximumTemperature = config.MaximumTemperature,
                MinimumTemperature = config.MinimumTemperature,
                MinimumMoisturePercent = config.MinimumMoisturePercent,
                OfflineTimeoutMinutes = config.OfflineTimeoutMinutes,
                AutoResolveWhenNormalized = config.AutoResolveWhenNormalized,
                StaleTelemetryThresholdMinutes = config.StaleTelemetryThresholdMinutes,
                EscalationTimeoutMinutes = config.EscalationTimeoutMinutes,
                AnomalySensitivityPercent = config.AnomalySensitivityPercent,
                MonitoringEnabled = config.MonitoringEnabled
            };

        private static MonitoringExecutionHistoryDto MapHistory(MonitoringExecutionHistory h) =>
            new MonitoringExecutionHistoryDto
            {
                Id = h.Id,
                TenantId = h.TenantId,
                JobType = h.JobType,
                StartedAt = h.StartedAt,
                CompletedAt = h.CompletedAt,
                DurationMs = h.DurationMs,
                Succeeded = h.Succeeded,
                ErrorMessage = h.ErrorMessage,
                AlertsGenerated = h.AlertsGenerated,
                AlertsResolved = h.AlertsResolved,
                DevicesEvaluated = h.DevicesEvaluated,
                EscalationsPerformed = h.EscalationsPerformed,
                SummaryJson = h.SummaryJson
            };
    }
}
