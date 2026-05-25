using Abp.Domain.Entities;
using System;

namespace Farmru.IotMonitoring.Domains.Monitoring
{
    public class MonitoringExecutionHistory : Entity<Guid>, IMayHaveTenant
    {
        protected MonitoringExecutionHistory()
        {
        }

        public int? TenantId { get; set; }
        public virtual MonitoringJobType JobType { get; private set; }
        public virtual DateTime StartedAt { get; private set; }
        public virtual DateTime? CompletedAt { get; private set; }
        public virtual long DurationMs { get; private set; }
        public virtual bool Succeeded { get; private set; }
        public virtual string ErrorMessage { get; private set; }
        public virtual int AlertsGenerated { get; private set; }
        public virtual int AlertsResolved { get; private set; }
        public virtual int DevicesEvaluated { get; private set; }
        public virtual int EscalationsPerformed { get; private set; }
        public virtual string SummaryJson { get; private set; }

        public static MonitoringExecutionHistory Start(int? tenantId, MonitoringJobType jobType)
        {
            return new MonitoringExecutionHistory
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                JobType = jobType,
                StartedAt = DateTime.UtcNow,
                Succeeded = false
            };
        }

        public virtual void Complete(
            bool succeeded,
            int alertsGenerated,
            int alertsResolved,
            int devicesEvaluated,
            int escalationsPerformed,
            string summaryJson = null,
            string errorMessage = null)
        {
            CompletedAt = DateTime.UtcNow;
            DurationMs = (long)(CompletedAt.Value - StartedAt).TotalMilliseconds;
            Succeeded = succeeded;
            AlertsGenerated = alertsGenerated;
            AlertsResolved = alertsResolved;
            DevicesEvaluated = devicesEvaluated;
            EscalationsPerformed = escalationsPerformed;
            SummaryJson = summaryJson;
            ErrorMessage = errorMessage;
        }
    }
}
