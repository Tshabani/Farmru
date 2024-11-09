using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Farmru.IotMonitoring.Domains.Organisations;
using Farmru.IotMonitoring.Domains.Tasks;
using Farmru.IotMonitoring.Services.Organisations.Dto;
using Farmru.IotMonitoring.Services.Tasks.Dto;
using Farmru.IotMonitoring.Users.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Tasks
{
    /// <summary>
    /// 
    /// </summary>
    [AbpAuthorize()]
    public class TaskManagementAppService : AsyncCrudAppService<TaskManagement, TaskManagementDto, Guid, PagedUserResultRequestDto, TaskManagementDto, TaskManagementDto>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public TaskManagementAppService(IRepository<TaskManagement, Guid> repository) : base(repository)
        {
                
        }
    }
}
