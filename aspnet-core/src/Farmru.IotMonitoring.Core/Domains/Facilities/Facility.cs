using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Authorization.Users;
using Farmru.IotMonitoring.Domains.Organisations;
using Farmru.IotMonitoring.Domains.Persons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Domains.Facilities
{
    public class Facility : FullAuditedEntity<Guid>
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public virtual string Name { get; set; }
        [DataType(DataType.MultilineText)]
        [StringLength(300)]
        public virtual string Description { get; set; }
        public virtual string Address { get; set; }
        public virtual Person? PrimaryContact { get; set; }
        public virtual Organisation? OwnerOrganisation { get; set; }
        public virtual decimal? Latitude { get; set; }
        public virtual decimal? Longitude { get; set; }
        public virtual decimal? Altitude { get; set; }
    }
}
