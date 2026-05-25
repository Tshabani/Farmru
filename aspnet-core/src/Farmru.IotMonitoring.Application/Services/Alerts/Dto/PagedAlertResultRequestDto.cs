using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Domains.Alerts;
using System;

namespace Farmru.IotMonitoring.Services.Alerts.Dto
{
    public class PagedAlertResultRequestDto : PagedResultRequestDto
    {
        public AlertSeverity? Severity { get; set; }
        public AlertType? AlertType { get; set; }
        public Guid? FacilityId { get; set; }
        public Guid? NodeId { get; set; }
        public bool? ActiveOnly { get; set; }
        public bool? CriticalOnly { get; set; }
        public bool? UnacknowledgedOnly { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Sorting { get; set; }
    }
}
