using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Farmru.IotMonitoring.MultiTenancy;

namespace Farmru.IotMonitoring.Sessions.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantLoginInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}
