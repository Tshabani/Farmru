using Farmru.IotMonitoring.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Farmru.IotMonitoring.Services.Nodes.Dto
{
    public class AssignNodeToFacilityInput
    {
        [Required]
        public Guid NodeId { get; set; }

        public EntityWithDisplayNameDto<Guid?> Facility { get; set; }
    }
}
