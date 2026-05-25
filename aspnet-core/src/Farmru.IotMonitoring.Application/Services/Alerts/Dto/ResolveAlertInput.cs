using System;
using System.ComponentModel.DataAnnotations;

namespace Farmru.IotMonitoring.Services.Alerts.Dto
{
    public class ResolveAlertInput
    {
        [Required]
        public Guid AlertId { get; set; }

        [StringLength(2000)]
        public string ResolutionNotes { get; set; }
    }
}
