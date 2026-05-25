using System;

namespace Farmru.IotMonitoring.Domains.Incidents
{
    public static class IncidentSlaDefaults
    {
        public static (int ResponseMinutes, int ResolutionMinutes) ForPriority(IncidentPriority priority) =>
            priority switch
            {
                IncidentPriority.Critical => (30, 240),
                IncidentPriority.High => (120, 1440),
                IncidentPriority.Medium => (480, 4320),
                _ => (1440, 10080)
            };

        public static int AtRiskThresholdPercent => 80;
    }
}
