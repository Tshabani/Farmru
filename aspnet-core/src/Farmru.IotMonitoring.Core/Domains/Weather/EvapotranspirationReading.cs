using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Facilities;
using System;

namespace Farmru.IotMonitoring.Domains.Weather
{
    /// <summary>
    /// Daily evapotranspiration for a Facility. CropSeasonId is an intentionally unconstrained
    /// seam (Phase 1 Technical Design Section 3.4): Crop Management's CropSeasons table does not
    /// exist yet at this point in the migration sequence, so no FK is declared until it does.
    /// </summary>
    public class EvapotranspirationReading : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected EvapotranspirationReading()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid FacilityId { get; private set; }
        public virtual Facility Facility { get; private set; }
        public virtual DateTime Date { get; private set; }
        public virtual decimal Et0Mm { get; private set; }
        public virtual decimal? EtcMm { get; private set; }
        public virtual Guid? CropSeasonId { get; private set; }

        public static EvapotranspirationReading Record(
            int tenantId,
            Facility facility,
            DateTime date,
            decimal et0Mm,
            decimal? etcMm = null,
            Guid? cropSeasonId = null)
        {
            if (facility == null)
            {
                throw new DomainRuleException("Facility is required for an evapotranspiration reading.");
            }

            if (et0Mm < 0)
            {
                throw new DomainRuleException("Reference evapotranspiration (Et0) cannot be negative.");
            }

            if (etcMm.HasValue && etcMm.Value < 0)
            {
                throw new DomainRuleException("Crop evapotranspiration (Etc) cannot be negative.");
            }

            return new EvapotranspirationReading
            {
                TenantId = tenantId,
                Facility = facility,
                FacilityId = facility.Id,
                Date = date.Date,
                Et0Mm = et0Mm,
                EtcMm = etcMm,
                CropSeasonId = cropSeasonId
            };
        }
    }
}
