using Abp.Domain.Entities;
using Farmru.IotMonitoring.Domains;
using Abp.Domain.Entities.Auditing;
using System;

namespace Farmru.IotMonitoring.Domains.Organisations
{
    public class Organisation : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected Organisation()
        {
        }

        public virtual string Name { get; private set; }
        public virtual string ShortAlias { get; private set; }
        public virtual string Description { get; private set; }
        public virtual string FreeTextAddress { get; private set; }
        public virtual int? OrganisationType { get; private set; }
        public virtual string CompanyRegistrationNo { get; private set; }
        public virtual string VatRegistrationNo { get; private set; }
        public virtual string ContactEmail { get; private set; }
        public virtual string ContactMobileNo { get; private set; }
        /// <summary>ABP multi-tenancy; set at creation.</summary>
        public int TenantId { get; set; }

        public static Organisation Create(int tenantId, string name, string shortAlias = null, string description = null)
        {
            if (tenantId <= 0)
            {
                throw new DomainRuleException("Tenant is required.");
            }

            var organisation = new Organisation { TenantId = tenantId };
            organisation.SetName(name);
            organisation.UpdateProfile(shortAlias, description, null, null, null, null, null, null);
            return organisation;
        }

        public virtual void SetName(string name)
        {
            var trimmed = name?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                throw new DomainRuleException("Organisation name is required.");
            }

            Name = trimmed;
        }

        public virtual void UpdateProfile(
            string shortAlias,
            string description,
            string freeTextAddress,
            int? organisationType,
            string companyRegistrationNo,
            string vatRegistrationNo,
            string contactEmail,
            string contactMobileNo)
        {
            ShortAlias = Normalize(shortAlias);
            Description = Normalize(description);
            FreeTextAddress = Normalize(freeTextAddress);
            OrganisationType = organisationType;
            CompanyRegistrationNo = Normalize(companyRegistrationNo);
            VatRegistrationNo = Normalize(vatRegistrationNo);
            ContactEmail = Normalize(contactEmail);
            ContactMobileNo = Normalize(contactMobileNo);
        }

        private static string Normalize(string value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
