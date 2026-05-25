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
using Farmru.IotMonitoring.Domains.Stats;
using Farmru.IotMonitoring.Domains.Alerts;
using Farmru.IotMonitoring.Domains.Monitoring;
using Farmru.IotMonitoring.Domains.Geo;

namespace Farmru.IotMonitoring.EntityFrameworkCore
{
    public class IotMonitoringDbContext : AbpZeroDbContext<Tenant, Role, User, IotMonitoringDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<NodeData> NodeDatas { get; set; }
        public DbSet<Node> Nodes { get; set; }
        public DbSet<NodeReplacementHistory> NodeReplacementHistories { get; set; }
        public DbSet<TaskManagement> Tasks { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Incident> Incidents { get; set; }
        public DbSet<IncidentAssignment> IncidentAssignments { get; set; }
        public DbSet<IncidentTimelineEvent> IncidentTimelineEvents { get; set; }
        public DbSet<IncidentAttachment> IncidentAttachments { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<FacilityAppointment> FacilityAppointments { get; set; }
        public DbSet<Person> People { get; set; }         
        public DbSet<AverageNodeData> AverageNodeData { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<AlertThresholdConfiguration> AlertThresholdConfigurations { get; set; }
        public DbSet<MonitoringExecutionHistory> MonitoringExecutionHistories { get; set; }
        public DbSet<GeoFence> GeoFences { get; set; }

        public IotMonitoringDbContext(DbContextOptions<IotMonitoringDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Alert>(b =>
            {
                b.HasOne(a => a.Node).WithMany().HasForeignKey(a => a.NodeId);
                b.HasOne(a => a.Facility).WithMany().HasForeignKey(a => a.FacilityId);
                b.HasIndex(e => new { e.TenantId, e.IsActive, e.IsResolved });
                b.HasIndex(e => new { e.TenantId, e.NodeId, e.AlertType, e.IsActive });
                b.HasIndex(e => new { e.TenantId, e.Severity, e.TriggeredAt });
                b.HasIndex(e => e.TriggeredAt);
            });

            modelBuilder.Entity<AlertThresholdConfiguration>(b =>
            {
                b.HasOne(t => t.Facility).WithMany().HasForeignKey(t => t.FacilityId);
                b.HasIndex(e => new { e.TenantId, e.FacilityId });
            });

            modelBuilder.Entity<MonitoringExecutionHistory>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.JobType, e.StartedAt });
                b.HasIndex(e => e.StartedAt);
            });

            modelBuilder.Entity<GeoFence>(b =>
            {
                b.HasOne(g => g.Facility).WithMany().HasForeignKey(g => g.FacilityId);
                b.HasIndex(e => new { e.TenantId, e.IsActive });
                b.HasIndex(e => new { e.TenantId, e.FacilityId });
            });

            modelBuilder.Entity<Node>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.LastKnownLatitude, e.LastKnownLongitude });
            });

            modelBuilder.Entity<Incident>(b =>
            {
                b.HasOne(i => i.CreatedBy).WithMany().HasForeignKey("CreatedById").OnDelete(DeleteBehavior.Restrict);
                b.HasOne(i => i.AssignedTo).WithMany().HasForeignKey("AssignedToId").OnDelete(DeleteBehavior.Restrict);
                b.HasOne(i => i.Facility).WithMany().HasForeignKey(i => i.FacilityId).OnDelete(DeleteBehavior.Restrict);
                b.HasMany(i => i.Assignments).WithOne(a => a.Incident).HasForeignKey(a => a.IncidentId);
                b.HasMany(i => i.Timeline).WithOne(t => t.Incident).HasForeignKey(t => t.IncidentId);
                b.HasMany(i => i.Attachments).WithOne(a => a.Incident).HasForeignKey(a => a.IncidentId);
                b.HasIndex(e => new { e.TenantId, e.Status, e.Priority });
                b.HasIndex(e => new { e.TenantId, e.SlaStatus, e.ResolutionDueAt });
                b.HasIndex("TenantId", "AssignedToId", "Status");
                b.HasIndex(e => new { e.Latitude, e.Longitude });
            });

            modelBuilder.Entity<IncidentAssignment>(b =>
            {
                b.HasOne(a => a.AssignedPerson).WithMany().HasForeignKey(a => a.AssignedPersonId).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.IncidentId, e.IsActive });
                b.HasIndex(e => new { e.TenantId, e.AssignedPersonId, e.IsActive });
            });

            modelBuilder.Entity<IncidentTimelineEvent>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.IncidentId, e.CreationTime });
            });

            modelBuilder.Entity<IncidentAttachment>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.IncidentId });
            });
        }
    }
}
