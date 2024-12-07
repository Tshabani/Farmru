using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Farmru.IotMonitoring.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.NodeData.Dto
{
    [AutoMap(typeof(Domains.Nodes.NodeData))]
    public class PagedNodeDataResultRequestDto :PagedResultRequestDto
    {    
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PredefinedPeriod { get; set; } // E.g., "Today", "Week", "Month", "Year", or "Custom"
    }
}
