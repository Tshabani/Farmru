using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Farmru.IotMonitoring.Domains.Incidents;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Incidents.Dto
{
    [AutoMap(typeof(Incident))]
    public class IncidentDto : EntityDto<Guid>
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public IncidentStatus? Status { get; set; }
        public IncidentPriority? Priority { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public EntityWithDisplayNameDto<Guid?>? CreatedBy { get; set; }
        public EntityWithDisplayNameDto<Guid?>? AssignedTo { get; set; }
    }
}
