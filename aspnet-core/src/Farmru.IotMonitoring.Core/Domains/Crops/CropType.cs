using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using System;

namespace Farmru.IotMonitoring.Domains.Crops
{
    public class CropType : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected CropType()
        {
        }

        public int TenantId { get; set; }
        public virtual string Name { get; private set; }
        public virtual string ScientificName { get; private set; }
        public virtual int TypicalGrowthDurationDays { get; private set; }
        public virtual bool IsActive { get; private set; }

        public static CropType Create(int tenantId, string name, string scientificName, int typicalGrowthDurationDays)
        {
            var cropType = new CropType { TenantId = tenantId, IsActive = true };
            cropType.UpdateDetails(name, scientificName, typicalGrowthDurationDays);
            return cropType;
        }

        public virtual void UpdateDetails(string name, string scientificName, int typicalGrowthDurationDays)
        {
            var trimmed = name?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                throw new DomainRuleException("Crop type name is required.");
            }

            if (typicalGrowthDurationDays <= 0)
            {
                throw new DomainRuleException("Typical growth duration must be greater than zero days.");
            }

            Name = trimmed;
            ScientificName = string.IsNullOrWhiteSpace(scientificName) ? null : scientificName.Trim();
            TypicalGrowthDurationDays = typicalGrowthDurationDays;
        }

        public virtual void SetActive(bool isActive) => IsActive = isActive;
    }
}
