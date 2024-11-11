using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Farmru.IotMonitoring.Authorization.Users;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Services.Persons.Dtos;

namespace Farmru.IotMonitoring.Users.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserDto : EntityDto<long>
    {
        [Required]
        [StringLength(AbpUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string PasswordConfirmation { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public DateTime CreationTime { get; set; }

        public string[] RoleNames { get; set; }

        public PersonDto Person { get; set; }

        public UserDto()
        {
            Person = new PersonDto();
        }
    }
}
