using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Domains.Nodes;
using System;

namespace Farmru.IotMonitoring.Services.Nodes.Dto
{
    public class PagedNodeResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public Guid? FacilityId { get; set; }
        public DeviceOperationalStatus? DeviceStatus { get; set; }
        public DeviceHealthStatus? HealthStatus { get; set; }
        public bool? OnlineOnly { get; set; }
        public bool? OfflineOnly { get; set; }
        public string Sorting { get; set; }
    }
}
