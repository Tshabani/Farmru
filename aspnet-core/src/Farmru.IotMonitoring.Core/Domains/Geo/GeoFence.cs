using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Alerts;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Geo;
using System;

namespace Farmru.IotMonitoring.Domains.Geo
{
    public class GeoFence : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected GeoFence()
        {
        }

        public int TenantId { get; set; }
        public virtual string Name { get; private set; }
        public virtual string Description { get; private set; }
        public virtual GeoFenceType GeoFenceType { get; private set; }
        public virtual decimal? CenterLatitude { get; private set; }
        public virtual decimal? CenterLongitude { get; private set; }
        public virtual double? RadiusMeters { get; private set; }
        public virtual string PolygonJson { get; private set; }
        public virtual bool IsActive { get; private set; }
        public virtual AlertSeverity Severity { get; private set; }
        public virtual Guid? FacilityId { get; private set; }
        public virtual Facility Facility { get; private set; }
        public virtual bool TriggerAlertOnExit { get; private set; }
        public virtual bool TriggerAlertOnEntry { get; private set; }

        public static GeoFence Create(
            int tenantId,
            string name,
            GeoFenceType geoFenceType,
            AlertSeverity severity,
            bool triggerAlertOnEntry = true,
            bool triggerAlertOnExit = true,
            string description = null,
            Facility facility = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainRuleException("Geo-fence name is required.");
            }

            return new GeoFence
            {
                TenantId = tenantId,
                Name = name.Trim(),
                Description = description?.Trim(),
                GeoFenceType = geoFenceType,
                Severity = severity,
                Facility = facility,
                FacilityId = facility?.Id,
                IsActive = true,
                TriggerAlertOnEntry = triggerAlertOnEntry,
                TriggerAlertOnExit = triggerAlertOnExit
            };
        }

        public virtual void ConfigureRadius(decimal centerLatitude, decimal centerLongitude, double radiusMeters)
        {
            var coords = GeoCoordinateHelper.Normalize(centerLatitude, centerLongitude)
                ?? throw new DomainRuleException("Invalid geo-fence center coordinates.");

            if (radiusMeters <= 0)
            {
                throw new DomainRuleException("Radius must be greater than zero.");
            }

            GeoFenceType = GeoFenceType.Radius;
            CenterLatitude = coords.Latitude;
            CenterLongitude = coords.Longitude;
            RadiusMeters = radiusMeters;
            PolygonJson = null;
        }

        public virtual void ConfigurePolygon(string polygonJson)
        {
            if (GeoCoordinateHelper.ParsePolygon(polygonJson).Count < 3)
            {
                throw new DomainRuleException("Polygon must contain at least three points.");
            }

            GeoFenceType = GeoFenceType.Polygon;
            PolygonJson = polygonJson;
            CenterLatitude = null;
            CenterLongitude = null;
            RadiusMeters = null;
        }

        public virtual void UpdateProfile(
            string name,
            string description,
            AlertSeverity severity,
            bool triggerAlertOnEntry,
            bool triggerAlertOnExit,
            Facility facility)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainRuleException("Geo-fence name is required.");
            }

            Name = name.Trim();
            Description = description?.Trim();
            Severity = severity;
            TriggerAlertOnEntry = triggerAlertOnEntry;
            TriggerAlertOnExit = triggerAlertOnExit;
            Facility = facility;
            FacilityId = facility?.Id;
        }

        public virtual void SetActive(bool isActive) => IsActive = isActive;

        public virtual bool ContainsPoint(decimal latitude, decimal longitude)
        {
            if (!IsActive)
            {
                return false;
            }

            if (GeoFenceType == GeoFenceType.Radius)
            {
                if (!CenterLatitude.HasValue || !CenterLongitude.HasValue || !RadiusMeters.HasValue)
                {
                    return false;
                }

                return GeoCoordinateHelper.IsInsideRadius(
                    latitude,
                    longitude,
                    CenterLatitude.Value,
                    CenterLongitude.Value,
                    RadiusMeters.Value);
            }

            return GeoCoordinateHelper.IsInsidePolygon(latitude, longitude, PolygonJson);
        }
    }
}
