using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Domains.Facilities
{
    public class FacilityAppointment : FullAuditedEntity<Guid>
    {
        public virtual User? AppointedUser { get; set; }
        public virtual Facility? Facility { get; set; }
    }
}
