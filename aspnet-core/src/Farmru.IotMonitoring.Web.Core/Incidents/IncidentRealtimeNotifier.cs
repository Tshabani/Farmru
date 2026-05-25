using Abp.Dependency;
using Farmru.IotMonitoring.Incidents;
using Farmru.IotMonitoring.Services.Incidents.Dto;
using Farmru.IotMonitoring.Web.Alerts;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Web.Incidents
{
    public class IncidentRealtimeNotifier : IIncidentRealtimeNotifier, ITransientDependency
    {
        private readonly IHubContext<AlertNotificationHub> _hubContext;

        public IncidentRealtimeNotifier(IHubContext<AlertNotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyIncidentChangedAsync(IncidentDto incident, string action)
        {
            if (incident == null)
            {
                return Task.CompletedTask;
            }

            var group = AlertNotificationHub.GetTenantGroupName(incident.TenantId);
            return _hubContext.Clients.Group(group).SendAsync("incidentChanged", new { action, incident });
        }
    }
}
