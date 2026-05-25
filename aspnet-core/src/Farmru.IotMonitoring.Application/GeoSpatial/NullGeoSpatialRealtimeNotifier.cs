using Farmru.IotMonitoring.Services.GeoSpatial.Dto;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.GeoSpatial
{
    public class NullGeoSpatialRealtimeNotifier : IGeoSpatialRealtimeNotifier
    {
        public Task NotifyMapUpdateAsync(int tenantId, MapUpdateEventDto evt) => Task.CompletedTask;
    }
}
