using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Farmru.IotMonitoring.Authorization.Users;
using Farmru.IotMonitoring.Domains.Organisations;
using Farmru.IotMonitoring.Domains.Tasks;
using Farmru.IotMonitoring.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Tasks.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(TaskManagement))]
    public class TaskManagementDto : EntityDto<Guid>
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual EntityWithDisplayNameDto<Guid?> AssignedTo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual EntityWithDisplayNameDto<Guid?> AssignedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual RefListTaskStatus? Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Description { get; set; }
    }
}
