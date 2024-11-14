using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Farmru.IotMonitoring.Authorization.Users;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Organisations;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Users.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Facilities.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(Facility))]
    public class FacilityDto : EntityDto<Guid>
    {
        public string Name { get; set; } 
        public string Description { get; set; }
        public string Address { get; set; }
        public EntityWithDisplayNameDto<Guid?>? PrimaryContact { get; set; }
        public EntityWithDisplayNameDto<Guid?>? OwnerOrganisation { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Altitude { get; set; }

    }
}
