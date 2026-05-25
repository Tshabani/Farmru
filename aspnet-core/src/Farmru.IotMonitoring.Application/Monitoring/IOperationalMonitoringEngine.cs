using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Monitoring
{
    public interface IOperationalMonitoringEngine
    {
        Task RunFullMonitoringCycleAsync();
    }
}
