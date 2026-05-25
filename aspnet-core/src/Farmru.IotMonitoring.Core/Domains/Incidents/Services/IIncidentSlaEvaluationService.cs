using Abp.Domain.Services;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Domains.Incidents.Services
{
    public interface IIncidentSlaEvaluationService : IDomainService
    {
        Task EvaluateTenantIncidentsAsync(int tenantId);
    }
}
