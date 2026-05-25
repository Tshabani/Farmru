using Abp.AspNetCore.SignalR.Hubs;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Web.Alerts
{
    [Authorize]
    public class AlertNotificationHub : AbpHubBase
    {
        public async Task JoinTenantAlerts()
        {
            if (AbpSession.TenantId.HasValue)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GetTenantGroupName(AbpSession.TenantId.Value));
            }
        }

        public static string GetTenantGroupName(int tenantId) => $"alerts_tenant_{tenantId}";
    }
}
