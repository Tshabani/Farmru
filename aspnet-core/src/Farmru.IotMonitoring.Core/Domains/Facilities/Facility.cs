using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Authorization.Users;
using Farmru.IotMonitoring.Domains.Organisations;
using Farmru.IotMonitoring.Domains.Persons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Column(TypeName = "decimal(10, 8)")]
        public virtual decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(10, 8)")]
        public virtual decimal? Longitude { get; set; }
        [Column(TypeName = "decimal(10, 8)")]
        public virtual decimal? Altitude { get; set; }
        public virtual bool? IsDefault { get; set; }
    }
}
