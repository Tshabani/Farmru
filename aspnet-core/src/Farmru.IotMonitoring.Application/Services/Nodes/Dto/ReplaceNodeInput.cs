using System;
using System.ComponentModel.DataAnnotations;

namespace Farmru.IotMonitoring.Services.Nodes.Dto
{
    public class ReplaceNodeInput
    {
        [Required]
        public Guid NodeId { get; set; }

        [Required]
        [StringLength(128)]
        public string NewSerialNumber { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }
    }
}
