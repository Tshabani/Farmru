using Farmru.IotMonitoring.Domains.Alerts;
using Farmru.IotMonitoring.EntityFrameworkCore;
using System.Linq;

namespace Farmru.IotMonitoring.EntityFrameworkCore.Seed.Tenants
{
    public class AlertThresholdSeedContributor
    {
        private readonly IotMonitoringDbContext _context;

        public AlertThresholdSeedContributor(IotMonitoringDbContext context)
        {
            _context = context;
        }

        public void Create(int tenantId)
        {
            if (_context.AlertThresholdConfigurations.Any(t => t.TenantId == tenantId && t.FacilityId == null))
            {
                return;
            }

            _context.AlertThresholdConfigurations.Add(AlertThresholdConfiguration.CreateDefault(tenantId));
            _context.SaveChanges();
        }
    }
}
