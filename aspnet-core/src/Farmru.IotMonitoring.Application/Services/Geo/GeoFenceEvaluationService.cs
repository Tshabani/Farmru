using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Farmru.IotMonitoring.Alerts;
using Farmru.IotMonitoring.Domains.Alerts;
using Farmru.IotMonitoring.Domains.Geo;
using Farmru.IotMonitoring.Domains.Geo.Services;
using Farmru.IotMonitoring.Domains.Nodes;
using Farmru.IotMonitoring.GeoSpatial;
using Farmru.IotMonitoring.Services.Alerts;
using Farmru.IotMonitoring.Services.Alerts.Dto;
using Farmru.IotMonitoring.Services.GeoSpatial.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Geo
{
    public class GeoFenceEvaluationService : DomainService, IGeoFenceEvaluationService
    {
        private readonly IRepository<GeoFence, Guid> _geoFenceRepository;
        private readonly IRepository<Alert, Guid> _alertRepository;
        private readonly IRepository<Node, Guid> _nodeRepository;
        private readonly IAlertRealtimeNotifier _alertNotifier;
        private readonly IGeoSpatialRealtimeNotifier _mapNotifier;

        public GeoFenceEvaluationService(
            IRepository<GeoFence, Guid> geoFenceRepository,
            IRepository<Alert, Guid> alertRepository,
            IRepository<Node, Guid> nodeRepository,
            IAlertRealtimeNotifier alertNotifier,
            IGeoSpatialRealtimeNotifier mapNotifier)
        {
            _geoFenceRepository = geoFenceRepository;
            _alertRepository = alertRepository;
            _nodeRepository = nodeRepository;
            _alertNotifier = alertNotifier;
            _mapNotifier = mapNotifier;
        }

        public async Task EvaluateNodeLocationAsync(Node node, decimal latitude, decimal longitude)
        {
            if (node == null)
            {
                return;
            }

            var fences = await _geoFenceRepository.GetAll()
                .Where(f => f.TenantId == node.TenantId && f.IsActive)
                .Where(f => !f.FacilityId.HasValue || (node.Facility != null && f.FacilityId == node.Facility.Id))
                .ToListAsync();

            foreach (var fence in fences)
            {
                var inside = fence.ContainsPoint(latitude, longitude);
                var existing = await _alertRepository.GetAll()
                    .FirstOrDefaultAsync(a =>
                        a.TenantId == node.TenantId &&
                        a.NodeId == node.Id &&
                        a.AlertType == AlertType.GeoFenceBreach &&
                        a.IsActive &&
                        !a.IsResolved &&
                        a.MetadataJson != null &&
                        a.MetadataJson.Contains(fence.Id.ToString()));

                var wasInside = ParseInsideState(existing?.MetadataJson);

                if (inside && fence.TriggerAlertOnEntry && !wasInside)
                {
                    await RaiseBreachAsync(node, fence, true, latitude, longitude);
                }
                else if (!inside && wasInside && fence.TriggerAlertOnExit)
                {
                    await RaiseBreachAsync(node, fence, false, latitude, longitude);
                    if (existing != null)
                    {
                        existing.AutoResolve("Device exited geo-fence.");
                        await _alertRepository.UpdateAsync(existing);
                        await NotifyAlertAsync(existing, "resolved");
                    }
                }
                else if (inside && existing != null)
                {
                    existing.UpdateLastTriggered(fence.Severity, $"Device remains inside geo-fence {fence.Name}.", null);
                    await _alertRepository.UpdateAsync(existing);
                }
            }
        }

        public async Task SyncGeoFencesForTenantAsync(int tenantId)
        {
            var nodes = await _nodeRepository.GetAll()
                .Include(n => n.Facility)
                .Where(n => n.TenantId == tenantId && n.IsActive)
                .ToListAsync();

            foreach (var node in nodes)
            {
                var coords = node.ResolveMapCoordinates();
                if (coords.HasValue)
                {
                    await EvaluateNodeLocationAsync(node, coords.Value.Latitude, coords.Value.Longitude);
                }
            }
        }

        private async Task RaiseBreachAsync(Node node, GeoFence fence, bool isEntry, decimal lat, decimal lon)
        {
            var title = isEntry ? $"Geo-fence entry: {fence.Name}" : $"Geo-fence exit: {fence.Name}";
            var description = isEntry
                ? $"Device {node.SerialNumber} entered geo-fence {fence.Name}."
                : $"Device {node.SerialNumber} exited geo-fence {fence.Name}.";

            var metadata = JsonSerializer.Serialize(new
            {
                geoFenceId = fence.Id,
                inside = isEntry,
                latitude = lat,
                longitude = lon
            });

            var alert = Alert.Raise(
                node.TenantId,
                node.Id,
                node.Facility?.Id,
                AlertType.GeoFenceBreach,
                fence.Severity,
                title,
                description,
                null,
                metadata);
            alert.AttachNode(node);
            await _alertRepository.InsertAsync(alert);
            await CurrentUnitOfWork.SaveChangesAsync();
            await NotifyAlertAsync(alert, "created");
            await NotifyMapAsync(node.TenantId, new MapUpdateEventDto
            {
                EventType = isEntry ? "geofence_entry" : "geofence_exit",
                Message = description,
                NodeId = node.Id,
                NodeDisplay = node.SerialNumber,
                AlertId = alert.Id,
                Latitude = lat,
                Longitude = lon,
                Severity = (int)fence.Severity
            });
        }

        private static bool ParseInsideState(string metadataJson)
        {
            if (string.IsNullOrWhiteSpace(metadataJson))
            {
                return false;
            }

            try
            {
                using var doc = JsonDocument.Parse(metadataJson);
                if (doc.RootElement.TryGetProperty("inside", out var inside))
                {
                    return inside.GetBoolean();
                }
            }
            catch (JsonException)
            {
            }

            return false;
        }

        private async Task NotifyAlertAsync(Alert alert, string action)
        {
            if (_alertNotifier == null)
            {
                return;
            }

            await _alertNotifier.NotifyAsync(AlertMappingHelper.ToDto(alert), action);
        }

        private Task NotifyMapAsync(int tenantId, MapUpdateEventDto evt)
        {
            return _mapNotifier == null
                ? Task.CompletedTask
                : _mapNotifier.NotifyMapUpdateAsync(tenantId, evt);
        }
    }
}
