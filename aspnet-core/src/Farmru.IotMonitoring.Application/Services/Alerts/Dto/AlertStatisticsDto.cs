using System.Collections.Generic;

namespace Farmru.IotMonitoring.Services.Alerts.Dto
{
    public class AlertStatisticsDto
    {
        public int TotalActive { get; set; }
        public int TotalCritical { get; set; }
        public int TotalWarning { get; set; }
        public int TotalInfo { get; set; }
        public int TotalUnacknowledged { get; set; }
        public int TotalResolvedToday { get; set; }
        public List<AlertCountBySeverityDto> BySeverity { get; set; } = new();
        public List<AlertCountByTypeDto> ByType { get; set; } = new();
        public List<AlertFacilitySummaryDto> ByFacility { get; set; } = new();
    }

    public class AlertCountBySeverityDto
    {
        public int Severity { get; set; }
        public int Count { get; set; }
    }

    public class AlertCountByTypeDto
    {
        public int AlertType { get; set; }
        public int Count { get; set; }
    }

    public class AlertFacilitySummaryDto
    {
        public string FacilityName { get; set; }
        public int ActiveCount { get; set; }
        public int CriticalCount { get; set; }
    }
}
