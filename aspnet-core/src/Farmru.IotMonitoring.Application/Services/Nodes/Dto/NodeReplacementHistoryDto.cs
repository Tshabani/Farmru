using Abp.Application.Services.Dto;
using System;

namespace Farmru.IotMonitoring.Services.Nodes.Dto
{
    public class NodeReplacementHistoryDto : EntityDto<Guid>
    {
        public Guid NodeId { get; set; }
        public string OldSerialNumber { get; set; }
        public string NewSerialNumber { get; set; }
        public DateTime ReplacedAt { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }
    }
}
