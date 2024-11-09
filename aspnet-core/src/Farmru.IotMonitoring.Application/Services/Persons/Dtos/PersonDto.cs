using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Farmru.IotMonitoring.Domains.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Persons.Dtos
{
    [AutoMap(typeof(Person))]
    public class PersonDto : EntityDto<Guid>
    {
        public string IdentityNumber { get; set; }
        public RefListPersonTitle? Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Initials { get; set; }
        public string CustomShortName { get; set; }
        public string HomeNumber { get; set; }
        public string MobileNumber1 { get; set; }
        public string MobileNumber2 { get; set; }
        public string EmailAddress1 { get; set; }
        public string EmailAddress2 { get; set; }
        public string Biography { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public RefListGender? Gender { get; set; }
        public bool IsActive { get; set; }
        public string FullName { get; protected set; }
    }
}
