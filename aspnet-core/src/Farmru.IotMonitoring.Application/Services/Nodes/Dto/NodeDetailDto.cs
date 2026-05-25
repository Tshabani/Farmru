using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Domains.Nodes;
using Farmru.IotMonitoring.Helpers;
using System;
using System.Collections.Generic;

namespace Farmru.IotMonitoring.Services.Nodes.Dto
{
    public class NodeDetailDto : EntityDto<Guid>
    {
        public string SerialNumber { get; set; }
        public string DisplayName { get; set; }
        public EntityWithDisplayNameDto<Guid?> Facility { get; set; }
        public bool IsActive { get; set; }
        public DeviceOperationalStatus DeviceStatus { get; set; }
        public DeviceHealthStatus HealthStatus { get; set; }
        public DateTime? LastSeenAt { get; set; }
        public decimal? BatteryLevel { get; set; }
        public int? SignalStrength { get; set; }
        public string FirmwareVersion { get; set; }
        public string Notes { get; set; }
        public bool IsOnline { get; set; }
        public NodeTelemetrySummaryDto TelemetrySummary { get; set; }
        public List<NodeReplacementHistoryDto> ReplacementHistory { get; set; }
    }
}
