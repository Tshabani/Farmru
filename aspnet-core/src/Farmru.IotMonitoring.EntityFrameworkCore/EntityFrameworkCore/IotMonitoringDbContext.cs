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
using Farmru.IotMonitoring.Domains.Weather;
using Farmru.IotMonitoring.Domains.Crops;
using Farmru.IotMonitoring.Domains.Nutrients;

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
        public DbSet<WeatherObservation> WeatherObservations { get; set; }
        public DbSet<WeatherForecastDaily> WeatherForecastDailies { get; set; }
        public DbSet<EvapotranspirationReading> EvapotranspirationReadings { get; set; }
        public DbSet<WeatherAlertRule> WeatherAlertRules { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<CropType> CropTypes { get; set; }
        public DbSet<SeedSupplier> SeedSuppliers { get; set; }
        public DbSet<SeedVariety> SeedVarieties { get; set; }
        public DbSet<CropSeason> CropSeasons { get; set; }
        public DbSet<GrowthStageEvent> GrowthStageEvents { get; set; }
        public DbSet<HarvestRecord> HarvestRecords { get; set; }
        public DbSet<FertilizerProduct> FertilizerProducts { get; set; }
        public DbSet<FertilizerApplication> FertilizerApplications { get; set; }
        public DbSet<NutrientBalanceSnapshot> NutrientBalanceSnapshots { get; set; }

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

            modelBuilder.Entity<WeatherObservation>(b =>
            {
                b.HasOne(w => w.Facility).WithMany().HasForeignKey(w => w.FacilityId);
                b.HasIndex(e => new { e.TenantId, e.FacilityId, e.ObservedAt });
                b.Property(e => e.TemperatureCelsius).HasPrecision(5, 2);
                b.Property(e => e.HumidityPercent).HasPrecision(5, 2);
                b.Property(e => e.WindSpeedKph).HasPrecision(5, 2);
                b.Property(e => e.PrecipitationMm).HasPrecision(6, 2);
                b.Property(e => e.PressureHpa).HasPrecision(6, 2);
                b.Property(e => e.UvIndex).HasPrecision(4, 2);
                b.Property(e => e.LightningProbabilityPercent).HasPrecision(5, 2);
            });

            modelBuilder.Entity<WeatherForecastDaily>(b =>
            {
                b.HasOne(w => w.Facility).WithMany().HasForeignKey(w => w.FacilityId);
                b.HasIndex(e => new { e.TenantId, e.FacilityId, e.ForecastFor, e.GeneratedAt });
                b.Property(e => e.TempMinCelsius).HasPrecision(5, 2);
                b.Property(e => e.TempMaxCelsius).HasPrecision(5, 2);
                b.Property(e => e.WindGustKph).HasPrecision(5, 2);
            });

            modelBuilder.Entity<EvapotranspirationReading>(b =>
            {
                b.HasOne(e => e.Facility).WithMany().HasForeignKey(e => e.FacilityId);
                b.HasIndex(e => new { e.TenantId, e.FacilityId, e.Date });
                b.Property(e => e.Et0Mm).HasPrecision(5, 2);
                b.Property(e => e.EtcMm).HasPrecision(5, 2);
            });

            modelBuilder.Entity<WeatherAlertRule>(b =>
            {
                b.HasOne(w => w.Facility).WithMany().HasForeignKey(w => w.FacilityId);
                b.HasOne(w => w.Organisation).WithMany().HasForeignKey(w => w.OrganisationId);
                b.HasIndex(e => new { e.TenantId, e.FacilityId });
                b.HasIndex(e => new { e.TenantId, e.OrganisationId });
                b.Property(e => e.ThresholdValue).HasPrecision(6, 2);
                b.ToTable(t => t.HasCheckConstraint(
                    "CK_WeatherAlertRules_FacilityOrOrganisation",
                    "([FacilityId] IS NOT NULL AND [OrganisationId] IS NULL) OR ([FacilityId] IS NULL AND [OrganisationId] IS NOT NULL)"));
            });

            modelBuilder.Entity<Field>(b =>
            {
                b.HasOne(f => f.Facility).WithMany().HasForeignKey(f => f.FacilityId);
                b.HasOne(f => f.Boundary).WithMany().HasForeignKey(f => f.BoundaryGeoFenceId);
                b.HasIndex(e => new { e.TenantId, e.FacilityId });
                b.Property(e => e.AreaHectares).HasPrecision(8, 2);
            });

            modelBuilder.Entity<CropType>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.IsActive });
            });

            modelBuilder.Entity<SeedVariety>(b =>
            {
                b.HasOne(v => v.CropType).WithMany().HasForeignKey(v => v.CropTypeId);
                b.HasOne(v => v.Supplier).WithMany().HasForeignKey(v => v.SupplierId);
                b.HasIndex(e => new { e.TenantId, e.CropTypeId });
            });

            modelBuilder.Entity<CropSeason>(b =>
            {
                b.HasOne(s => s.Field).WithMany().HasForeignKey(s => s.FieldId);
                b.HasOne(s => s.CropType).WithMany().HasForeignKey(s => s.CropTypeId);
                b.HasOne(s => s.SeedVariety).WithMany().HasForeignKey(s => s.SeedVarietyId);
                b.HasMany(s => s.StageEvents).WithOne(e => e.CropSeason).HasForeignKey(e => e.CropSeasonId);
                b.HasOne(s => s.Harvest).WithOne(h => h.CropSeason).HasForeignKey<HarvestRecord>(h => h.CropSeasonId);
                b.HasIndex(e => new { e.TenantId, e.FieldId, e.Status });
                b.Property(e => e.ExpectedYieldKg).HasPrecision(10, 2);
            });

            modelBuilder.Entity<GrowthStageEvent>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.CropSeasonId, e.ObservedDate });
            });

            modelBuilder.Entity<HarvestRecord>(b =>
            {
                b.HasIndex(e => e.CropSeasonId).IsUnique();
                b.Property(e => e.ActualYieldKg).HasPrecision(10, 2);
            });

            modelBuilder.Entity<FertilizerProduct>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.Name });
                b.Property(e => e.NitrogenPercent).HasPrecision(5, 2);
                b.Property(e => e.PhosphorusPercent).HasPrecision(5, 2);
                b.Property(e => e.PotassiumPercent).HasPrecision(5, 2);
                b.Property(e => e.UnitCostPerKg).HasPrecision(8, 2);
            });

            modelBuilder.Entity<FertilizerApplication>(b =>
            {
                b.HasOne(a => a.Field).WithMany().HasForeignKey(a => a.FieldId);
                b.HasOne(a => a.CropSeason).WithMany().HasForeignKey(a => a.CropSeasonId);
                b.HasOne(a => a.Product).WithMany().HasForeignKey(a => a.ProductId);
                b.HasOne(a => a.Operator).WithMany().HasForeignKey(a => a.OperatorPersonId);
                b.HasIndex(e => new { e.TenantId, e.FieldId, e.ApplicationDate });
                b.Property(e => e.RateKgPerHectare).HasPrecision(8, 2);
                b.Property(e => e.Cost).HasPrecision(10, 2);
            });

            modelBuilder.Entity<NutrientBalanceSnapshot>(b =>
            {
                b.HasOne(s => s.Field).WithMany().HasForeignKey(s => s.FieldId);
                b.HasIndex(e => new { e.TenantId, e.FieldId, e.SnapshotDate });
                b.Property(e => e.SensedNitrogen).HasPrecision(6, 2);
                b.Property(e => e.SensedPhosphorus).HasPrecision(6, 2);
                b.Property(e => e.SensedPotassium).HasPrecision(6, 2);
                b.Property(e => e.AppliedNitrogenTrailing30d).HasPrecision(8, 2);
                b.Property(e => e.AppliedPhosphorusTrailing30d).HasPrecision(8, 2);
                b.Property(e => e.AppliedPotassiumTrailing30d).HasPrecision(8, 2);
            });
        }
    }
}
