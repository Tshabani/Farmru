using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Domains.Monitoring;
using Farmru.IotMonitoring.Helpers;
using System;
using System.Collections.Generic;

namespace Farmru.IotMonitoring.Services.Monitoring.Dto
{
    public class MonitoringJobResultDto
    {
        public int AlertsGenerated { get; set; }
        public int AlertsResolved { get; set; }
        public int DevicesEvaluated { get; set; }
        public int EscalationsPerformed { get; set; }
    }

    public class MonitoringDashboardDto
    {
        public int OnlineDevices { get; set; }
        public int OfflineDevices { get; set; }
        public int StaleTelemetryDevices { get; set; }
        public int ActiveAlerts { get; set; }
        public int CriticalAlerts { get; set; }
        public int EscalatedAlerts { get; set; }
        public bool MonitoringEnabled { get; set; }
        public DateTime? LastExecutionAt { get; set; }
        public bool LastExecutionSucceeded { get; set; }
        public MonitoringExecutionHistoryDto LastExecution { get; set; }
    }

    public class MonitoringExecutionHistoryDto : EntityDto<Guid>
    {
        public int? TenantId { get; set; }
        public MonitoringJobType JobType { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public long DurationMs { get; set; }
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }
        public int AlertsGenerated { get; set; }
        public int AlertsResolved { get; set; }
        public int DevicesEvaluated { get; set; }
        public int EscalationsPerformed { get; set; }
        public string SummaryJson { get; set; }
    }

    public class PagedMonitoringHistoryRequestDto : PagedAndSortedResultRequestDto
    {
        public MonitoringJobType? JobType { get; set; }
        public bool? SucceededOnly { get; set; }
    }

    public class MonitoringEventDto
    {
        public string EventType { get; set; }
        public string Message { get; set; }
        public DateTime OccurredAt { get; set; }
        public Guid? NodeId { get; set; }
        public string NodeDisplay { get; set; }
        public Guid? AlertId { get; set; }
        public int? Severity { get; set; }
    }

    public class MonitoringExecutionSummaryDto
    {
        public DateTime CompletedAt { get; set; }
        public int AlertsGenerated { get; set; }
        public int AlertsResolved { get; set; }
        public int DevicesEvaluated { get; set; }
        public int EscalationsPerformed { get; set; }
        public bool Succeeded { get; set; }
    }

    public class MonitoringConfigurationDto
    {
        public Guid? Id { get; set; }
        public EntityWithDisplayNameDto<Guid?> Facility { get; set; }
        public decimal MinimumBatteryPercent { get; set; }
        public decimal CriticalBatteryPercent { get; set; }
        public decimal MaximumTemperature { get; set; }
        public decimal MinimumTemperature { get; set; }
        public decimal MinimumMoisturePercent { get; set; }
        public int OfflineTimeoutMinutes { get; set; }
        public bool AutoResolveWhenNormalized { get; set; }
        public int StaleTelemetryThresholdMinutes { get; set; }
        public int EscalationTimeoutMinutes { get; set; }
        public int AnomalySensitivityPercent { get; set; }
        public bool MonitoringEnabled { get; set; }
    }

    public class UpdateMonitoringConfigurationInput
    {
        public Guid? FacilityId { get; set; }
        public decimal MinimumBatteryPercent { get; set; }
        public decimal CriticalBatteryPercent { get; set; }
        public decimal MaximumTemperature { get; set; }
        public decimal MinimumTemperature { get; set; }
        public decimal MinimumMoisturePercent { get; set; }
        public int OfflineTimeoutMinutes { get; set; }
        public bool AutoResolveWhenNormalized { get; set; }
        public int StaleTelemetryThresholdMinutes { get; set; }
        public int EscalationTimeoutMinutes { get; set; }
        public int AnomalySensitivityPercent { get; set; }
        public bool MonitoringEnabled { get; set; }
    }

    public class MonitoringJobResult
    {
        public int AlertsGenerated { get; set; }
        public int AlertsResolved { get; set; }
        public int DevicesEvaluated { get; set; }
        public int EscalationsPerformed { get; set; }

        public void Add(MonitoringJobResult other)
        {
            AlertsGenerated += other.AlertsGenerated;
            AlertsResolved += other.AlertsResolved;
            DevicesEvaluated += other.DevicesEvaluated;
            EscalationsPerformed += other.EscalationsPerformed;
        }
    }
}
