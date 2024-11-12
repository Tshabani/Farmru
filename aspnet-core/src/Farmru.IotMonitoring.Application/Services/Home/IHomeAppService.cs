using Abp.Application.Services;
using System.Threading.Tasks;
using Farmru.IotMonitoring.Services.Home.Dto;
using Farmru.IotMonitoring.Domains.Stats;

namespace Farmru.IotMonitoring.Services.Home
{
    public interface IHomeAppService : IApplicationService
    {
        Task<AppStatisticsDto> GetAppStats();
        Task<AverageNodeData> GetSensorData();
    }
}
