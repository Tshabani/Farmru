using Farmru.IotMonitoring.Services.Monitoring.Dto;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Monitoring
{
    public interface IOperationalRealtimeNotifier
    {
        Task NotifyMonitoringEventAsync(int tenantId, MonitoringEventDto evt);
        Task NotifyExecutionSummaryAsync(int tenantId, MonitoringExecutionSummaryDto summary);
    }
}
