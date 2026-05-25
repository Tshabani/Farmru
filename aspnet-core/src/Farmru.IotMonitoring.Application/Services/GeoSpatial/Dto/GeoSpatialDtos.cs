using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Domains.Alerts;
using Farmru.IotMonitoring.Domains.Geo;
using System;
using System.Collections.Generic;

namespace Farmru.IotMonitoring.Services.GeoSpatial.Dto
{
    public class MapUpdateEventDto
    {
        public string EventType { get; set; }
        public string Message { get; set; }
        public Guid? NodeId { get; set; }
        public string NodeDisplay { get; set; }
        public Guid? AlertId { get; set; }
        public Guid? FacilityId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? Severity { get; set; }
    }

    public class OperationalMapDto
    {
        public List<DeviceMapMarkerDto> Devices { get; set; } = new();
        public List<FacilityMapMarkerDto> Facilities { get; set; } = new();
        public List<AlertMapMarkerDto> Alerts { get; set; } = new();
        public List<IncidentMapMarkerDto> Incidents { get; set; } = new();
        public List<GeoFenceMapDto> GeoFences { get; set; } = new();
    }

    public class DeviceMapMarkerDto
    {
        public Guid Id { get; set; }
        public string DisplayText { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool IsOnline { get; set; }
        public int HealthStatus { get; set; }
        public decimal? BatteryLevel { get; set; }
        public Guid? FacilityId { get; set; }
        public string FacilityName { get; set; }
        public int ActiveAlertCount { get; set; }
        public string LatestAlertTitle { get; set; }
    }

    public class FacilityMapMarkerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int DeviceCount { get; set; }
        public int ActiveAlertCount { get; set; }
        public int OfflineDeviceCount { get; set; }
        public int OperationalScore { get; set; }
    }

    public class AlertMapMarkerDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Severity { get; set; }
        public int AlertType { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public Guid? NodeId { get; set; }
    }

    public class IncidentMapMarkerDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class GeoFenceMapDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public GeoFenceType GeoFenceType { get; set; }
        public decimal? CenterLatitude { get; set; }
        public decimal? CenterLongitude { get; set; }
        public double? RadiusMeters { get; set; }
        public string PolygonJson { get; set; }
        public bool IsActive { get; set; }
        public int Severity { get; set; }
        public Guid? FacilityId { get; set; }
    }

    public class AlertHeatmapPointDto
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Weight { get; set; }
        public int Severity { get; set; }
    }

    public class ExecutiveGisSummaryDto
    {
        public int TotalDevices { get; set; }
        public int OnlineDevices { get; set; }
        public int OfflineDevices { get; set; }
        public int CriticalAlerts { get; set; }
        public int ActiveGeoFences { get; set; }
        public int FacilitiesMonitored { get; set; }
        public List<AlertHeatmapPointDto> Heatmap { get; set; } = new();
        public List<FacilityMapMarkerDto> TopRiskFacilities { get; set; } = new();
    }

    public class NearbyQueryInput
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public double RadiusKm { get; set; } = 10;
        public int MaxResults { get; set; } = 50;
    }

    public class GeoFenceDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public GeoFenceType GeoFenceType { get; set; }
        public decimal? CenterLatitude { get; set; }
        public decimal? CenterLongitude { get; set; }
        public double? RadiusMeters { get; set; }
        public string PolygonJson { get; set; }
        public bool IsActive { get; set; }
        public AlertSeverity Severity { get; set; }
        public Guid? FacilityId { get; set; }
        public bool TriggerAlertOnExit { get; set; }
        public bool TriggerAlertOnEntry { get; set; }
    }

    public class CreateGeoFenceInput
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public GeoFenceType GeoFenceType { get; set; }
        public decimal? CenterLatitude { get; set; }
        public decimal? CenterLongitude { get; set; }
        public double? RadiusMeters { get; set; }
        public string PolygonJson { get; set; }
        public AlertSeverity Severity { get; set; }
        public Guid? FacilityId { get; set; }
        public bool TriggerAlertOnExit { get; set; } = true;
        public bool TriggerAlertOnEntry { get; set; } = true;
    }

    public class UpdateGeoFenceInput : CreateGeoFenceInput
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
