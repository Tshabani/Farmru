using Farmru.IotMonitoring.Services.Alerts.Dto;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Alerts
{
    public interface IAlertRealtimeNotifier
    {
        Task NotifyAsync(AlertDto alert, string action);
    }
}
