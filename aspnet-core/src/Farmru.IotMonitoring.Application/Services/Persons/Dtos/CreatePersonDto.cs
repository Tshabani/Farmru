using Abp.Authorization.Users;
using Abp.AutoMapper;
using Farmru.IotMonitoring.Authorization.Users;
using Farmru.IotMonitoring.Domains.Persons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Persons.Dtos
{
    [AutoMapTo(typeof(Person))]
    public class CreatePersonDto
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
