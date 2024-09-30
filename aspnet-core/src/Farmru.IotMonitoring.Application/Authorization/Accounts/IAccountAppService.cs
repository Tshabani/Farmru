using System.Threading.Tasks;
using Abp.Application.Services;
using Farmru.IotMonitoring.Authorization.Accounts.Dto;

namespace Farmru.IotMonitoring.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
