﻿using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Authorization.Users;
using Farmru.IotMonitoring.Domains.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Domains.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    public class TaskManagement : FullAuditedEntity<Guid>
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual Person AssignedTo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Person AssignedBy { get; set; }
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
