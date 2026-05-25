using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Farmru.IotMonitoring.Domains.Incidents;
using Farmru.IotMonitoring.Domains.Incidents.Services;
using Farmru.IotMonitoring.Incidents;
using Farmru.IotMonitoring.Services.Incidents.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Incidents
{
    public class IncidentSlaEvaluationService : DomainService, IIncidentSlaEvaluationService
    {
        private readonly IRepository<Incident, Guid> _incidentRepository;
        private readonly IIncidentRealtimeNotifier _realtimeNotifier;

        public IncidentSlaEvaluationService(
            IRepository<Incident, Guid> incidentRepository,
            IIncidentRealtimeNotifier realtimeNotifier)
        {
            _incidentRepository = incidentRepository;
            _realtimeNotifier = realtimeNotifier;
        }

        public async Task EvaluateTenantIncidentsAsync(int tenantId)
        {
            var openIncidents = await _incidentRepository.GetAll()
                .Where(i => i.TenantId == tenantId)
                .Where(i => i.Status != IncidentStatus.Resolved &&
                            i.Status != IncidentStatus.Closed &&
                            i.Status != IncidentStatus.Cancelled)
                .ToListAsync();

            foreach (var incident in openIncidents)
            {
                var previousSla = incident.SlaStatus;
                incident.UpdateSlaRisk();

                if (incident.SlaStatus == IncidentSlaStatus.Breached && previousSla != IncidentSlaStatus.Breached)
                {
                    if (!incident.IsEscalated && incident.Status != IncidentStatus.Escalated)
                    {
                        incident.Escalate("Automatic escalation due to SLA breach");
                    }

                    await _incidentRepository.UpdateAsync(incident);
                    await _realtimeNotifier.NotifyIncidentChangedAsync(MapBrief(incident), "slaBreached");
                }
                else if (incident.SlaStatus != previousSla)
                {
                    await _incidentRepository.UpdateAsync(incident);
                }
            }
        }

        private static IncidentDto MapBrief(Incident incident) =>
            new IncidentDto
            {
                Id = incident.Id,
                TenantId = incident.TenantId,
                Title = incident.Title,
                Status = incident.Status,
                Priority = incident.Priority,
                SlaStatus = incident.SlaStatus,
                SlaResponseBreached = incident.SlaResponseBreached,
                SlaResolutionBreached = incident.SlaResolutionBreached
            };
    }
}
