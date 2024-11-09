using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Services.Facilities.Dto;
using Farmru.IotMonitoring.Users.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Facilities
{

    /// <summary>
    /// 
    /// </summary>
    [AbpAuthorize()]
    public class FacilityAppointmentAppService : AsyncCrudAppService<FacilityAppointment, FacilityAppointmentDto, Guid, PagedUserResultRequestDto, FacilityAppointmentDto, FacilityAppointmentDto>
    {
        public FacilityAppointmentAppService(IRepository<FacilityAppointment, Guid> repository) : base(repository)
        {
                
        }
    }
}
