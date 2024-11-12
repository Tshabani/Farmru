using Abp.Authorization;
using Abp.Domain.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Farmru.IotMonitoring.Services.Home;
using Farmru.IotMonitoring.Services.Home.Dto;
using Farmru.IotMonitoring.Authorization.Users;
using Farmru.IotMonitoring.Authorization.Roles;
using Farmru.IotMonitoring.Domains.Nodes;
using System;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains.Stats;
using Farmru.IotMonitoring.Repository;

namespace Farmru.IotMonitoring.Services.Home
{
    [AbpAuthorize(PermissionNames.Pages_Home)]
    public class HomeAppService : IHomeAppService
    {
        private readonly IRepository<User,long> _users;
        private readonly IRepository<Node, Guid> _nodes;
        private readonly IRepository<Role> _roles;
        private readonly INodeDataRepository _nodeDataRepository;

        public HomeAppService(IRepository<User,long> users,
            IRepository<Node, Guid> nodes,
            IRepository<Role> roles, INodeDataRepository nodeDataRepository
            )
        {
            _users = users;
            _roles = roles;
            _nodes = nodes;
            _nodeDataRepository = nodeDataRepository;
        }

        public async Task<AppStatisticsDto> GetAppStats()
        {
            var AppStatistics = new AppStatisticsDto();
            AppStatistics.TotalNumberOfRoles = _roles.GetAllList().Count(); 
            AppStatistics.TotalNumberOfUsers = _users.GetAllList().Count();
            AppStatistics.TotalNumberOfNodes = _nodes.GetAllList().Count();
            AppStatistics.TotalNumberOfActiveNodes = _nodes.GetAllList().Count() - 1;

            return AppStatistics;
        }

        public async Task<AverageNodeData> GetSensorData()
        { 
            return await _nodeDataRepository.GetAverageNodeDataAsync();
        }
    }
}
