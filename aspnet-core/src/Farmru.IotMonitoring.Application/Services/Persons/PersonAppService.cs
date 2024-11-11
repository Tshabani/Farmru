using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.Runtime.Validation;
using AutoMapper.Internal.Mappers;
using Farmru.IotMonitoring.Authorization.Users;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Services.Persons.Dtos;
using Farmru.IotMonitoring.Users.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Persons
{
    [AbpAuthorize]
    public class PersonAppService : AsyncCrudAppService<Person, PersonDto, Guid, PagedUserResultRequestDto, CreatePersonDto, PersonDto>
    {
        private readonly UserManager _userManager;
        private readonly IRepository<User, long> _userRepository;
        public PersonAppService(IRepository<Person, Guid> repository, UserManager userManager, IRepository<User, long> userRepository) : base(repository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }
         
        public override async Task<PersonDto> CreateAsync(CreatePersonDto input)
        {
            // Performing additional validations
            /* var validationResults = new List<ValidationResult>();

             if (string.IsNullOrWhiteSpace(input.FirstName))
                 validationResults.Add(new ValidationResult("First Name is mandatory"));
             if (string.IsNullOrWhiteSpace(input.LastName))
                 validationResults.Add(new ValidationResult("Last Name is mandatory"));

             if (validationResults.Any())
                 throw new AbpValidationException("Please correct the errors and try again", validationResults);

             //// Creating User Account to enable login into the application
             User user = await _userManager.CreateUser(
               input.UserName,
               true,
               input.Password,
               input.PasswordConfirmation,
               input.FirstName,
               input.LastName,
               input.MobileNumber,
               input.EmailAddress);

             // Creating Person entity
             var person = ObjectMapper.Map<Person>(input);
             // manual map for now
             person.EmailAddress1 = input.EmailAddress;
             person.MobileNumber1 = input.MobileNumber;
             person.User = user;

             await Repository.InsertAsync(person);

             CurrentUnitOfWork.SaveChanges();
             return ObjectMapper.Map<PersonDto>(person); */
            return null;
        }

        public async Task<PersonDto> GetCurrentPerson()
        {
            var person = await GetCurrentPersonAsync();
            return ObjectMapper.Map<PersonDto>(person);
        }
        protected virtual async Task<Person> GetCurrentPersonAsync()
        {
            var person = await Repository.GetAll().FirstOrDefaultAsync(p => p.User.Id == AbpSession.GetUserId());
            return person;
        }

        [HttpPut()]
        public async Task<bool> Activate(Guid personId)
        {
            await UpdateUserStatus(personId, isActive: true);
            return true;
        }

        [HttpDelete()]
        public async Task<bool> Deactivate(Guid personId)
        {
            await UpdateUserStatus(personId, isActive: false);
            return true;
        }

        private async Task UpdateUserStatus(Guid personId, bool isActive)
        {
            var person = await Repository.GetAsync(personId);
            if (person == null) throw new KeyNotFoundException($"Person with ID {personId} not found.");

            var user = person.User;
            if (user == null) throw new InvalidOperationException("Person does not have an associated user account.");

            user.IsActive = isActive;
            await _userRepository.InsertOrUpdateAsync(user);
        }
    }
}
