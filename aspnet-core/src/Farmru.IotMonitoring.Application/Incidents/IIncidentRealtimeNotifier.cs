using Farmru.IotMonitoring.Services.Incidents.Dto;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Incidents
{
    public interface IIncidentRealtimeNotifier
    {
        Task NotifyIncidentChangedAsync(IncidentDto incident, string action);
    }
}
