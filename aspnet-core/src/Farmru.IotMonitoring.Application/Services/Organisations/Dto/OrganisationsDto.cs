using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Farmru.IotMonitoring.Domains.Organisations;
using Farmru.IotMonitoring.Domains.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Organisations.Dto
{
    [AutoMap(typeof(Organisation))]
    public class OrganisationsDto : EntityDto<Guid>
    {
        public string Name { get; set; }
    }
}
