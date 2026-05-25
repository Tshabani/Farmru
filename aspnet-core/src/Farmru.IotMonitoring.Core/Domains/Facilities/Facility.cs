using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Organisations;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Geo;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farmru.IotMonitoring.Domains.Facilities
{
    public class Facility : FullAuditedAggregateRoot<Guid>
    {
        protected Facility()
        {
        }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public virtual string Name { get; private set; }

        [StringLength(300)]
        public virtual string Description { get; private set; }

        public virtual string Address { get; private set; }
        public virtual Person PrimaryContact { get; private set; }
        public virtual Organisation OwnerOrganisation { get; private set; }

        [Column(TypeName = "decimal(10, 8)")]
        public virtual decimal? Latitude { get; private set; }

        [Column(TypeName = "decimal(10, 8)")]
        public virtual decimal? Longitude { get; private set; }

        [Column(TypeName = "decimal(10, 8)")]
        public virtual decimal? Altitude { get; private set; }

        public virtual bool? IsDefault { get; private set; }

        public static Facility Create(
            string name,
            string description = null,
            string address = null,
            Person primaryContact = null,
            Organisation ownerOrganisation = null,
            decimal? latitude = null,
            decimal? longitude = null,
            decimal? altitude = null,
            bool? isDefault = null)
        {
            var facility = new Facility();
            facility.SetName(name);
            facility.UpdateDetails(description, address, primaryContact, ownerOrganisation, latitude, longitude, altitude, isDefault);
            return facility;
        }

        public virtual void SetName(string name)
        {
            var trimmed = name?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.Length < 2)
            {
                throw new DomainRuleException("Facility name must be at least 2 characters.");
            }

            Name = trimmed;
        }

        public virtual void UpdateDetails(
            string description,
            string address,
            Person primaryContact,
            Organisation ownerOrganisation,
            decimal? latitude,
            decimal? longitude,
            decimal? altitude,
            bool? isDefault)
        {
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
            PrimaryContact = primaryContact;
            OwnerOrganisation = ownerOrganisation;
            if (latitude.HasValue && longitude.HasValue)
            {
                var coords = GeoCoordinateHelper.Normalize(latitude, longitude)
                    ?? throw new DomainRuleException("Invalid facility coordinates.");
                Latitude = coords.Latitude;
                Longitude = coords.Longitude;
            }
            else
            {
                Latitude = null;
                Longitude = null;
            }
            Altitude = altitude;
            IsDefault = isDefault;
        }
    }
}
