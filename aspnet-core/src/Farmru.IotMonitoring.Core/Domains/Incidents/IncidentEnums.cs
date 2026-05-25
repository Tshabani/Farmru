namespace Farmru.IotMonitoring.Domains.Incidents
{
    public enum IncidentStatus
    {
        Open = 0,
        Assigned = 1,
        InProgress = 2,
        WaitingOnParts = 3,
        Escalated = 4,
        Resolved = 5,
        Closed = 6,
        Cancelled = 7
    }

    public enum IncidentPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }

    public enum IncidentSlaStatus
    {
        OnTrack = 0,
        AtRisk = 1,
        Breached = 2,
        Met = 3,
        NotApplicable = 4
    }

    public enum IncidentTimelineEventType
    {
        Created = 0,
        StatusChanged = 1,
        Assigned = 2,
        Reassigned = 3,
        Acknowledged = 4,
        Comment = 5,
        Escalated = 6,
        AttachmentAdded = 7,
        Resolved = 8,
        Closed = 9,
        Cancelled = 10,
        Reopened = 11,
        SlaBreached = 12
    }
}
