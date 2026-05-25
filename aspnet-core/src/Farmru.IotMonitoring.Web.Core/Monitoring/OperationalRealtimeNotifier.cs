using Abp.Dependency;
using Farmru.IotMonitoring.Monitoring;
using Farmru.IotMonitoring.Services.Monitoring.Dto;
using Farmru.IotMonitoring.Web.Alerts;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Web.Monitoring
{
    public class OperationalRealtimeNotifier : IOperationalRealtimeNotifier, ITransientDependency
    {
        private readonly IHubContext<AlertNotificationHub> _hubContext;

        public OperationalRealtimeNotifier(IHubContext<AlertNotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyMonitoringEventAsync(int tenantId, MonitoringEventDto evt)
        {
            var group = AlertNotificationHub.GetTenantGroupName(tenantId);
            return _hubContext.Clients.Group(group).SendAsync("monitoringEvent", evt);
        }

        public Task NotifyExecutionSummaryAsync(int tenantId, MonitoringExecutionSummaryDto summary)
        {
            var group = AlertNotificationHub.GetTenantGroupName(tenantId);
            return _hubContext.Clients.Group(group).SendAsync("monitoringSummary", summary);
        }
    }
}
