using Farmru.IotMonitoring.Services.Incidents.Dto;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Incidents
{
    public class NullIncidentRealtimeNotifier : IIncidentRealtimeNotifier
    {
        public Task NotifyIncidentChangedAsync(IncidentDto incident, string action) => Task.CompletedTask;
    }
}
