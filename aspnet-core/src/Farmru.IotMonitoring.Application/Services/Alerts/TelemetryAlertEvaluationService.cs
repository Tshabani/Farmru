using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Farmru.IotMonitoring.Alerts;
using Farmru.IotMonitoring.Domains.Alerts;
using Farmru.IotMonitoring.Domains.Alerts.Services;
using Farmru.IotMonitoring.Domains.Nodes;
using Farmru.IotMonitoring.Services.Alerts.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NodeDataEntity = Farmru.IotMonitoring.Domains.Nodes.NodeData;

namespace Farmru.IotMonitoring.Services.Alerts
{
    public class TelemetryAlertEvaluationService : DomainService, ITelemetryAlertEvaluationService
    {
        private readonly IRepository<Alert, Guid> _alertRepository;
        private readonly IRepository<AlertThresholdConfiguration, Guid> _thresholdRepository;
        private readonly IRepository<Node, Guid> _nodeRepository;
        private readonly IRepository<NodeDataEntity, Guid> _nodeDataRepository;
        private readonly IAlertRealtimeNotifier _realtimeNotifier;
        private MonitoringEvaluationResult _jobResult;

        public TelemetryAlertEvaluationService(
            IRepository<Alert, Guid> alertRepository,
            IRepository<AlertThresholdConfiguration, Guid> thresholdRepository,
            IRepository<Node, Guid> nodeRepository,
            IRepository<NodeDataEntity, Guid> nodeDataRepository,
            IAlertRealtimeNotifier realtimeNotifier)
        {
            _alertRepository = alertRepository;
            _thresholdRepository = thresholdRepository;
            _nodeRepository = nodeRepository;
            _nodeDataRepository = nodeDataRepository;
            _realtimeNotifier = realtimeNotifier;
        }

        public async Task EvaluateAfterTelemetryAsync(Node node, NodeDataEntity nodeData)
        {
            if (node == null)
            {
                return;
            }

            _jobResult = null;
            var thresholds = await GetThresholdsAsync(node.TenantId, node.Facility?.Id);
            if (!thresholds.MonitoringEnabled)
            {
                return;
            }

            var facilityId = node.Facility?.Id;
            var telemetryId = nodeData?.Id;
            var quality = EvaluateTelemetryQuality(node, nodeData, thresholds);
            node.ApplyOperationalEvaluation(thresholds.OfflineTimeoutMinutes, quality);
            await _nodeRepository.UpdateAsync(node);

            await ResolveIfNormalizedAsync(node, AlertType.DeviceOffline, thresholds.AutoResolveWhenNormalized);

            if (node.BatteryLevel.HasValue)
            {
                if (node.BatteryLevel <= thresholds.CriticalBatteryPercent)
                {
                    await UpsertAlertAsync(node, facilityId, AlertType.LowBattery, AlertSeverity.Critical,
                        "Critical battery level",
                        $"Device {node.SerialNumber} battery is at {node.BatteryLevel}% (critical threshold {thresholds.CriticalBatteryPercent}%).",
                        telemetryId);
                }
                else if (node.BatteryLevel <= thresholds.MinimumBatteryPercent)
                {
                    await UpsertAlertAsync(node, facilityId, AlertType.LowBattery, AlertSeverity.Warning,
                        "Low battery",
                        $"Device {node.SerialNumber} battery is at {node.BatteryLevel}% (warning threshold {thresholds.MinimumBatteryPercent}%).",
                        telemetryId);
                }
                else
                {
                    await ResolveIfNormalizedAsync(node, AlertType.LowBattery, thresholds.AutoResolveWhenNormalized);
                }
            }

            if (TryParseDecimal(nodeData?.Moisture, out var moisture))
            {
                if (moisture < thresholds.MinimumMoisturePercent)
                {
                    await UpsertAlertAsync(node, facilityId, AlertType.SoilMoistureLow, AlertSeverity.Warning,
                        "Low soil moisture",
                        $"Moisture reading {moisture}% is below minimum {thresholds.MinimumMoisturePercent}% for device {node.SerialNumber}.",
                        telemetryId,
                        $"{{\"moisture\":{moisture}}}");
                }
                else
                {
                    await ResolveIfNormalizedAsync(node, AlertType.SoilMoistureLow, thresholds.AutoResolveWhenNormalized);
                }
            }

            if (TryParseDecimal(nodeData?.SoilTemperature, out var temperature))
            {
                if (temperature > thresholds.MaximumTemperature)
                {
                    await UpsertAlertAsync(node, facilityId, AlertType.TemperatureHigh, AlertSeverity.Critical,
                        "High soil temperature",
                        $"Temperature {temperature}°C exceeds maximum {thresholds.MaximumTemperature}°C on device {node.SerialNumber}.",
                        telemetryId);
                }
                else if (temperature < thresholds.MinimumTemperature)
                {
                    await UpsertAlertAsync(node, facilityId, AlertType.TemperatureLow, AlertSeverity.Warning,
                        "Low soil temperature",
                        $"Temperature {temperature}°C is below minimum {thresholds.MinimumTemperature}°C on device {node.SerialNumber}.",
                        telemetryId);
                }
                else
                {
                    await ResolveIfNormalizedAsync(node, AlertType.TemperatureHigh, thresholds.AutoResolveWhenNormalized);
                    await ResolveIfNormalizedAsync(node, AlertType.TemperatureLow, thresholds.AutoResolveWhenNormalized);
                }
            }

            if (IsSensorFailure(nodeData))
            {
                await UpsertAlertAsync(node, facilityId, AlertType.SensorFailure, AlertSeverity.Warning,
                    "Sensor data quality issue",
                    $"Incomplete or invalid sensor payload received from device {node.SerialNumber}.",
                    telemetryId);
            }
            else
            {
                await ResolveIfNormalizedAsync(node, AlertType.SensorFailure, thresholds.AutoResolveWhenNormalized);
            }

            if (quality == TelemetryQualityStatus.Anomaly || IsTelemetryAnomaly(nodeData, thresholds))
            {
                await UpsertAlertAsync(node, facilityId, AlertType.TelemetryAnomaly, AlertSeverity.Warning,
                    "Telemetry anomaly",
                    $"Unusual telemetry pattern detected for device {node.SerialNumber}.",
                    telemetryId);
            }
            else if (quality == TelemetryQualityStatus.Stale)
            {
                await UpsertAlertAsync(node, facilityId, AlertType.TelemetryAnomaly, AlertSeverity.Warning,
                    "Stale telemetry",
                    $"Device {node.SerialNumber} telemetry has not been refreshed within {thresholds.StaleTelemetryThresholdMinutes} minutes.",
                    telemetryId);
            }
            else if (quality == TelemetryQualityStatus.Good)
            {
                await ResolveIfNormalizedAsync(node, AlertType.TelemetryAnomaly, thresholds.AutoResolveWhenNormalized);
            }
        }

        public async Task<MonitoringEvaluationResult> RunOfflineMonitoringForTenantAsync(int tenantId)
        {
            _jobResult = new MonitoringEvaluationResult();
            var thresholds = await GetThresholdsAsync(tenantId, null);
            if (!thresholds.MonitoringEnabled)
            {
                return _jobResult;
            }

            var nodes = await _nodeRepository.GetAll()
                .Include(n => n.Facility)
                .Where(n => n.TenantId == tenantId && n.IsActive && n.DeviceStatus != DeviceOperationalStatus.Decommissioned)
                .ToListAsync();

            foreach (var node in nodes)
            {
                var nodeThresholds = await GetThresholdsAsync(tenantId, node.Facility?.Id);
                _jobResult.DevicesEvaluated++;
                var wasOnline = node.IsOnline(nodeThresholds.OfflineTimeoutMinutes);
                var latestData = await GetLatestNodeDataAsync(node.Id);
                var quality = EvaluateTelemetryQuality(node, latestData, nodeThresholds);
                node.ApplyOperationalEvaluation(nodeThresholds.OfflineTimeoutMinutes, quality);
                await _nodeRepository.UpdateAsync(node);

                var isOnline = node.IsOnline(nodeThresholds.OfflineTimeoutMinutes);
                if (!isOnline)
                {
                    await UpsertAlertAsync(node, node.Facility?.Id, AlertType.DeviceOffline, AlertSeverity.Critical,
                        "Device offline",
                        $"Device {node.SerialNumber} has not reported within {nodeThresholds.OfflineTimeoutMinutes} minutes.",
                        null);
                }
                else
                {
                    await ResolveIfNormalizedAsync(node, AlertType.DeviceOffline, nodeThresholds.AutoResolveWhenNormalized);
                }
            }

            return _jobResult;
        }

        public async Task<MonitoringEvaluationResult> RunTelemetryHealthMonitoringForTenantAsync(int tenantId)
        {
            _jobResult = new MonitoringEvaluationResult();
            var thresholds = await GetThresholdsAsync(tenantId, null);
            if (!thresholds.MonitoringEnabled)
            {
                return _jobResult;
            }

            var nodes = await _nodeRepository.GetAll()
                .Include(n => n.Facility)
                .Where(n => n.TenantId == tenantId && n.IsActive && n.DeviceStatus != DeviceOperationalStatus.Decommissioned)
                .ToListAsync();

            foreach (var node in nodes)
            {
                var nodeThresholds = await GetThresholdsAsync(tenantId, node.Facility?.Id);
                if (!nodeThresholds.MonitoringEnabled)
                {
                    continue;
                }

                _jobResult.DevicesEvaluated++;
                var latestData = await GetLatestNodeDataAsync(node.Id);
                var quality = EvaluateTelemetryQuality(node, latestData, nodeThresholds);
                node.ApplyOperationalEvaluation(nodeThresholds.OfflineTimeoutMinutes, quality);
                await _nodeRepository.UpdateAsync(node);

                if (!node.IsOnline(nodeThresholds.OfflineTimeoutMinutes))
                {
                    continue;
                }

                if (quality == TelemetryQualityStatus.Missing || quality == TelemetryQualityStatus.Stale)
                {
                    await UpsertAlertAsync(node, node.Facility?.Id, AlertType.TelemetryAnomaly, AlertSeverity.Warning,
                        quality == TelemetryQualityStatus.Missing ? "Missing telemetry" : "Stale telemetry",
                        quality == TelemetryQualityStatus.Missing
                            ? $"Device {node.SerialNumber} has no usable telemetry payload."
                            : $"Device {node.SerialNumber} telemetry is stale (>{nodeThresholds.StaleTelemetryThresholdMinutes} min).",
                        latestData?.Id);
                }
                else if (quality == TelemetryQualityStatus.Anomaly)
                {
                    await UpsertAlertAsync(node, node.Facility?.Id, AlertType.TelemetryAnomaly, AlertSeverity.Warning,
                        "Telemetry anomaly",
                        $"Suspicious telemetry values detected for device {node.SerialNumber}.",
                        latestData?.Id);
                }
                else if (IsSensorFailure(latestData))
                {
                    await UpsertAlertAsync(node, node.Facility?.Id, AlertType.SensorFailure, AlertSeverity.Warning,
                        "Sensor data quality issue",
                        $"Incomplete sensor payload from device {node.SerialNumber}.",
                        latestData?.Id);
                }
                else
                {
                    await ResolveIfNormalizedAsync(node, AlertType.TelemetryAnomaly, nodeThresholds.AutoResolveWhenNormalized);
                    await ResolveIfNormalizedAsync(node, AlertType.SensorFailure, nodeThresholds.AutoResolveWhenNormalized);
                }
            }

            return _jobResult;
        }

        public async Task<MonitoringEvaluationResult> RunAlertEscalationForTenantAsync(int tenantId)
        {
            _jobResult = new MonitoringEvaluationResult();
            var thresholds = await GetThresholdsAsync(tenantId, null);
            if (!thresholds.MonitoringEnabled)
            {
                return _jobResult;
            }

            var cutoff = DateTime.UtcNow.AddMinutes(-thresholds.EscalationTimeoutMinutes);
            var candidates = await _alertRepository.GetAll()
                .Include(a => a.Node)
                .Where(a =>
                    a.TenantId == tenantId &&
                    a.IsActive &&
                    !a.IsResolved &&
                    a.Severity < AlertSeverity.Critical &&
                    (a.LastTriggeredAt ?? a.TriggeredAt) <= cutoff)
                .ToListAsync();

            foreach (var alert in candidates)
            {
                var previousSeverity = alert.Severity;
                alert.RecordEscalation(AlertSeverity.Critical);
                await _alertRepository.UpdateAsync(alert);
                _jobResult.EscalationsPerformed++;
                if (previousSeverity != AlertSeverity.Critical)
                {
                    _jobResult.AlertsGenerated++;
                }

                await NotifyAsync(alert, "escalated");
            }

            return _jobResult;
        }

        private async Task<NodeDataEntity> GetLatestNodeDataAsync(Guid nodeId)
        {
            return await _nodeDataRepository.GetAll()
                .Include(d => d.Node)
                .Where(d => d.Node != null && d.Node.Id == nodeId)
                .OrderByDescending(d => d.CreationTime)
                .FirstOrDefaultAsync();
        }

        private static TelemetryQualityStatus EvaluateTelemetryQuality(
            Node node,
            NodeDataEntity data,
            AlertThresholdConfiguration thresholds)
        {
            if (!node.LastSeenAt.HasValue)
            {
                return TelemetryQualityStatus.Missing;
            }

            if (node.LastSeenAt.Value < DateTime.UtcNow.AddMinutes(-thresholds.StaleTelemetryThresholdMinutes))
            {
                return TelemetryQualityStatus.Stale;
            }

            if (data == null)
            {
                return TelemetryQualityStatus.Missing;
            }

            if (IsTelemetryAnomaly(data, thresholds))
            {
                return TelemetryQualityStatus.Anomaly;
            }

            if (IsSensorFailure(data))
            {
                return TelemetryQualityStatus.Missing;
            }

            return TelemetryQualityStatus.Good;
        }

        private async Task UpsertAlertAsync(
            Node node,
            Guid? facilityId,
            AlertType alertType,
            AlertSeverity severity,
            string title,
            string description,
            Guid? telemetryId,
            string metadataJson = null)
        {
            var existing = await _alertRepository.GetAll()
                .FirstOrDefaultAsync(a =>
                    a.TenantId == node.TenantId &&
                    a.NodeId == node.Id &&
                    a.AlertType == alertType &&
                    a.IsActive &&
                    !a.IsResolved);

            if (existing != null)
            {
                var severityIncreased = severity > existing.Severity;
                existing.UpdateLastTriggered(severity, description, telemetryId);
                await _alertRepository.UpdateAsync(existing);
                await NotifyAsync(existing, severityIncreased ? "escalated" : "updated");
                return;
            }

            var alert = Alert.Raise(node.TenantId, node.Id, facilityId, alertType, severity, title, description, telemetryId, metadataJson);
            alert.AttachNode(node);
            await _alertRepository.InsertAsync(alert);
            await CurrentUnitOfWork.SaveChangesAsync();
            if (_jobResult != null)
            {
                _jobResult.AlertsGenerated++;
            }

            await NotifyAsync(alert, "created");
        }

        private async Task ResolveIfNormalizedAsync(Node node, AlertType alertType, bool autoResolve)
        {
            if (!autoResolve)
            {
                return;
            }

            var existing = await _alertRepository.GetAll()
                .FirstOrDefaultAsync(a =>
                    a.TenantId == node.TenantId &&
                    a.NodeId == node.Id &&
                    a.AlertType == alertType &&
                    a.IsActive &&
                    !a.IsResolved);

            if (existing == null)
            {
                return;
            }

            existing.AutoResolve();
            await _alertRepository.UpdateAsync(existing);
            if (_jobResult != null)
            {
                _jobResult.AlertsResolved++;
            }

            await NotifyAsync(existing, "resolved");
        }

        private async Task<AlertThresholdConfiguration> GetThresholdsAsync(int tenantId, Guid? facilityId)
        {
            if (facilityId.HasValue)
            {
                var facilityConfig = await _thresholdRepository.GetAll()
                    .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.FacilityId == facilityId.Value);

                if (facilityConfig != null)
                {
                    return facilityConfig;
                }
            }

            var tenantConfig = await _thresholdRepository.GetAll()
                .FirstOrDefaultAsync(t => t.TenantId == tenantId && t.FacilityId == null);

            return tenantConfig ?? AlertThresholdConfiguration.CreateDefault(tenantId);
        }

        private async Task NotifyAsync(Alert alert, string action)
        {
            if (_realtimeNotifier == null)
            {
                return;
            }

            await _realtimeNotifier.NotifyAsync(AlertMappingHelper.ToDto(alert), action);
        }

        private static bool TryParseDecimal(string value, out decimal result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)
                || decimal.TryParse(value, out result);
        }

        private static bool IsSensorFailure(NodeDataEntity data)
        {
            if (data == null)
            {
                return true;
            }

            var hasMoisture = !string.IsNullOrWhiteSpace(data.Moisture);
            var hasTemp = !string.IsNullOrWhiteSpace(data.SoilTemperature);
            return !hasMoisture && !hasTemp;
        }

        private static bool IsTelemetryAnomaly(NodeDataEntity data, AlertThresholdConfiguration thresholds = null)
        {
            if (data == null)
            {
                return false;
            }

            var sensitivity = thresholds?.AnomalySensitivityPercent ?? 50;
            var moistureHigh = 100m - (sensitivity * 0.5m);
            var moistureLow = sensitivity * 0.05m;
            var tempHigh = 50m + (sensitivity * 0.3m);
            var tempLow = -10m - (sensitivity * 0.2m);

            if (TryParseDecimal(data.Moisture, out var moisture) && (moisture > moistureHigh || moisture < moistureLow))
            {
                return true;
            }

            if (TryParseDecimal(data.SoilTemperature, out var temp) && (temp > tempHigh || temp < tempLow))
            {
                return true;
            }

            return false;
        }
    }
}
