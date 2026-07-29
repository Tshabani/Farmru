using Abp.Dependency;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Nutrients
{
    /// <summary>
    /// Computes and persists a NutrientBalanceSnapshot per Field with an active CropSeason,
    /// from the latest NodeData in the Field's Facility plus trailing-30-day
    /// FertilizerApplication totals (Phase 1 Technical Design Section 5.2).
    ///
    /// Domain-event publishing (NutrientDeficiencyDetectedEvent, Technical Design Section 5.4)
    /// is deliberately NOT implemented here: no ABP event-bus convention exists anywhere in
    /// this codebase today (verified — Incident and Alert, the two workflows the Technical
    /// Design expected to copy from, raise no domain events at all). Since the event has zero
    /// subscribers in Phase 1 by design, this is a documented gap for a future decision, not a
    /// blocker for this evaluator's actual job (computing and persisting the snapshot).
    /// </summary>
    public interface INutrientBalanceEvaluationEngine : ITransientDependency
    {
        Task RunFullEvaluationCycleAsync();
    }
}
