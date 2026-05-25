using Abp.Domain.Services;
using Farmru.IotMonitoring.Domains.Nodes;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Domains.Alerts.Services
{
    public interface ITelemetryAlertEvaluationService : IDomainService
    {
        Task EvaluateAfterTelemetryAsync(Node node, NodeData nodeData);
        Task<MonitoringEvaluationResult> RunOfflineMonitoringForTenantAsync(int tenantId);
        Task<MonitoringEvaluationResult> RunTelemetryHealthMonitoringForTenantAsync(int tenantId);
        Task<MonitoringEvaluationResult> RunAlertEscalationForTenantAsync(int tenantId);
    }

    public class MonitoringEvaluationResult
    {
        public int AlertsGenerated { get; set; }
        public int AlertsResolved { get; set; }
        public int DevicesEvaluated { get; set; }
        public int EscalationsPerformed { get; set; }
    }
}
