using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Facilities.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(FacilityAppointment))]
    public class FacilityAppointmentDto : EntityDto<Guid>
    {
        public EntityWithDisplayNameDto<Guid?>? AppointedUser { get; set; }
        public EntityWithDisplayNameDto<Guid?>? Facility { get; set; }
    }
}
