using Abp.Dependency;
using Farmru.IotMonitoring.Alerts;
using Farmru.IotMonitoring.Services.Alerts.Dto;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Web.Alerts
{
    public class AlertRealtimeNotifier : IAlertRealtimeNotifier, ITransientDependency
    {
        private readonly IHubContext<AlertNotificationHub> _hubContext;

        public AlertRealtimeNotifier(IHubContext<AlertNotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyAsync(AlertDto alert, string action)
        {
            if (alert == null)
            {
                return Task.CompletedTask;
            }

            var group = AlertNotificationHub.GetTenantGroupName(alert.TenantId);
            return _hubContext.Clients.Group(group).SendAsync("alertChanged", new { action, alert });
        }
    }
}
