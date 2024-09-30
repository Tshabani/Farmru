using Abp.Application.Services;
using Farmru.IotMonitoring.MultiTenancy.Dto;

namespace Farmru.IotMonitoring.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

