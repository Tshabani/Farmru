using Abp.Domain.Services;
using Farmru.IotMonitoring.Domains.Nodes;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Domains.Geo.Services
{
    public interface IGeoFenceEvaluationService : IDomainService
    {
        Task EvaluateNodeLocationAsync(Node node, decimal latitude, decimal longitude);
        Task SyncGeoFencesForTenantAsync(int tenantId);
    }
}
