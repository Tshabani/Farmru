using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Persons;
using System;

namespace Farmru.IotMonitoring.Domains.Incidents
{
    public partial class Incident
    {
        public virtual void Assign(Person assignee, long assignedByUserId, string teamName = null, string dispatchNotes = null)
        {
            if (IsTerminal())
            {
                throw new DomainRuleException("Cannot assign a closed incident.");
            }

            if (assignee == null)
            {
                throw new DomainRuleException("Assignee is required.");
            }

            foreach (var active in Assignments)
            {
                if (active.IsActive)
                {
                    active.Deactivate();
                }
            }

            var assignment = IncidentAssignment.Create(TenantId, this, assignee, assignedByUserId, teamName, dispatchNotes);
            Assignments.Add(assignment);
            AssignedTo = assignee;
            AssignedAt = DateTime.UtcNow;
            AssignedTeamName = teamName?.Trim();
            TransitionTo(IncidentStatus.Assigned);
        }

        public virtual void Acknowledge(long userId)
        {
            if (IsTerminal())
            {
                throw new DomainRuleException("Cannot acknowledge a terminal incident.");
            }

            AcknowledgedAt ??= DateTime.UtcNow;
            FirstResponseAt ??= DateTime.UtcNow;
            RecordTimeline(IncidentTimelineEventType.Acknowledged, "Assignment acknowledged", null, userId);
        }

        public virtual void StartWork()
        {
            EnsureNotTerminal();
            TransitionTo(IncidentStatus.InProgress);
            FirstResponseAt ??= DateTime.UtcNow;
        }

        public virtual void MarkWaitingOnParts()
        {
            EnsureNotTerminal();
            TransitionTo(IncidentStatus.WaitingOnParts);
        }

        public virtual void Escalate(string reason = null)
        {
            EnsureNotTerminal();
            EscalationLevel++;
            IsEscalated = true;
            TransitionTo(IncidentStatus.Escalated);
            RecordTimeline(IncidentTimelineEventType.Escalated, "Incident escalated", reason);
        }

        public virtual void Resolve(string resolutionNotes, long? userId = null)
        {
            EnsureNotTerminal();
            ResolutionNotes = string.IsNullOrWhiteSpace(resolutionNotes) ? null : resolutionNotes.Trim();
            ResolvedDate = DateTime.UtcNow;
            TransitionTo(IncidentStatus.Resolved);
            EvaluateSlaMet();
            RecordTimeline(IncidentTimelineEventType.Resolved, "Incident resolved", ResolutionNotes, userId);
        }

        public virtual void Close()
        {
            if (Status != IncidentStatus.Resolved && Status != IncidentStatus.Cancelled)
            {
                throw new DomainRuleException("Only resolved or cancelled incidents can be closed.");
            }

            ClosedDate = DateTime.UtcNow;
            TransitionTo(IncidentStatus.Closed);
        }

        public virtual void Cancel(string reason)
        {
            EnsureNotTerminal();
            TransitionTo(IncidentStatus.Cancelled);
            SlaStatus = IncidentSlaStatus.NotApplicable;
            RecordTimeline(IncidentTimelineEventType.Cancelled, "Incident cancelled", reason);
        }

        public virtual void Reopen()
        {
            if (Status != IncidentStatus.Resolved && Status != IncidentStatus.Closed && Status != IncidentStatus.Cancelled)
            {
                throw new DomainRuleException("Only terminal incidents can be reopened.");
            }

            ResolvedDate = null;
            ClosedDate = null;
            IsEscalated = false;
            SlaStatus = IncidentSlaStatus.OnTrack;
            ApplySlaDeadlines();
            TransitionTo(IncidentStatus.Open);
            RecordTimeline(IncidentTimelineEventType.Reopened, "Incident reopened", null);
        }

        public virtual void AddComment(string comment, long? userId = null)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new DomainRuleException("Comment cannot be empty.");
            }

            RecordTimeline(IncidentTimelineEventType.Comment, "Comment added", comment.Trim(), userId);
        }

        public virtual IncidentAttachment AddAttachment(string fileName, string contentType, string storagePath, long size, string caption = null)
        {
            var attachment = IncidentAttachment.Create(TenantId, this, fileName, contentType, storagePath, size, caption);
            Attachments.Add(attachment);
            RecordTimeline(IncidentTimelineEventType.AttachmentAdded, "Attachment uploaded", fileName);
            return attachment;
        }

        public virtual void RecordTimeline(IncidentTimelineEventType type, string title, string description, long? userId = null)
        {
            var evt = IncidentTimelineEvent.Record(TenantId, this, type, title, description);
            if (userId.HasValue)
            {
                evt.CreatorUserId = userId;
            }

            Timeline.Add(evt);
        }

        internal virtual void TransitionTo(IncidentStatus newStatus)
        {
            if (Status == newStatus)
            {
                return;
            }

            var old = Status;
            Status = newStatus;
            RecordTimeline(IncidentTimelineEventType.StatusChanged, $"Status changed to {newStatus}", $"From {old}");
        }

        internal virtual void EvaluateSlaMet()
        {
            if (FirstResponseAt.HasValue && FirstResponseAt <= ResponseDueAt)
            {
                SlaResponseBreached = false;
            }

            if (ResolvedDate.HasValue && ResolvedDate <= ResolutionDueAt)
            {
                SlaResolutionBreached = false;
                SlaStatus = IncidentSlaStatus.Met;
            }
        }

        public virtual void MarkSlaBreached(bool responseBreached, bool resolutionBreached)
        {
            if (responseBreached)
            {
                SlaResponseBreached = true;
            }

            if (resolutionBreached)
            {
                SlaResolutionBreached = true;
            }

            SlaStatus = IncidentSlaStatus.Breached;
            RecordTimeline(IncidentTimelineEventType.SlaBreached, "SLA breached",
                $"Response: {responseBreached}, Resolution: {resolutionBreached}");
        }

        public virtual void UpdateSlaRisk()
        {
            if (IsTerminal())
            {
                return;
            }

            var now = DateTime.UtcNow;
            if (SlaStatus == IncidentSlaStatus.Breached)
            {
                return;
            }

            var responseWindow = (ResponseDueAt - CreatedDate).TotalMinutes;
            var resolutionWindow = (ResolutionDueAt - CreatedDate).TotalMinutes;
            var responseElapsed = (now - CreatedDate).TotalMinutes;
            var resolutionElapsed = (now - CreatedDate).TotalMinutes;

            if (!FirstResponseAt.HasValue && now > ResponseDueAt)
            {
                MarkSlaBreached(true, false);
                return;
            }

            if (!ResolvedDate.HasValue && now > ResolutionDueAt)
            {
                MarkSlaBreached(SlaResponseBreached, true);
                return;
            }

            if ((!FirstResponseAt.HasValue && responseWindow > 0 && responseElapsed / responseWindow * 100 >= IncidentSlaDefaults.AtRiskThresholdPercent) ||
                (!ResolvedDate.HasValue && resolutionWindow > 0 && resolutionElapsed / resolutionWindow * 100 >= IncidentSlaDefaults.AtRiskThresholdPercent))
            {
                SlaStatus = IncidentSlaStatus.AtRisk;
            }
            else
            {
                SlaStatus = IncidentSlaStatus.OnTrack;
            }
        }

        private void EnsureNotTerminal()
        {
            if (IsTerminal())
            {
                throw new DomainRuleException("Incident is in a terminal state.");
            }
        }
    }
}
