using System;
using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using Farmru.IotMonitoring.Authorization.Users;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Services.Persons.Dtos;

namespace Farmru.IotMonitoring.Users.Dto
{
    [AutoMapTo(typeof(User))]
    public class CreateUserDto : IShouldNormalize
    {
        [Required]
        [MinLength(5)]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string PasswordConfirmation { get; set; }

        public bool IsActive { get; set; }

        public string[] RoleNames { get; set; }

        public PersonDto Person { get; set; }

        public void Normalize()
        {
            if (RoleNames == null)
            {
                RoleNames = new string[0];
            }
        }
        public CreateUserDto()
        {
            Person = new PersonDto();
        }
    }
}
