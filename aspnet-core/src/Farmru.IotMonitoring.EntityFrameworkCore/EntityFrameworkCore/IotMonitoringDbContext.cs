using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Farmru.IotMonitoring.Authorization.Roles;
using Farmru.IotMonitoring.Authorization.Users;
using Farmru.IotMonitoring.MultiTenancy;
using Farmru.IotMonitoring.Domains.Nodes;
using Farmru.IotMonitoring.Domains.Tasks;
using Farmru.IotMonitoring.Domains.Organisations;
using Farmru.IotMonitoring.Domains.Incidents;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Persons;

namespace Farmru.IotMonitoring.EntityFrameworkCore
{
    public class IotMonitoringDbContext : AbpZeroDbContext<Tenant, Role, User, IotMonitoringDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<NodeData> NodeDatas { get; set; }
        public DbSet<Node> Nodes { get; set; }
        public DbSet<TaskManagement> Tasks { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<FacilityAppointment> FacilityAppointments { get; set; }
        public DbSet<Person> People { get; set; }         
        public IotMonitoringDbContext(DbContextOptions<IotMonitoringDbContext> options)
            : base(options)
        {
        }
    }
}
