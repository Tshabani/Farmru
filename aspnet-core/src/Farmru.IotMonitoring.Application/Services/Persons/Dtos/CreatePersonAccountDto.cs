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
    [AutoMapTo(typeof(User), typeof(Person))]
    public class CreatePersonAccountDto
    {
        [Required]
        [MinLength(5)]
        public virtual string UserName { get; set; }

        [Required]
        [MinLength(1)]
        public virtual string FirstName { get; set; }

        [Required]
        [MinLength(1)]
        public virtual string LastName { get; set; }
        public string IdentityNumber { get; set; }
        public RefListPersonTitle? Title { get; set; }
        public string HomeNumber { get; set; }
        public string MobileNumber { get; set; }
        public string MobileNumber2 { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public string Biography { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public RefListGender? Gender { get; set; }
    }
}
