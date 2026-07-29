using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Farmru.IotMonitoring.Domains.Crops;
using Farmru.IotMonitoring.Domains.Nodes;
using Farmru.IotMonitoring.Domains.Nutrients;
using Farmru.IotMonitoring.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NodeDataEntity = Farmru.IotMonitoring.Domains.Nodes.NodeData;

namespace Farmru.IotMonitoring.Services.Nutrients
{
    public class NutrientBalanceEvaluationEngine : INutrientBalanceEvaluationEngine
    {
        // Adequate-range defaults for the three sensed nutrients, on the same 0-100-ish
        // sensor scale NodeData already reports on. No NutrientThresholdConfiguration entity
        // exists in Phase 1 (Technical Design Section 2.3 defines no such entity), so these
        // are fixed defaults for now rather than per-tenant configurable, mirroring how
        // AlertThresholdConfiguration's own defaults started as fixed constants.
        private const decimal NitrogenDeficientBelow = 20m;
        private const decimal NitrogenSurplusAbove = 60m;
        private const decimal PhosphorusDeficientBelow = 15m;
        private const decimal PhosphorusSurplusAbove = 50m;
        private const decimal PotassiumDeficientBelow = 15m;
        private const decimal PotassiumSurplusAbove = 50m;

        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<Field, Guid> _fieldRepository;
        private readonly IRepository<CropSeason, Guid> _cropSeasonRepository;
        private readonly IRepository<Node, Guid> _nodeRepository;
        private readonly IRepository<NodeDataEntity, Guid> _nodeDataRepository;
        private readonly IRepository<FertilizerApplication, Guid> _applicationRepository;
        private readonly IRepository<NutrientBalanceSnapshot, Guid> _snapshotRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public NutrientBalanceEvaluationEngine(
            IRepository<Tenant> tenantRepository,
            IRepository<Field, Guid> fieldRepository,
            IRepository<CropSeason, Guid> cropSeasonRepository,
            IRepository<Node, Guid> nodeRepository,
            IRepository<NodeDataEntity, Guid> nodeDataRepository,
            IRepository<FertilizerApplication, Guid> applicationRepository,
            IRepository<NutrientBalanceSnapshot, Guid> snapshotRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _tenantRepository = tenantRepository;
            _fieldRepository = fieldRepository;
            _cropSeasonRepository = cropSeasonRepository;
            _nodeRepository = nodeRepository;
            _nodeDataRepository = nodeDataRepository;
            _applicationRepository = applicationRepository;
            _snapshotRepository = snapshotRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task RunFullEvaluationCycleAsync()
        {
            var tenantIds = await _tenantRepository.GetAll()
                .Where(t => t.IsActive)
                .Select(t => t.Id)
                .ToListAsync();

            foreach (var tenantId in tenantIds)
            {
                using var uow = _unitOfWorkManager.Begin();
                using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                {
                    await EvaluateTenantAsync(tenantId);
                    await uow.CompleteAsync();
                }
            }
        }

        private async Task EvaluateTenantAsync(int tenantId)
        {
            var fieldsWithActiveSeason = await _cropSeasonRepository.GetAll()
                .Include(s => s.Field)
                .Where(s => s.TenantId == tenantId
                    && (s.Status == CropSeasonStatus.Planned || s.Status == CropSeasonStatus.Growing))
                .Select(s => s.Field)
                .Distinct()
                .ToListAsync();

            foreach (var field in fieldsWithActiveSeason)
            {
                await EvaluateFieldAsync(tenantId, field);
            }
        }

        private async Task EvaluateFieldAsync(int tenantId, Field field)
        {
            var nodeIds = await _nodeRepository.GetAll()
                .Where(n => n.Facility != null && n.Facility.Id == field.FacilityId)
                .Select(n => n.Id)
                .ToListAsync();

            if (!nodeIds.Any())
            {
                return;
            }

            var latestReadings = await _nodeDataRepository.GetAll()
                .Include(d => d.Node)
                .Where(d => d.Node != null && nodeIds.Contains(d.Node.Id))
                .OrderByDescending(d => d.CreationTime)
                .Take(nodeIds.Count * 3)
                .ToListAsync();

            var latestPerNode = latestReadings
                .GroupBy(d => d.Node.Id)
                .Select(g => g.OrderByDescending(d => d.CreationTime).First())
                .ToList();

            if (!latestPerNode.Any())
            {
                return;
            }

            var sensedNitrogen = AverageOrZero(latestPerNode.Select(d => TryParseDecimal(d.Nitrogen)));
            var sensedPhosphorus = AverageOrZero(latestPerNode.Select(d => TryParseDecimal(d.Phosphorus)));
            var sensedPotassium = AverageOrZero(latestPerNode.Select(d => TryParseDecimal(d.Potassium)));

            var cutoff = DateTime.UtcNow.Date.AddDays(-30);
            var trailingApplications = await _applicationRepository.GetAll()
                .Include(a => a.Product)
                .Where(a => a.FieldId == field.Id && a.ApplicationDate >= cutoff)
                .ToListAsync();

            var appliedNitrogen = trailingApplications.Sum(a => a.RateKgPerHectare * (a.Product?.NitrogenPercent ?? 0) / 100m);
            var appliedPhosphorus = trailingApplications.Sum(a => a.RateKgPerHectare * (a.Product?.PhosphorusPercent ?? 0) / 100m);
            var appliedPotassium = trailingApplications.Sum(a => a.RateKgPerHectare * (a.Product?.PotassiumPercent ?? 0) / 100m);

            var snapshot = NutrientBalanceSnapshot.Record(
                tenantId,
                field,
                DateTime.UtcNow.Date,
                sensedNitrogen,
                sensedPhosphorus,
                sensedPotassium,
                appliedNitrogen,
                appliedPhosphorus,
                appliedPotassium,
                ClassifyStatus(sensedNitrogen, NitrogenDeficientBelow, NitrogenSurplusAbove),
                ClassifyStatus(sensedPhosphorus, PhosphorusDeficientBelow, PhosphorusSurplusAbove),
                ClassifyStatus(sensedPotassium, PotassiumDeficientBelow, PotassiumSurplusAbove));

            await _snapshotRepository.InsertAsync(snapshot);
        }

        private static NutrientBalanceStatus ClassifyStatus(decimal sensedValue, decimal deficientBelow, decimal surplusAbove)
        {
            if (sensedValue < deficientBelow)
            {
                return NutrientBalanceStatus.Deficient;
            }

            return sensedValue > surplusAbove ? NutrientBalanceStatus.Surplus : NutrientBalanceStatus.Adequate;
        }

        private static decimal AverageOrZero(System.Collections.Generic.IEnumerable<decimal?> values)
        {
            var parsed = values.Where(v => v.HasValue).Select(v => v.Value).ToList();
            return parsed.Any() ? parsed.Average() : 0m;
        }

        // Defensive parsing per Technical Design Section 2.3's known data-quality note:
        // NodeData's sensor fields are string-typed (pre-existing debt, not fixed here —
        // out of Phase 1 scope per the Sprint 0 finding). Unparseable readings are skipped,
        // not thrown, matching TelemetryAlertEvaluationService's existing TryParseDecimal.
        private static decimal? TryParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
                ? result
                : null;
        }
    }
}
