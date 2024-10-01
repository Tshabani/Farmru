using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Farmru.IotMonitoring.Authorization.Roles;
using Farmru.IotMonitoring.Authorization.Users;
using Farmru.IotMonitoring.MultiTenancy;
using Farmru.IotMonitoring.Domains.Nodes;

namespace Farmru.IotMonitoring.EntityFrameworkCore
{
    public class IotMonitoringDbContext : AbpZeroDbContext<Tenant, Role, User, IotMonitoringDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<NodeData> NodeDatas { get; set; }
        public DbSet<Node> Nodes { get; set; }
        
        public IotMonitoringDbContext(DbContextOptions<IotMonitoringDbContext> options)
            : base(options)
        {
        }
    }
}
