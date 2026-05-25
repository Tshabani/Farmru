using System;
using System.ComponentModel.DataAnnotations;

namespace Farmru.IotMonitoring.Services.Alerts.Dto
{
    public class AcknowledgeAlertInput
    {
        [Required]
        public Guid AlertId { get; set; }
    }
}
