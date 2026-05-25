using Abp.Dependency;
using Farmru.IotMonitoring.GeoSpatial;
using Farmru.IotMonitoring.Services.GeoSpatial.Dto;
using Farmru.IotMonitoring.Web.Alerts;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Web.GeoSpatial
{
    public class GeoSpatialRealtimeNotifier : IGeoSpatialRealtimeNotifier, ITransientDependency
    {
        private readonly IHubContext<AlertNotificationHub> _hubContext;

        public GeoSpatialRealtimeNotifier(IHubContext<AlertNotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyMapUpdateAsync(int tenantId, MapUpdateEventDto evt)
        {
            var group = AlertNotificationHub.GetTenantGroupName(tenantId);
            return _hubContext.Clients.Group(group).SendAsync("mapUpdate", evt);
        }
    }
}
