using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Persons.Dtos
{
    [AutoMap(typeof(Person))]
    public class PeopleDto : EntityDto<Guid>
    {
        public string FullName { get; set; }
    }
}
