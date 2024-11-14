using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Services.Persons.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Farmru.IotMonitoring.Services.Persons
{
    [AbpAuthorize]
    public class PersonAppService : ApplicationService, IApplicationService
    {
        private readonly IRepository<Person,Guid> _personRepository;
        public PersonAppService(IRepository<Person, Guid> personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<PeopleDto> GetAll()
        { 
            return ObjectMapper.Map<PeopleDto>(await _personRepository.GetAllListAsync());
        }
    }
}
