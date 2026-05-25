using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Alerts;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Geo;
using Farmru.IotMonitoring.Domains.Incidents;
using Farmru.IotMonitoring.Domains.Nodes;
using Farmru.IotMonitoring.Geo;
using Farmru.IotMonitoring.Services.GeoSpatial.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.GeoSpatial
{
    [AbpAuthorize(PermissionNames.Pages_Gis)]
    public class GeoSpatialAppService : IotMonitoringAppServiceBase, IGeoSpatialAppService
    {
        private readonly IRepository<Node, Guid> _nodeRepository;
        private readonly IRepository<Facility, Guid> _facilityRepository;
        private readonly IRepository<Alert, Guid> _alertRepository;
        private readonly IRepository<Incident, Guid> _incidentRepository;
        private readonly IRepository<GeoFence, Guid> _geoFenceRepository;

        public GeoSpatialAppService(
            IRepository<Node, Guid> nodeRepository,
            IRepository<Facility, Guid> facilityRepository,
            IRepository<Alert, Guid> alertRepository,
            IRepository<Incident, Guid> incidentRepository,
            IRepository<GeoFence, Guid> geoFenceRepository)
        {
            _nodeRepository = nodeRepository;
            _facilityRepository = facilityRepository;
            _alertRepository = alertRepository;
            _incidentRepository = incidentRepository;
            _geoFenceRepository = geoFenceRepository;
        }

        public async Task<OperationalMapDto> GetOperationalMap()
        {
            var devices = await _nodeRepository.GetAll()
                .Include(n => n.Facility)
                .Where(n => n.DeviceStatus != DeviceOperationalStatus.Decommissioned)
                .ToListAsync();

            var activeAlerts = await _alertRepository.GetAll()
                .Where(a => a.IsActive && !a.IsResolved)
                .ToListAsync();

            var deviceMarkers = new List<DeviceMapMarkerDto>();
            foreach (var node in devices)
            {
                var coords = node.ResolveMapCoordinates();
                if (!coords.HasValue)
                {
                    continue;
                }

                var nodeAlerts = activeAlerts.Where(a => a.NodeId == node.Id).ToList();
                deviceMarkers.Add(new DeviceMapMarkerDto
                {
                    Id = node.Id,
                    DisplayText = node.DisplayName ?? node.SerialNumber,
                    Latitude = coords.Value.Latitude,
                    Longitude = coords.Value.Longitude,
                    IsOnline = node.IsOnline(),
                    HealthStatus = (int)node.HealthStatus,
                    BatteryLevel = node.BatteryLevel,
                    FacilityId = node.Facility?.Id,
                    FacilityName = node.Facility?.Name,
                    ActiveAlertCount = nodeAlerts.Count,
                    LatestAlertTitle = nodeAlerts.OrderByDescending(a => a.LastTriggeredAt).FirstOrDefault()?.Title
                });
            }

            var facilities = await _facilityRepository.GetAll()
                .Where(f => f.Latitude.HasValue && f.Longitude.HasValue)
                .ToListAsync();

            var facilityMarkers = facilities.Select(f =>
            {
                var facilityDevices = devices.Where(d => d.Facility?.Id == f.Id).ToList();
                var facilityAlerts = activeAlerts.Where(a => a.FacilityId == f.Id).ToList();
                var offline = facilityDevices.Count(d => !d.IsOnline());
                var score = ComputeOperationalScore(facilityDevices.Count, offline, facilityAlerts.Count);
                return new FacilityMapMarkerDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Latitude = f.Latitude.Value,
                    Longitude = f.Longitude.Value,
                    DeviceCount = facilityDevices.Count,
                    ActiveAlertCount = facilityAlerts.Count,
                    OfflineDeviceCount = offline,
                    OperationalScore = score
                };
            }).ToList();

            var alertMarkers = new List<AlertMapMarkerDto>();
            foreach (var alert in activeAlerts)
            {
                var coords = await ResolveAlertCoordinatesAsync(alert, devices);
                if (!coords.HasValue)
                {
                    continue;
                }

                alertMarkers.Add(new AlertMapMarkerDto
                {
                    Id = alert.Id,
                    Title = alert.Title,
                    Severity = (int)alert.Severity,
                    AlertType = (int)alert.AlertType,
                    Latitude = coords.Value.Latitude,
                    Longitude = coords.Value.Longitude,
                    NodeId = alert.NodeId
                });
            }

            var incidents = await _incidentRepository.GetAll()
                .Where(i => i.Latitude.HasValue && i.Longitude.HasValue)
                .ToListAsync();

            return new OperationalMapDto
            {
                Devices = deviceMarkers,
                Facilities = facilityMarkers,
                Alerts = alertMarkers,
                Incidents = incidents.Select(i => new IncidentMapMarkerDto
                {
                    Id = i.Id,
                    Title = i.Title,
                    Priority = (int)i.Priority,
                    Status = (int)i.Status,
                    Latitude = i.Latitude.Value,
                    Longitude = i.Longitude.Value
                }).ToList(),
                GeoFences = (await GetGeoFencesInternalAsync()).Select(MapGeoFence).ToList()
            };
        }

        public async Task<ExecutiveGisSummaryDto> GetExecutiveSummary()
        {
            var map = await GetOperationalMap();
            return new ExecutiveGisSummaryDto
            {
                TotalDevices = map.Devices.Count,
                OnlineDevices = map.Devices.Count(d => d.IsOnline),
                OfflineDevices = map.Devices.Count(d => !d.IsOnline),
                CriticalAlerts = map.Alerts.Count(a => a.Severity == (int)AlertSeverity.Critical),
                ActiveGeoFences = map.GeoFences.Count(g => g.IsActive),
                FacilitiesMonitored = map.Facilities.Count,
                Heatmap = await GetAlertHeatmap(),
                TopRiskFacilities = map.Facilities.OrderBy(f => f.OperationalScore).Take(5).ToList()
            };
        }

        public async Task<List<AlertHeatmapPointDto>> GetAlertHeatmap()
        {
            var map = await GetOperationalMap();
            return map.Alerts
                .GroupBy(a => new { a.Latitude, a.Longitude, a.Severity })
                .Select(g => new AlertHeatmapPointDto
                {
                    Latitude = g.Key.Latitude,
                    Longitude = g.Key.Longitude,
                    Severity = g.Key.Severity,
                    Weight = g.Count()
                })
                .ToList();
        }

        public async Task<List<DeviceMapMarkerDto>> GetNearbyDevices(NearbyQueryInput input)
        {
            ValidateNearbyInput(input);
            var map = await GetOperationalMap();
            return map.Devices
                .Where(d => GeoCoordinateHelper.DistanceMeters(input.Latitude, input.Longitude, d.Latitude, d.Longitude) <= input.RadiusKm * 1000)
                .OrderBy(d => GeoCoordinateHelper.DistanceMeters(input.Latitude, input.Longitude, d.Latitude, d.Longitude))
                .Take(input.MaxResults)
                .ToList();
        }

        public async Task<List<IncidentMapMarkerDto>> GetNearbyIncidents(NearbyQueryInput input)
        {
            ValidateNearbyInput(input);
            var map = await GetOperationalMap();
            return map.Incidents
                .Where(i => GeoCoordinateHelper.DistanceMeters(input.Latitude, input.Longitude, i.Latitude, i.Longitude) <= input.RadiusKm * 1000)
                .OrderBy(i => GeoCoordinateHelper.DistanceMeters(input.Latitude, input.Longitude, i.Latitude, i.Longitude))
                .Take(input.MaxResults)
                .ToList();
        }

        public async Task<List<GeoFenceDto>> GetGeoFences() =>
            (await GetGeoFencesInternalAsync()).Select(MapGeoFenceDto).ToList();

        public async Task<GeoFenceDto> GetGeoFence(EntityDto<Guid> input)
        {
            var fence = await _geoFenceRepository.GetAsync(input.Id);
            return MapGeoFenceDto(fence);
        }

        [AbpAuthorize(PermissionNames.Pages_Gis_Manage)]
        public async Task<GeoFenceDto> CreateGeoFence(CreateGeoFenceInput input)
        {
            Facility facility = null;
            if (input.FacilityId.HasValue)
            {
                facility = await _facilityRepository.GetAsync(input.FacilityId.Value);
            }

            try
            {
                var fence = GeoFence.Create(
                    AbpSession.GetTenantId(),
                    input.Name,
                    input.GeoFenceType,
                    input.Severity,
                    input.TriggerAlertOnEntry,
                    input.TriggerAlertOnExit,
                    input.Description,
                    facility);

                ApplyGeometry(fence, input);
                await _geoFenceRepository.InsertAsync(fence);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapGeoFenceDto(fence);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Pages_Gis_Manage)]
        public async Task<GeoFenceDto> UpdateGeoFence(UpdateGeoFenceInput input)
        {
            var fence = await _geoFenceRepository.GetAsync(input.Id);
            Facility facility = null;
            if (input.FacilityId.HasValue)
            {
                facility = await _facilityRepository.GetAsync(input.FacilityId.Value);
            }

            try
            {
                fence.UpdateProfile(
                    input.Name,
                    input.Description,
                    input.Severity,
                    input.TriggerAlertOnEntry,
                    input.TriggerAlertOnExit,
                    facility);
                fence.SetActive(input.IsActive);
                ApplyGeometry(fence, input);
                await _geoFenceRepository.UpdateAsync(fence);
                return MapGeoFenceDto(fence);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AbpAuthorize(PermissionNames.Pages_Gis_Manage)]
        public async Task DeleteGeoFence(EntityDto<Guid> input)
        {
            await _geoFenceRepository.DeleteAsync(input.Id);
        }

        private async Task<List<GeoFence>> GetGeoFencesInternalAsync() =>
            await _geoFenceRepository.GetAll().ToListAsync();

        private static void ApplyGeometry(GeoFence fence, CreateGeoFenceInput input)
        {
            if (input.GeoFenceType == GeoFenceType.Radius)
            {
                fence.ConfigureRadius(input.CenterLatitude.Value, input.CenterLongitude.Value, input.RadiusMeters.Value);
            }
            else
            {
                fence.ConfigurePolygon(input.PolygonJson);
            }
        }

        private static GeoFenceDto MapGeoFenceDto(GeoFence fence) =>
            new GeoFenceDto
            {
                Id = fence.Id,
                Name = fence.Name,
                Description = fence.Description,
                GeoFenceType = fence.GeoFenceType,
                CenterLatitude = fence.CenterLatitude,
                CenterLongitude = fence.CenterLongitude,
                RadiusMeters = fence.RadiusMeters,
                PolygonJson = fence.PolygonJson,
                IsActive = fence.IsActive,
                Severity = fence.Severity,
                FacilityId = fence.FacilityId,
                TriggerAlertOnEntry = fence.TriggerAlertOnEntry,
                TriggerAlertOnExit = fence.TriggerAlertOnExit
            };

        private static GeoFenceMapDto MapGeoFence(GeoFence fence) =>
            new GeoFenceMapDto
            {
                Id = fence.Id,
                Name = fence.Name,
                GeoFenceType = fence.GeoFenceType,
                CenterLatitude = fence.CenterLatitude,
                CenterLongitude = fence.CenterLongitude,
                RadiusMeters = fence.RadiusMeters,
                PolygonJson = fence.PolygonJson,
                IsActive = fence.IsActive,
                Severity = (int)fence.Severity,
                FacilityId = fence.FacilityId
            };

        private static int ComputeOperationalScore(int deviceCount, int offlineCount, int alertCount)
        {
            if (deviceCount == 0)
            {
                return 100;
            }

            var onlineRatio = (deviceCount - offlineCount) * 100 / deviceCount;
            return Math.Max(0, onlineRatio - alertCount * 5);
        }

        private async Task<(decimal Latitude, decimal Longitude)?> ResolveAlertCoordinatesAsync(Alert alert, List<Node> devices)
        {
            if (alert.NodeId.HasValue)
            {
                var node = devices.FirstOrDefault(n => n.Id == alert.NodeId);
                return node?.ResolveMapCoordinates();
            }

            if (alert.FacilityId.HasValue)
            {
                var facility = await _facilityRepository.FirstOrDefaultAsync(alert.FacilityId.Value);
                if (facility?.Latitude != null && facility.Longitude != null)
                {
                    return (facility.Latitude.Value, facility.Longitude.Value);
                }
            }

            return null;
        }

        private static void ValidateNearbyInput(NearbyQueryInput input)
        {
            if (!GeoCoordinateHelper.IsValid(input.Latitude, input.Longitude))
            {
                throw new UserFriendlyException("Invalid coordinates for nearby query.");
            }
        }
    }
}
