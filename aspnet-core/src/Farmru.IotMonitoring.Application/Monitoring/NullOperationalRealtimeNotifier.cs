using Farmru.IotMonitoring.Services.Monitoring.Dto;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Monitoring
{
    public class NullOperationalRealtimeNotifier : IOperationalRealtimeNotifier
    {
        public Task NotifyMonitoringEventAsync(int tenantId, MonitoringEventDto evt) => Task.CompletedTask;
        public Task NotifyExecutionSummaryAsync(int tenantId, MonitoringExecutionSummaryDto summary) => Task.CompletedTask;
    }
}
