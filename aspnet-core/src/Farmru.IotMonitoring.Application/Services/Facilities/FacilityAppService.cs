using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Services.Facilities.Dto;
using Farmru.IotMonitoring.Services.NodeData.Dto;
using Farmru.IotMonitoring.Services.Persons.Dtos;
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
    public class FacilityAppService : AsyncCrudAppService<Facility, FacilityDto, Guid, PagedUserResultRequestDto, CreateFacilityDto, FacilityDto>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public FacilityAppService(IRepository<Facility, Guid> repository) : base(repository)
        { 
            
        }

        public async Task<List<FacilitiesDto>> GetListOfFacilities()
        {
            var people = await Repository.GetAllListAsync();
            return ObjectMapper.Map<List<FacilitiesDto>>(people);
        }
    }
}
