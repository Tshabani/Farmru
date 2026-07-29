using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Crops;
using System;

namespace Farmru.IotMonitoring.Domains.Nutrients
{
    /// <summary>
    /// A computed/materialized rollup written by NutrientBalanceEvaluationHostedService —
    /// never directly by a user action (Phase 1 Technical Design Section 2.3).
    /// </summary>
    public class NutrientBalanceSnapshot : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected NutrientBalanceSnapshot()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid FieldId { get; private set; }
        public virtual Field Field { get; private set; }
        public virtual DateTime SnapshotDate { get; private set; }
        public virtual decimal SensedNitrogen { get; private set; }
        public virtual decimal SensedPhosphorus { get; private set; }
        public virtual decimal SensedPotassium { get; private set; }
        public virtual decimal AppliedNitrogenTrailing30d { get; private set; }
        public virtual decimal AppliedPhosphorusTrailing30d { get; private set; }
        public virtual decimal AppliedPotassiumTrailing30d { get; private set; }
        public virtual NutrientBalanceStatus NitrogenStatus { get; private set; }
        public virtual NutrientBalanceStatus PhosphorusStatus { get; private set; }
        public virtual NutrientBalanceStatus PotassiumStatus { get; private set; }

        public static NutrientBalanceSnapshot Record(
            int tenantId,
            Field field,
            DateTime snapshotDate,
            decimal sensedNitrogen,
            decimal sensedPhosphorus,
            decimal sensedPotassium,
            decimal appliedNitrogenTrailing30d,
            decimal appliedPhosphorusTrailing30d,
            decimal appliedPotassiumTrailing30d,
            NutrientBalanceStatus nitrogenStatus,
            NutrientBalanceStatus phosphorusStatus,
            NutrientBalanceStatus potassiumStatus)
        {
            if (field == null)
            {
                throw new DomainRuleException("A nutrient balance snapshot requires a Field.");
            }

            return new NutrientBalanceSnapshot
            {
                TenantId = tenantId,
                Field = field,
                FieldId = field.Id,
                SnapshotDate = snapshotDate.Date,
                SensedNitrogen = sensedNitrogen,
                SensedPhosphorus = sensedPhosphorus,
                SensedPotassium = sensedPotassium,
                AppliedNitrogenTrailing30d = appliedNitrogenTrailing30d,
                AppliedPhosphorusTrailing30d = appliedPhosphorusTrailing30d,
                AppliedPotassiumTrailing30d = appliedPotassiumTrailing30d,
                NitrogenStatus = nitrogenStatus,
                PhosphorusStatus = phosphorusStatus,
                PotassiumStatus = potassiumStatus
            };
        }
    }
}
