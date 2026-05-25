using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Persons;
using System;

namespace Farmru.IotMonitoring.Domains.Facilities
{
    /// <summary>
    /// Links a person to a facility. Managed within the facility operational boundary.
    /// </summary>
    public class FacilityAppointment : FullAuditedEntity<Guid>
    {
        protected FacilityAppointment()
        {
        }

        public virtual Person AppointedUser { get; private set; }
        public virtual Facility Facility { get; private set; }

        public static FacilityAppointment Schedule(Person appointedUser, Facility facility)
        {
            if (appointedUser == null)
            {
                throw new DomainRuleException("Appointed user is required.");
            }

            if (facility == null)
            {
                throw new DomainRuleException("Facility is required.");
            }

            return new FacilityAppointment
            {
                AppointedUser = appointedUser,
                Facility = facility
            };
        }

        public virtual void Reschedule(Person appointedUser, Facility facility)
        {
            if (appointedUser == null || facility == null)
            {
                throw new DomainRuleException("Both user and facility are required.");
            }

            AppointedUser = appointedUser;
            Facility = facility;
        }
    }
}
