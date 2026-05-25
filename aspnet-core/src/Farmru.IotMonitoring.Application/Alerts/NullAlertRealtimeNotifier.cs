using Farmru.IotMonitoring.Services.Alerts.Dto;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Alerts
{
    public class NullAlertRealtimeNotifier : IAlertRealtimeNotifier
    {
        public Task NotifyAsync(AlertDto alert, string action) => Task.CompletedTask;
    }
}
