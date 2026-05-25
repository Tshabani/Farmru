using System;

namespace Farmru.IotMonitoring.Domains.Nodes
{
    public partial class Node
    {
        public virtual TelemetryQualityStatus TelemetryQuality { get; private set; }
        public virtual DateTime? LastHealthEvaluatedAt { get; private set; }

        public virtual void ApplyOperationalEvaluation(
            int offlineThresholdMinutes,
            TelemetryQualityStatus telemetryQuality)
        {
            TelemetryQuality = telemetryQuality;
            LastHealthEvaluatedAt = DateTime.UtcNow;
            EvaluateHealth(offlineThresholdMinutes);
        }
    }
}
