using Farmru.IotMonitoring.Domains.Alerts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Farmru.IotMonitoring.EntityFrameworkCore.Seed.Tenants
{
    public class SampleAlertSeedContributor
    {
        private readonly IotMonitoringDbContext _context;

        public SampleAlertSeedContributor(IotMonitoringDbContext context)
        {
            _context = context;
        }

        public void Create(int tenantId)
        {
            if (_context.Alerts.IgnoreQueryFilters().Any(a => a.TenantId == tenantId))
            {
                return;
            }

            var node = _context.Nodes
                .IgnoreQueryFilters()
                .Include(n => n.Facility)
                .FirstOrDefault(n => n.TenantId == tenantId);

            if (node == null)
            {
                return;
            }

            var facilityId = node.Facility?.Id;

            var critical = Alert.Raise(
                tenantId,
                node.Id,
                facilityId,
                AlertType.LowBattery,
                AlertSeverity.Critical,
                "Low battery — demo alert",
                "Battery level is below the critical threshold. Replace or recharge the power source.");

            critical.AttachNode(node);

            var warning = Alert.Raise(
                tenantId,
                node.Id,
                facilityId,
                AlertType.SoilMoistureLow,
                AlertSeverity.Warning,
                "Soil moisture low — demo alert",
                "Soil moisture has dropped below the configured minimum for this facility.");

            warning.AttachNode(node);
            warning.Acknowledge(1);

            var info = Alert.Raise(
                tenantId,
                node.Id,
                facilityId,
                AlertType.TelemetryAnomaly,
                AlertSeverity.Info,
                "Telemetry anomaly — demo alert",
                "A minor deviation was detected in the latest sensor reading batch.");

            info.AttachNode(node);

            _context.Alerts.AddRange(critical, warning, info);
            _context.SaveChanges();
        }
    }
}
