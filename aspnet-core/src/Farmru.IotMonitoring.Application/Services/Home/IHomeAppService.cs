using Abp.Application.Services;
using System.Threading.Tasks;
using Farmru.IotMonitoring.Services.Home.Dto;

namespace Farmru.IotMonitoring.Services.Home
{
    public interface IHomeAppService : IApplicationService
    {
        Task<AppStatisticsDto> GetAppStats();
    }
}
