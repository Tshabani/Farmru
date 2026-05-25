using Farmru.IotMonitoring.Services.GeoSpatial.Dto;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.GeoSpatial
{
    public interface IGeoSpatialRealtimeNotifier
    {
        Task NotifyMapUpdateAsync(int tenantId, MapUpdateEventDto evt);
    }
}
