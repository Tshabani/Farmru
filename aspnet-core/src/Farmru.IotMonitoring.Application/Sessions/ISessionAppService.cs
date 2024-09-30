using System.Threading.Tasks;
using Abp.Application.Services;
using Farmru.IotMonitoring.Sessions.Dto;

namespace Farmru.IotMonitoring.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
