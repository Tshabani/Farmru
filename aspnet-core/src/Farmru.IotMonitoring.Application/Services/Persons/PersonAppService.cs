using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Services.Persons.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public override async Task<PersonDto> CreateAsync(CreatePersonDto input)
        {
            try
            {
                var person = Person.Create(input.FirstName, input.LastName);
                person.UpdateProfile(
                    input.IdentityNumber,
                    input.Title,
                    input.Biography,
                    null,
                    null,
                    input.HomeNumber,
                    input.MobileNumber,
                    input.AltMobileNumber,
                    null,
                    input.AltEmailAddress,
                    input.DateOfBirth,
                    input.Gender);

                await Repository.InsertAsync(person);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(person);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public override async Task<PersonDto> UpdateAsync(PersonDto input)
        {
            var person = await Repository.GetAsync(input.Id);
            try
            {
                if (!string.IsNullOrWhiteSpace(input.FirstName) && !string.IsNullOrWhiteSpace(input.LastName))
                {
                    person.SetName(input.FirstName, input.LastName);
                }

                person.UpdateProfile(
                    input.IdentityNumber,
                    input.Title,
                    input.Biography,
                    null,
                    null,
                    input.HomeNumber,
                    input.MobileNumber,
                    input.AltMobileNumber,
                    null,
                    input.AltEmailAddress,
                    input.DateOfBirth,
                    input.Gender);

                await Repository.UpdateAsync(person);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(person);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
