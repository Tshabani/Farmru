using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Helpers
{
    public class EntityWithDisplayNameDto<TPrimaryKey> : EntityDto<TPrimaryKey>
    {
        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public EntityWithDisplayNameDto()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        public EntityWithDisplayNameDto(TPrimaryKey id, string displayText)
        {
            Id = id;
            DisplayText = displayText;
        }

        /// <summary>
        /// Entity display name
        /// </summary>
        public string DisplayText { get; set; }
    }
}
