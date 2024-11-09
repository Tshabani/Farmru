using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Domains.Organisations
{
    public class Organisation : FullAuditedEntity<Guid>, IMustHaveTenant
    {
        public virtual string Name { get; set; }
        public virtual string ShortAlias { get; set; }
        public virtual string Description { get; set; }
        public virtual string FreeTextAddress { get; set; }
        public virtual int? OrganisationType { get; set; }
        public virtual string CompanyRegistrationNo { get; set; }
        public virtual string VatRegistrationNo { get; set; }
        public virtual string ContactEmail { get; set; }
        public virtual string ContactMobileNo { get; set; }
        public int TenantId { get ; set; }
    }
}
