using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Farmru.IotMonitoring.Alerts;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Alerts;
using Farmru.IotMonitoring.Domains.Alerts.Services;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Services.Alerts.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Alerts
{
    [AbpAuthorize(PermissionNames.Pages_Alerts)]
    public class AlertAppService : IotMonitoringAppServiceBase, IAlertAppService
    {
        private readonly IRepository<Alert, Guid> _alertRepository;
        private readonly IRepository<AlertThresholdConfiguration, Guid> _thresholdRepository;
        private readonly IRepository<Facility, Guid> _facilityRepository;
        private readonly ITelemetryAlertEvaluationService _telemetryAlertEvaluationService;
        private readonly IAlertRealtimeNotifier _realtimeNotifier;

        public AlertAppService(
            IRepository<Alert, Guid> alertRepository,
            IRepository<AlertThresholdConfiguration, Guid> thresholdRepository,
            IRepository<Facility, Guid> facilityRepository,
            ITelemetryAlertEvaluationService telemetryAlertEvaluationService,
            IAlertRealtimeNotifier realtimeNotifier)
        {
            _alertRepository = alertRepository;
            _thresholdRepository = thresholdRepository;
            _facilityRepository = facilityRepository;
            _telemetryAlertEvaluationService = telemetryAlertEvaluationService;
            _realtimeNotifier = realtimeNotifier;
        }

        public async Task<PagedResultDto<AlertDto>> GetAlerts(PagedAlertResultRequestDto input)
        {
            await _telemetryAlertEvaluationService.RunOfflineMonitoringForTenantAsync(AbpSession.GetTenantId());

            var query = BuildFilteredQuery(input);
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(string.IsNullOrWhiteSpace(input.Sorting) ? "LastTriggeredAt desc" : input.Sorting)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<AlertDto>(totalCount, items.Select(MapToDto).ToList());
        }

        public async Task<List<AlertDto>> GetActiveAlerts()
        {
            await _telemetryAlertEvaluationService.RunOfflineMonitoringForTenantAsync(AbpSession.GetTenantId());

            var alerts = await _alertRepository.GetAll()
                .Include(a => a.Node)
                .Include(a => a.Facility)
                .Where(a => a.IsActive && !a.IsResolved)
                .OrderByDescending(a => a.Severity)
                .ThenByDescending(a => a.LastTriggeredAt)
                .Take(100)
                .ToListAsync();

            return alerts.Select(MapToDto).ToList();
        }

        public async Task<List<AlertDto>> GetCriticalAlerts()
        {
            var alerts = await _alertRepository.GetAll()
                .Include(a => a.Node)
                .Include(a => a.Facility)
                .Where(a => a.IsActive && !a.IsResolved && a.Severity == AlertSeverity.Critical)
                .OrderByDescending(a => a.LastTriggeredAt)
                .Take(50)
                .ToListAsync();

            return alerts.Select(MapToDto).ToList();
        }

        public async Task<AlertDto> GetAlert(EntityDto<Guid> input)
        {
            var alert = await GetAlertOrThrowAsync(input.Id);
            return MapToDto(alert);
        }

        public async Task<List<AlertDto>> GetAlertsByNode(EntityDto<Guid> input)
        {
            var alerts = await _alertRepository.GetAll()
                .Include(a => a.Node)
                .Include(a => a.Facility)
                .Where(a => a.NodeId == input.Id)
                .OrderByDescending(a => a.TriggeredAt)
                .Take(50)
                .ToListAsync();

            return alerts.Select(MapToDto).ToList();
        }

        [AbpAuthorize(PermissionNames.Pages_Alerts_Manage)]
        public async Task<AlertDto> Acknowledge(AcknowledgeAlertInput input)
        {
            var alert = await GetAlertOrThrowAsync(input.AlertId);
            try
            {
                alert.Acknowledge(AbpSession.GetUserId());
                await _alertRepository.UpdateAsync(alert);
                await CurrentUnitOfWork.SaveChangesAsync();
                var dto = MapToDto(alert);
                await _realtimeNotifier.NotifyAsync(dto, "acknowledged");
                return dto;
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Pages_Alerts_Manage)]
        public async Task<AlertDto> Resolve(ResolveAlertInput input)
        {
            var alert = await GetAlertOrThrowAsync(input.AlertId);
            try
            {
                alert.Resolve(AbpSession.GetUserId(), input.ResolutionNotes);
                await _alertRepository.UpdateAsync(alert);
                await CurrentUnitOfWork.SaveChangesAsync();
                var dto = MapToDto(alert);
                await _realtimeNotifier.NotifyAsync(dto, "resolved");
                return dto;
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<AlertStatisticsDto> GetStatistics()
        {
            await _telemetryAlertEvaluationService.RunOfflineMonitoringForTenantAsync(AbpSession.GetTenantId());

            var active = await _alertRepository.GetAll()
                .Include(a => a.Facility)
                .Where(a => a.IsActive && !a.IsResolved)
                .ToListAsync();

            var today = DateTime.UtcNow.Date;
            var resolvedToday = await _alertRepository.CountAsync(a => a.IsResolved && a.ResolvedAt >= today);

            return new AlertStatisticsDto
            {
                TotalActive = active.Count,
                TotalCritical = active.Count(a => a.Severity == AlertSeverity.Critical),
                TotalWarning = active.Count(a => a.Severity == AlertSeverity.Warning),
                TotalInfo = active.Count(a => a.Severity == AlertSeverity.Info),
                TotalUnacknowledged = active.Count(a => !a.IsAcknowledged),
                TotalResolvedToday = resolvedToday,
                BySeverity = active.GroupBy(a => (int)a.Severity)
                    .Select(g => new AlertCountBySeverityDto { Severity = g.Key, Count = g.Count() }).ToList(),
                ByType = active.GroupBy(a => (int)a.AlertType)
                    .Select(g => new AlertCountByTypeDto { AlertType = g.Key, Count = g.Count() }).ToList(),
                ByFacility = active
                    .GroupBy(a => a.Facility?.Name ?? "Unassigned")
                    .Select(g => new AlertFacilitySummaryDto
                    {
                        FacilityName = g.Key,
                        ActiveCount = g.Count(),
                        CriticalCount = g.Count(x => x.Severity == AlertSeverity.Critical)
                    })
                    .OrderByDescending(x => x.CriticalCount)
                    .Take(10)
                    .ToList()
            };
        }

        public async Task<AlertThresholdConfigurationDto> GetThresholdConfiguration(Guid? facilityId = null)
        {
            var tenantId = AbpSession.GetTenantId();
            var config = await _thresholdRepository.GetAll()
                .Include(t => t.Facility)
                .FirstOrDefaultAsync(t =>
                    t.TenantId == tenantId &&
                    ((facilityId == null && t.FacilityId == null) || t.FacilityId == facilityId));

            if (config == null)
            {
                config = AlertThresholdConfiguration.CreateDefault(tenantId);
            }

            return MapThresholdToDto(config);
        }

        [AbpAuthorize(PermissionNames.Pages_Alerts_Manage)]
        public async Task<AlertThresholdConfigurationDto> UpdateThresholdConfiguration(UpdateAlertThresholdConfigurationInput input)
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
                    await _thresholdRepository.InsertAsync(config);
                }
                else
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
                    await _thresholdRepository.UpdateAsync(config);
                }

                await CurrentUnitOfWork.SaveChangesAsync();
                return MapThresholdToDto(config);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private IQueryable<Alert> BuildFilteredQuery(PagedAlertResultRequestDto input)
        {
            var query = _alertRepository.GetAll()
                .Include(a => a.Node)
                .Include(a => a.Facility)
                .AsQueryable();

            if (input.Severity.HasValue)
            {
                query = query.Where(a => a.Severity == input.Severity.Value);
            }

            if (input.AlertType.HasValue)
            {
                query = query.Where(a => a.AlertType == input.AlertType.Value);
            }

            if (input.FacilityId.HasValue)
            {
                query = query.Where(a => a.FacilityId == input.FacilityId.Value);
            }

            if (input.NodeId.HasValue)
            {
                query = query.Where(a => a.NodeId == input.NodeId.Value);
            }

            if (input.ActiveOnly == true)
            {
                query = query.Where(a => a.IsActive && !a.IsResolved);
            }

            if (input.CriticalOnly == true)
            {
                query = query.Where(a => a.Severity == AlertSeverity.Critical);
            }

            if (input.UnacknowledgedOnly == true)
            {
                query = query.Where(a => !a.IsAcknowledged && !a.IsResolved);
            }

            if (input.FromDate.HasValue)
            {
                query = query.Where(a => a.TriggeredAt >= input.FromDate.Value);
            }

            if (input.ToDate.HasValue)
            {
                query = query.Where(a => a.TriggeredAt <= input.ToDate.Value);
            }

            return query;
        }

        private async Task<Alert> GetAlertOrThrowAsync(Guid id)
        {
            var alert = await _alertRepository.GetAll()
                .Include(a => a.Node)
                .Include(a => a.Facility)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (alert == null)
            {
                throw new UserFriendlyException(L("AlertNotFound"));
            }

            return alert;
        }

        private AlertDto MapToDto(Alert alert)
        {
            return new AlertDto
            {
                Id = alert.Id,
                TenantId = alert.TenantId,
                NodeId = alert.NodeId,
                FacilityId = alert.FacilityId,
                Node = alert.Node != null
                    ? new EntityWithDisplayNameDto<Guid?> { Id = alert.Node.Id, DisplayText = alert.Node.SerialNumber }
                    : null,
                Facility = alert.Facility != null
                    ? new EntityWithDisplayNameDto<Guid?> { Id = alert.Facility.Id, DisplayText = alert.Facility.Name }
                    : null,
                AlertType = alert.AlertType,
                Severity = alert.Severity,
                Title = alert.Title,
                Description = alert.Description,
                IsAcknowledged = alert.IsAcknowledged,
                AcknowledgedByUserId = alert.AcknowledgedByUserId,
                AcknowledgedAt = alert.AcknowledgedAt,
                IsResolved = alert.IsResolved,
                ResolvedByUserId = alert.ResolvedByUserId,
                ResolvedAt = alert.ResolvedAt,
                ResolutionNotes = alert.ResolutionNotes,
                TriggeredAt = alert.TriggeredAt,
                LastTriggeredAt = alert.LastTriggeredAt,
                IsActive = alert.IsActive,
                MetadataJson = alert.MetadataJson,
                SourceTelemetryId = alert.SourceTelemetryId
            };
        }

        private static AlertThresholdConfigurationDto MapThresholdToDto(AlertThresholdConfiguration config)
        {
            return new AlertThresholdConfigurationDto
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
        }
    }
}
