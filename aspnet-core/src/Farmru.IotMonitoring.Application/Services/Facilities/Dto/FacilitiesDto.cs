using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Helpers;
using System;

namespace Farmru.IotMonitoring.Services.Facilities.Dto
{
    [AutoMap(typeof(Facility))]
    public class FacilitiesDto : EntityDto<Guid>
    {
        public string Name { get; set; }

    }
}


