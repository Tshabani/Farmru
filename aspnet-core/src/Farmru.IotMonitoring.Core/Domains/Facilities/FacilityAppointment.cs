using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Authorization.Users;
using Farmru.IotMonitoring.Domains.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Domains.Facilities
{
    public class FacilityAppointment : FullAuditedEntity<Guid>
    {
        public virtual Person? AppointedUser { get; set; }
        public virtual Facility? Facility { get; set; }
    }
}
