using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Farmru.IotMonitoring.Domains.Persons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Persons.Dtos
{
    [AutoMap(typeof(Person))]
    public class PersonDto : EntityDto<Guid>
    {
        [Required]
        [StringLength(AbpUserBase.MaxNameLength)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(AbpUserBase.MaxSurnameLength)]
        public string LastName { get; set; }

        public string IdentityNumber { get; set; }

        public RefListPersonTitle? Title { get; set; }

        public string HomeNumber { get; set; }

        public string MobileNumber { get; set; }

        public string AltMobileNumber { get; set; }

        public string AltEmailAddress { get; set; }

        public string Biography { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public RefListGender? Gender { get; set; }
    }
}
