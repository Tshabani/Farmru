using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Services.Alerts.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Alerts
{
    public interface IAlertAppService : IApplicationService
    {
        Task<PagedResultDto<AlertDto>> GetAlerts(PagedAlertResultRequestDto input);
        Task<List<AlertDto>> GetActiveAlerts();
        Task<List<AlertDto>> GetCriticalAlerts();
        Task<AlertDto> GetAlert(EntityDto<Guid> input);
        Task<List<AlertDto>> GetAlertsByNode(EntityDto<Guid> input);
        Task<AlertDto> Acknowledge(AcknowledgeAlertInput input);
        Task<AlertDto> Resolve(ResolveAlertInput input);
        Task<AlertStatisticsDto> GetStatistics();
        Task<AlertThresholdConfigurationDto> GetThresholdConfiguration(Guid? facilityId = null);
        Task<AlertThresholdConfigurationDto> UpdateThresholdConfiguration(UpdateAlertThresholdConfigurationInput input);
    }
}
