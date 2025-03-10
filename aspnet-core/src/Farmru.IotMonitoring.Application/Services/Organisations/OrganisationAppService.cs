﻿using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Organisations;
using Farmru.IotMonitoring.Services.Facilities.Dto;
using Farmru.IotMonitoring.Services.NodeData.Dto;
using Farmru.IotMonitoring.Services.Organisations.Dto;
using Farmru.IotMonitoring.Users.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Organisations
{
    /// <summary>
    /// 
    /// </summary>
    [AbpAuthorize()]
    public class OrganisationAppService : AsyncCrudAppService<Organisation, OrganisationDto, Guid, PagedResultRequestDto, OrganisationDto, OrganisationDto>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public OrganisationAppService(IRepository<Organisation, Guid> repository) : base(repository)
        {
                
        }

        public async Task<List<OrganisationsDto>> GetListOfOrganisations()
        {
            var people = await Repository.GetAllListAsync();
            return ObjectMapper.Map<List<OrganisationsDto>>(people);
        }
    }
}
