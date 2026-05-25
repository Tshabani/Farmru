using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Services.Monitoring.Dto;
using System;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Monitoring
{
    public interface IMonitoringAppService : IApplicationService
    {
        Task<MonitoringDashboardDto> GetDashboard();
        Task<PagedResultDto<MonitoringExecutionHistoryDto>> GetExecutionHistory(PagedMonitoringHistoryRequestDto input);
        Task<MonitoringConfigurationDto> GetConfiguration(Guid? facilityId = null);
        Task<MonitoringConfigurationDto> UpdateConfiguration(UpdateMonitoringConfigurationInput input);
    }
}
