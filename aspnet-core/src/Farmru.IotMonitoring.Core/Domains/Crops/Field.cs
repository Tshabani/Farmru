using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Geo;
using System;

namespace Farmru.IotMonitoring.Domains.Crops
{
    /// <summary>
    /// A subdivision of a Facility that crops are actually planted on (Phase 1 Technical
    /// Design Section 2.2). Boundary reuses the existing GeoFence polygon/radius capability
    /// rather than introducing a new spatial type.
    /// </summary>
    public class Field : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected Field()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid FacilityId { get; private set; }
        public virtual Facility Facility { get; private set; }
        public virtual string Name { get; private set; }
        public virtual decimal? AreaHectares { get; private set; }
        public virtual string SoilType { get; private set; }
        public virtual Guid? BoundaryGeoFenceId { get; private set; }
        public virtual GeoFence Boundary { get; private set; }

        public static Field Create(
            int tenantId,
            Facility facility,
            string name,
            decimal? areaHectares = null,
            string soilType = null,
            GeoFence boundary = null)
        {
            if (facility == null)
            {
                throw new DomainRuleException("A Field must belong to a Facility.");
            }

            var field = new Field
            {
                TenantId = tenantId,
                Facility = facility,
                FacilityId = facility.Id,
                Boundary = boundary,
                BoundaryGeoFenceId = boundary?.Id
            };
            field.SetName(name);
            field.UpdateDetails(areaHectares, soilType);
            return field;
        }

        public virtual void SetName(string name)
        {
            var trimmed = name?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.Length < 2)
            {
                throw new DomainRuleException("Field name must be at least 2 characters.");
            }

            Name = trimmed;
        }

        public virtual void UpdateDetails(decimal? areaHectares, string soilType)
        {
            if (areaHectares.HasValue && areaHectares.Value <= 0)
            {
                throw new DomainRuleException("Field area must be greater than zero.");
            }

            AreaHectares = areaHectares;
            SoilType = string.IsNullOrWhiteSpace(soilType) ? null : soilType.Trim();
        }

        public virtual void AssignBoundary(GeoFence boundary)
        {
            Boundary = boundary;
            BoundaryGeoFenceId = boundary?.Id;
        }
    }
}
