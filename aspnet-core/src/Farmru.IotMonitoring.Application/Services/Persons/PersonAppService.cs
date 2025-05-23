﻿using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Services.Nodes.Dto;
using Farmru.IotMonitoring.Services.Nodes;
using Farmru.IotMonitoring.Services.Persons.Dtos;
using Farmru.IotMonitoring.Users.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace Farmru.IotMonitoring.Services.Persons
{
    [AbpAuthorize]
    public class PersonAppService : AsyncCrudAppService<Person, PersonDto, Guid, PagedResultRequestDto, CreatePersonDto, PersonDto>
    {
        public PersonAppService(IRepository<Person, Guid> repository) : base(repository)
        {
        }

        public async Task<List<PeopleDto>> GetListOfPeople()
        { 
            var people = await Repository.GetAllListAsync();
            return ObjectMapper.Map<List<PeopleDto>>(people);
        }
    }
}

