---
title: Farmru Phase 1 — Technical Design
---

# Farmru Phase 1 Technical Design
## Weather Intelligence · Crop Management · Fertilizer & Nutrient Management

**Document status:** Implementation blueprint, traceable to **Farmru Product Plan v1.0** (frozen — see governance note below). This document does not introduce new product scope; every entity, screen, and endpoint here is a direct technical elaboration of Product Plan Sections 5.1, 5.2, and 5.3.

**Governance chain in effect from this point forward:**
```
Product Plan v1.0 (source of truth, frozen)
        │
        ▼
Phase 1 Technical Design (this document — implementation blueprint)
        │
        ▼
Sprint Backlog (execution)
        │
        ▼
Code
```
Any capability not traceable to the Product Plan requires a formal change request against the Plan before it can enter this design or a sprint backlog — this document does not accept new scope on its own authority.

**Grounding note:** every convention below (entity style, AppService pattern, permission naming, Angular module shape, Flutter service shape) is taken directly from the existing codebase — `Node`, `NodeData`, `Facility`, `NodeAppService`, `PermissionNames`, the `node` Angular module, and `NodeService` in Flutter — not invented. Where the existing codebase doesn't yet establish a pattern this design needs (background job scheduling, event bus usage, device authentication), that gap is called out explicitly rather than assumed.

---

## 1. Architecture & Scope

### 1.1 Phase 1 Boundaries

**In scope:**
- Weather Intelligence (current conditions, forecast, history, evapotranspiration, weather-driven alerts) — Product Plan 5.1
- Crop Management (Fields, Crop Seasons, Crop Types, Seed Varieties/Suppliers, Growth Stages, Harvest, derived Rotation History) — Product Plan 5.2
- Fertilizer & Nutrient Management (fertilizer products, applications, nutrient balance, deficiency-driven recommendation stub) — Product Plan 5.3
- Farm Activity Timeline **event capture only** (Product Plan 5.21) — the three Phase 1 modules should emit domain events sufficient to populate a timeline later; the Timeline read-surface itself is not built in Phase 1, but retrofitting event emission after the fact is exactly the kind of rework this design avoids by doing it now.
- **Prerequisite hardening carried in from Product Plan Section 19/25 risk log:** telemetry ingestion device authentication. This is included in Phase 1 scope because Predictive Maintenance and Disease Risk (Phase 2) both depend on trustworthy `NodeData`, and it is cheaper to fix before three more phases of code assume the current posture.

**Explicitly out of scope for Phase 1** (per Product Plan phasing): AI Agronomist, Disease Risk, Pest Monitoring, Automation Rules Engine, Workflow Designer, and everything in Phase 3+. Weather and Nutrient data produced in Phase 1 are *consumed* by these later — this design's job is to make sure the data they produce is shaped so Phase 2 doesn't need to reshape it.

### 1.2 Existing Components Reused

| Layer | Reused as-is |
|---|---|
| `Farmru.IotMonitoring.Core` | `FullAuditedAggregateRoot<Guid>`, `IMustHaveTenant`, `DomainRuleException`, `GeoCoordinateHelper` (value-object-style coordinate normalization already used by `Facility`) |
| `Farmru.IotMonitoring.Application` | `AsyncCrudAppService<...>` base class pattern (as used by `NodeAppService`), existing `PagedNodeResultRequestDto`-style paging convention |
| `Farmru.IotMonitoring.EntityFrameworkCore` | EF Core migration pipeline (existing dated-migration convention, e.g. `20260527121612_Add_AlertThresholdConfigurations`) |
| `Farmru.IotMonitoring.Web.Host` | JWT auth, Swashbuckle/Swagger, existing SignalR hub infrastructure (`AlertNotificationHub` and friends) |
| Angular `shared/service-proxies` | NSwag-generated service proxy pattern (evidenced by `ServiceProxyModule` import in `node.module.ts`) — new AppServices are added to the existing proxy generation, not hand-written HTTP clients |
| Angular `shared/auth/auth-route-guard` | `AppRouteGuard` + `Pages.*` permission-gated routing (as used in `node-routing.module.ts`) |
| Flutter `utils/base_client.dart` | Existing `BaseClient().get(...)`/presumed `.post(...)` wrapper hitting `api/services/app/{Service}/{Method}` — the ABP dynamic web API convention confirmed in `NodeService` |
| Flutter `services/*_service.dart` pattern | Static-method service classes returning typed models parsed from the ABP `{ result: ... }` envelope |

### 1.3 New Bounded Contexts

Per the Bounded Context Map in Product Plan Section 22, Phase 1 introduces one new context and extends one existing one:

- **Field Operations context (extended):** `Facility` gains a child `Field` concept; `Weather*` entities attach to `Facility`. This context already owns Node/NodeData/GeoFence — Weather and Field are additive, same ownership.
- **Agronomy Intelligence context (new, Phase 1 seed):** `CropType`, `SeedVariety`, `SeedSupplier`, `CropSeason`, `GrowthStageEvent`, `HarvestRecord`, `FertilizerProduct`, `FertilizerApplication`, `NutrientBalanceHistory`. This context does not yet exist in the codebase — Phase 1 creates it. Per the Bounded Context ownership rule (Product Plan 22), Fertilizer Application deducting inventory stock is *not* implemented in Phase 1 (Inventory Management is Phase 3) — `FertilizerApplication.Cost` is captured as a plain decimal for now, with a documented seam (`InventorySourceRef` nullable FK, unused until Phase 3) rather than no seam at all.

### 1.4 Integration Points

```
                    ┌──────────────────────────┐
                    │   IWeatherProvider        │   (new, external)
                    │   (3rd-party weather API) │
                    └────────────┬──────────────┘
                                 │
                    ┌────────────▼──────────────┐
                    │  WeatherSyncJob (new)      │──► WeatherObservation
                    │  scheduled, per-Facility   │──► WeatherForecastDaily/Hourly
                    └────────────┬──────────────┘──► EvapotranspirationReading
                                 │
                                 ▼
                    ┌───────────────────────────┐
                    │  WeatherAlertEvaluator     │──► Alert (existing entity,
                    │  (new domain service)      │    AlertSource.Weather)
                    └────────────┬──────────────┘
                                 │
                    ┌────────────▼──────────────┐
                    │  AlertNotificationHub       │  (existing SignalR hub,
                    │  (existing)                 │   reused, not extended)
                    └────────────────────────────┘

  Node/NodeData (existing) ──► NutrientBalanceEvaluator (new) ──► NutrientBalanceHistory
                                        ▲
                                        │
                             CropSeason.GrowthStage (new, provides stage-aware
                             nutrient requirement lookup)
```

### 1.5 Sequence Diagrams

**Weather sync → alert (scheduled path):**
```
WeatherSyncJob          IWeatherProvider     WeatherObservation/Forecast repo   WeatherAlertEvaluator   IRepository<Alert>   AlertNotificationHub
     │  (per Facility, on schedule)
     │──GetCurrent/GetForecast(lat,long)──►│
     │◄──────────normalized data───────────│
     │──save───────────────────────────────────────►│
     │──evaluate(FacilityId)───────────────────────────────────────────────────►│
     │                                                                          │──check WeatherAlertRule thresholds
     │                                                                          │──create Alert if breached──────────►│
     │                                                                                                                 │──push to tenant group───────────────►│
```

**Crop Season → Harvest (user-driven path):**
```
Angular CropSeasonWizard   CropSeasonAppService   Field/CropType/SeedVariety repos   CropSeason (domain)
        │──POST Create(FieldId, CropTypeId, SeedVarietyId, plantingDate, expectedYield)──►│
        │                                                    │──load Field, CropType, SeedVariety───►│
        │                                                    │──CropSeason.Plant(...)──────────────────────►│ (validates: Field not already in an open season)
        │◄──────────────────────────CropSeasonDto─────────────────────────────────────────────────────│

  ... season progresses via GrowthStageEvent entries (each: CropSeasonAppService.LogGrowthStage) ...

        │──POST Harvest(CropSeasonId, actualYield, qualityGrade)──────────────►│
        │                                                    │──CropSeason.Close(harvest)───────────────────►│ (state transition: Growing → Harvested)
        │                                                    │──derive/update CropRotationHistory (query, not write)
```

---

## 2. Domain Model

Following the codebase's established rich-domain style: `FullAuditedAggregateRoot<Guid>`, `IMustHaveTenant`, private setters, static factory methods, `DomainRuleException` for invariant violations. Namespace convention: `Farmru.IotMonitoring.Domains.<AggregateFolder>`.

### 2.1 Weather Intelligence

```csharp
namespace Farmru.IotMonitoring.Domains.Weather
{
    // Aggregate root — one row per Facility per observation timestamp.
    public class WeatherObservation : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        public virtual int TenantId { get; set; }
        public virtual Facility Facility { get; private set; }
        public virtual DateTime ObservedAt { get; private set; }
        public virtual decimal TemperatureCelsius { get; private set; }
        public virtual decimal HumidityPercent { get; private set; }
        public virtual decimal? WindSpeedKph { get; private set; }
        public virtual int? WindDirectionDegrees { get; private set; }
        public virtual decimal? PrecipitationMm { get; private set; }
        public virtual decimal? PressureHpa { get; private set; }
        public virtual decimal? UvIndex { get; private set; }
        public virtual string ProviderRef { get; private set; }   // provider's own record id, for reconciliation

        public static WeatherObservation Record(Facility facility, DateTime observedAt,
            decimal temperatureCelsius, decimal humidityPercent, decimal? windSpeedKph,
            int? windDirectionDegrees, decimal? precipitationMm, decimal? pressureHpa,
            decimal? uvIndex, string providerRef)
        {
            if (facility == null) throw new DomainRuleException("Facility is required for a weather observation.");
            if (humidityPercent < 0 || humidityPercent > 100) throw new DomainRuleException("Humidity must be 0-100%.");
            // ... remaining assignment, mirrors NodeData.RecordFromDevice's factory-method shape
        }
    }

    // Aggregate root — one row per Facility per forecast validity window.
    public class WeatherForecastDaily : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        public virtual int TenantId { get; set; }
        public virtual Facility Facility { get; private set; }
        public virtual DateTime ForecastFor { get; private set; }       // the calendar date being forecast
        public virtual DateTime GeneratedAt { get; private set; }       // when the forecast was fetched (forecasts are superseded, not updated in place — see 2.1 note)
        public virtual decimal TempMinCelsius { get; private set; }
        public virtual decimal TempMaxCelsius { get; private set; }
        public virtual int PrecipitationProbabilityPercent { get; private set; }
        public virtual decimal? WindGustKph { get; private set; }
        public virtual FrostRiskLevel FrostRisk { get; private set; }
        public virtual HeatStressLevel HeatStress { get; private set; }
        public virtual string ProviderRef { get; private set; }
    }

    public class EvapotranspirationReading : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        public virtual int TenantId { get; set; }
        public virtual Facility Facility { get; private set; }
        public virtual DateTime Date { get; private set; }
        public virtual decimal Et0Mm { get; private set; }              // reference evapotranspiration
        public virtual decimal? EtcMm { get; private set; }             // crop-adjusted, requires an active CropSeason + crop coefficient
        public virtual Guid? CropSeasonId { get; private set; }         // nullable: Et0 can be computed with no crop planted
    }

    public class WeatherAlertRule : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        public virtual int TenantId { get; set; }
        public virtual Facility Facility { get; private set; }          // null = organisation-wide default rule
        public virtual Organisation Organisation { get; private set; }
        public virtual WeatherAlertType AlertType { get; private set; } // Frost, Wind, Heat, Lightning, RainSevere
        public virtual decimal ThresholdValue { get; private set; }
        public virtual AlertSeverity Severity { get; private set; }     // reuses existing Alert severity enum
        public virtual bool IsActive { get; private set; }

        public static WeatherAlertRule Create(Facility facility, Organisation organisation,
            WeatherAlertType alertType, decimal thresholdValue, AlertSeverity severity)
        {
            if (facility == null && organisation == null)
                throw new DomainRuleException("A weather alert rule must be scoped to a Facility or an Organisation.");
            // ...
        }
    }

    public enum WeatherAlertType { Frost, Wind, Heat, Lightning, RainSevere }
    public enum FrostRiskLevel { None, Watch, Warning }
    public enum HeatStressLevel { None, Elevated, Severe }
}
```

**Domain events:** `WeatherObservationRecordedEvent`, `WeatherAlertRaisedEvent` (published via ABP's local event bus — see Section 5.4; feeds the future Timeline).

**Validation rules:** humidity 0–100%; `ForecastFor` must be present/future at creation time (a forecast for a past date is a data bug, not a valid state); `WeatherAlertRule` must be scoped to exactly one of Facility/Organisation, not both, not neither.

**Note on forecast mutability:** `WeatherForecastDaily` rows are **append-only** (a new row per fetch, not an update-in-place) — this preserves forecast-accuracy history for free (compare `GeneratedAt=T-3` vs `GeneratedAt=T-1` predictions for the same `ForecastFor` date), which the Product Plan's Yield Prediction module (Phase 5) will want and which is far cheaper to capture now than to backfill later.

### 2.2 Crop Management

```csharp
namespace Farmru.IotMonitoring.Domains.Crops
{
    // New child concept under Facility.
    public class Field : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        public virtual int TenantId { get; set; }
        public virtual Facility Facility { get; private set; }
        public virtual string Name { get; private set; }
        public virtual decimal? AreaHectares { get; private set; }
        public virtual string SoilType { get; private set; }
        public virtual GeoFence Boundary { get; private set; }   // reuses existing GeoFence entity/polygon capability — no new spatial type introduced

        public static Field Create(Facility facility, string name, decimal? areaHectares, string soilType, GeoFence boundary)
        {
            if (facility == null) throw new DomainRuleException("A Field must belong to a Facility.");
            var trimmed = name?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.Length < 2)
                throw new DomainRuleException("Field name must be at least 2 characters.");
            // ...
        }
    }

    public class CropType : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        public virtual int TenantId { get; set; }
        public virtual string Name { get; private set; }               // e.g. "Maize"
        public virtual string ScientificName { get; private set; }
        public virtual int TypicalGrowthDurationDays { get; private set; }
        public virtual bool IsActive { get; private set; }
    }

    public class SeedSupplier : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        public virtual int TenantId { get; set; }
        public virtual string Name { get; private set; }
        public virtual string ContactInfo { get; private set; }
    }

    public class SeedVariety : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        public virtual int TenantId { get; set; }
        public virtual CropType CropType { get; private set; }
        public virtual SeedSupplier Supplier { get; private set; }
        public virtual string Name { get; private set; }
        public virtual int? DaysToMaturity { get; private set; }
    }

    // Aggregate root for the season lifecycle — owns GrowthStageEvent and HarvestRecord as
    // part of its consistency boundary (a season's stage history and harvest outcome are
    // never meaningful independent of the season itself).
    public class CropSeason : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        private readonly List<GrowthStageEvent> _stageEvents = new();

        public virtual int TenantId { get; set; }
        public virtual Field Field { get; private set; }
        public virtual CropType CropType { get; private set; }
        public virtual SeedVariety SeedVariety { get; private set; }
        public virtual DateTime PlantingDate { get; private set; }
        public virtual DateTime ExpectedHarvestDate { get; private set; }
        public virtual decimal? ExpectedYieldKg { get; private set; }
        public virtual int? PlantPopulationPerHectare { get; private set; }
        public virtual CropSeasonStatus Status { get; private set; }   // Planned -> Growing -> Harvested -> Closed (see state machine, 2.5)
        public virtual HarvestRecord Harvest { get; private set; }     // null until Harvested

        public virtual IReadOnlyCollection<GrowthStageEvent> StageEvents => _stageEvents.AsReadOnly();

        public static CropSeason Plant(Field field, CropType cropType, SeedVariety seedVariety,
            DateTime plantingDate, DateTime expectedHarvestDate, decimal? expectedYieldKg, int? plantPopulationPerHectare)
        {
            if (field == null) throw new DomainRuleException("A Crop Season requires a Field.");
            if (expectedHarvestDate <= plantingDate)
                throw new DomainRuleException("Expected harvest date must be after planting date.");
            var season = new CropSeason { /* ... */ Status = CropSeasonStatus.Planned };
            season.LogStageInternal(GrowthStage.Planted, plantingDate, GrowthStageSource.Manual);
            return season;
        }

        public virtual void LogGrowthStage(GrowthStage stage, DateTime observedDate, GrowthStageSource source)
        {
            if (Status == CropSeasonStatus.Closed)
                throw new DomainRuleException("Cannot log a growth stage on a closed season.");
            LogStageInternal(stage, observedDate, source);
            if (Status == CropSeasonStatus.Planned) Status = CropSeasonStatus.Growing;
        }

        public virtual HarvestRecord Harvest(DateTime harvestDate, decimal actualYieldKg, string qualityGrade)
        {
            if (Status != CropSeasonStatus.Growing)
                throw new DomainRuleException($"Cannot harvest a season in status {Status}. Expected: Growing.");
            Harvest = HarvestRecord.Create(this, harvestDate, actualYieldKg, qualityGrade);
            Status = CropSeasonStatus.Harvested;
            return Harvest;
        }

        public virtual void Close()
        {
            if (Status != CropSeasonStatus.Harvested)
                throw new DomainRuleException("Only a harvested season can be closed.");
            Status = CropSeasonStatus.Closed;
        }

        private void LogStageInternal(GrowthStage stage, DateTime observedDate, GrowthStageSource source)
            => _stageEvents.Add(GrowthStageEvent.Create(this, stage, observedDate, source));
    }

    public class GrowthStageEvent : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        public virtual int TenantId { get; set; }
        public virtual CropSeason CropSeason { get; private set; }
        public virtual GrowthStage Stage { get; private set; }
        public virtual DateTime ObservedDate { get; private set; }
        public virtual GrowthStageSource Source { get; private set; }  // Manual | Satellite (Phase 3+ NDVI-inferred, seam only)

        internal static GrowthStageEvent Create(CropSeason season, GrowthStage stage, DateTime observedDate, GrowthStageSource source) { /* ... */ }
    }

    public class HarvestRecord : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        public virtual int TenantId { get; set; }
        public virtual CropSeason CropSeason { get; private set; }
        public virtual DateTime HarvestDate { get; private set; }
        public virtual decimal ActualYieldKg { get; private set; }
        public virtual string QualityGrade { get; private set; }

        internal static HarvestRecord Create(CropSeason season, DateTime harvestDate, decimal actualYieldKg, string qualityGrade) { /* ... */ }
    }

    public enum CropSeasonStatus { Planned, Growing, Harvested, Closed }
    public enum GrowthStage { Planted, Germination, Vegetative, Flowering, Fruiting, Maturity, Harvested }
    public enum GrowthStageSource { Manual, Satellite }
}
```

**Value object note:** `CropRotationHistory` (Product Plan 5.2) is **deliberately not a stored entity** — it's a computed query (`CropSeasonAppService.GetRotationHistory(fieldId)` orders closed `CropSeason`s by `PlantingDate`). This avoids a denormalized table that can drift from the source `CropSeason` records.

**Domain events:** `CropSeasonPlantedEvent`, `GrowthStageLoggedEvent`, `CropSeasonHarvestedEvent`.

**Validation rules:** a `Field` cannot have two `CropSeason`s with overlapping `Status` in `{Planned, Growing}` at once (enforced at the AppService layer via a query check before calling `CropSeason.Plant`, since it's a cross-aggregate invariant — the domain layer alone can't see sibling seasons); `ExpectedHarvestDate` must be after `PlantingDate`.

### 2.3 Fertilizer & Nutrient Management

```csharp
namespace Farmru.IotMonitoring.Domains.Nutrients
{
    public class FertilizerProduct : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        public virtual int TenantId { get; set; }
        public virtual string Name { get; private set; }
        public virtual decimal NitrogenPercent { get; private set; }
        public virtual decimal PhosphorusPercent { get; private set; }
        public virtual decimal PotassiumPercent { get; private set; }
        public virtual decimal? UnitCostPerKg { get; private set; }
        public virtual Guid? SupplierId { get; private set; }          // seam to SeedSupplier-style Supplier concept; not enforced as FK to a broader Supplier entity until Marketplace (Phase 5)
    }

    public class FertilizerApplication : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        public virtual int TenantId { get; set; }
        public virtual Field Field { get; private set; }
        public virtual CropSeason CropSeason { get; private set; }     // nullable: an application can predate a season being logged
        public virtual FertilizerProduct Product { get; private set; }
        public virtual decimal RateKgPerHectare { get; private set; }
        public virtual DateTime ApplicationDate { get; private set; }
        public virtual decimal? Cost { get; private set; }
        public virtual Person Operator { get; private set; }           // reuses existing Person entity
        public virtual Guid? InventorySourceRef { get; private set; }  // unused seam — populated from Phase 3 Inventory Management onward

        public static FertilizerApplication Apply(Field field, CropSeason cropSeason, FertilizerProduct product,
            decimal rateKgPerHectare, DateTime applicationDate, decimal? cost, Person appliedBy)
        {
            if (field == null) throw new DomainRuleException("An application requires a Field.");
            if (product == null) throw new DomainRuleException("An application requires a Fertilizer Product.");
            if (rateKgPerHectare <= 0) throw new DomainRuleException("Application rate must be positive.");
            // ...
        }
    }

    // Computed/materialized snapshot — written by NutrientBalanceEvaluator (Section 5.3), never
    // directly by a user action. Exists purely so the dashboard doesn't recompute from raw
    // NodeData + FertilizerApplication history on every page load.
    public class NutrientBalanceSnapshot : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        public virtual int TenantId { get; set; }
        public virtual Field Field { get; private set; }
        public virtual DateTime SnapshotDate { get; private set; }
        public virtual decimal SensedNitrogen { get; private set; }    // from latest NodeData on a Node assigned within the Field's Facility
        public virtual decimal SensedPhosphorus { get; private set; }
        public virtual decimal SensedPotassium { get; private set; }
        public virtual decimal AppliedNitrogenTrailing30d { get; private set; }
        public virtual decimal AppliedPhosphorusTrailing30d { get; private set; }
        public virtual decimal AppliedPotassiumTrailing30d { get; private set; }
        public virtual NutrientBalanceStatus NitrogenStatus { get; private set; }   // Deficient | Adequate | Surplus
        public virtual NutrientBalanceStatus PhosphorusStatus { get; private set; }
        public virtual NutrientBalanceStatus PotassiumStatus { get; private set; }
    }

    public enum NutrientBalanceStatus { Deficient, Adequate, Surplus }
}
```

**A known upstream data-quality issue this module must account for:** `NodeData.SoilPH`, `.Nitrogen`, `.Phosphorus`, `.Potassium`, `.Moisture`, `.SoilTemperature` are currently typed as `string` in the existing entity (`Farmru.IotMonitoring.Core/Domains/Nodes/NodeData.cs`), not `decimal`. `NutrientBalanceEvaluator` (Section 5.3 below) must therefore parse defensively (culture-invariant `decimal.TryParse`, skip/flag unparseable readings rather than throw) — this is a pre-existing data-quality debt, not something Phase 1 should silently paper over. **Recommendation, not required for Phase 1:** file a follow-up item to migrate `NodeData`'s sensor columns to proper numeric types, since every future analytical module (Disease Risk, Yield Prediction) inherits this same parsing burden otherwise.

**Domain events:** `FertilizerAppliedEvent`, `NutrientDeficiencyDetectedEvent` (raised by the evaluator when a snapshot's status flips to `Deficient` — this is the seam Phase 2's AI Agronomist will subscribe to).

### 2.4 Cross-Cutting: Facility → Field Relationship

`Facility` (existing) is **not modified** — `Field` is added as a new child aggregate referencing `Facility` by ID, consistent with the Bounded Context rule (Product Plan 22) that existing aggregates are referenced, not restructured. No migration touches the `Facilities` table.

### 2.5 State Transitions

```
CropSeason.Status:
  Planned ──(LogGrowthStage, any stage)──► Growing ──(Harvest)──► Harvested ──(Close)──► Closed
     │
     └──(no direct transition to Harvested — must pass through Growing; enforced in CropSeason.Harvest guard clause)

WeatherForecastDaily: immutable once created (no transitions — append-only per 2.1 note)

Alert (existing entity, reused): unaffected — WeatherAlertEvaluator creates new Alert rows through the
  existing Alert.Raise(...)-style factory, entering the existing Open → Acknowledged → Resolved workflow untouched.
```

---

## 3. Database Design

### 3.1 ERD (Phase 1 additions only; existing tables shown for FK context)

```
 AbpTenants (existing)                Facilities (existing)
        │1                                    │1
        │                                     │
        │                    ┌────────────────┼─────────────────────┐
        │                    │N                │N                    │N
        │              WeatherObservations  WeatherForecastDaily   Fields (new)
        │                    │                                       │1
        │              EvapotranspirationReadings                    │
        │                    │                                       │N
        │              WeatherAlertRules ──────► Alerts (existing)  CropSeasons
        │                                                             │1
        │                                          ┌──────────────────┼───────────────────┐
        │                                          │N                 │1                    │N
        │                                   GrowthStageEvents    HarvestRecords    FertilizerApplications
        │                                                                                    │N
        │                                                                                     ▼
        │                                                                          FertilizerProducts
        │
   CropTypes ◄──────────── SeedVarieties ────────► SeedSuppliers
        │N                        │
        └────────────────────────┘
                 (SeedVariety.CropTypeId FK)

  NutrientBalanceSnapshots ──► Fields (FK), reads NodeData (existing, via Facility→Node join, no FK)
```

### 3.2 Table Layout (representative — full DDL generated by EF Core migrations, not hand-written here)

| Table | Key columns | Notes |
|---|---|---|
| `WeatherObservations` | `Id (uniqueidentifier, PK)`, `TenantId (int)`, `FacilityId (uniqueidentifier, FK)`, `ObservedAt (datetime2)`, `TemperatureCelsius (decimal(5,2))`, `HumidityPercent (decimal(5,2))`, `WindSpeedKph (decimal(5,2), null)`, `PrecipitationMm (decimal(6,2), null)`, `ProviderRef (nvarchar(100), null)` + ABP audit columns (`CreationTime`, `CreatorUserId`, `IsDeleted`, ...) | High write volume — see indexing below |
| `WeatherForecastDaily` | `Id`, `TenantId`, `FacilityId (FK)`, `ForecastFor (date)`, `GeneratedAt (datetime2)`, `TempMinCelsius`, `TempMaxCelsius`, `PrecipitationProbabilityPercent (int)`, `FrostRisk (int, enum)`, `HeatStress (int, enum)` | Append-only per 2.1 |
| `EvapotranspirationReadings` | `Id`, `TenantId`, `FacilityId (FK)`, `Date (date)`, `Et0Mm (decimal(5,2))`, `EtcMm (decimal(5,2), null)`, `CropSeasonId (FK, null)` | |
| `WeatherAlertRules` | `Id`, `TenantId`, `FacilityId (FK, null)`, `OrganisationId (FK, null)`, `AlertType (int, enum)`, `ThresholdValue (decimal(6,2))`, `Severity (int, enum)`, `IsActive (bit)` | Check constraint: exactly one of FacilityId/OrganisationId non-null |
| `Fields` | `Id`, `TenantId`, `FacilityId (FK)`, `Name (nvarchar(100))`, `AreaHectares (decimal(8,2), null)`, `SoilType (nvarchar(50), null)`, `BoundaryGeoFenceId (FK, null)` | |
| `CropTypes` | `Id`, `TenantId`, `Name (nvarchar(100))`, `ScientificName (nvarchar(150), null)`, `TypicalGrowthDurationDays (int)`, `IsActive (bit)` | |
| `SeedSuppliers` | `Id`, `TenantId`, `Name`, `ContactInfo` | |
| `SeedVarieties` | `Id`, `TenantId`, `CropTypeId (FK)`, `SupplierId (FK, null)`, `Name`, `DaysToMaturity (int, null)` | |
| `CropSeasons` | `Id`, `TenantId`, `FieldId (FK)`, `CropTypeId (FK)`, `SeedVarietyId (FK, null)`, `PlantingDate (date)`, `ExpectedHarvestDate (date)`, `ExpectedYieldKg (decimal(10,2), null)`, `PlantPopulationPerHectare (int, null)`, `Status (int, enum)` | |
| `GrowthStageEvents` | `Id`, `TenantId`, `CropSeasonId (FK)`, `Stage (int, enum)`, `ObservedDate (date)`, `Source (int, enum)` | |
| `HarvestRecords` | `Id`, `TenantId`, `CropSeasonId (FK, unique — 1:1)`, `HarvestDate (date)`, `ActualYieldKg (decimal(10,2))`, `QualityGrade (nvarchar(50), null)` | Unique FK enforces the 1:1 aggregate relationship |
| `FertilizerProducts` | `Id`, `TenantId`, `Name`, `NitrogenPercent (decimal(5,2))`, `PhosphorusPercent (decimal(5,2))`, `PotassiumPercent (decimal(5,2))`, `UnitCostPerKg (decimal(8,2), null)`, `SupplierId (uniqueidentifier, null, unconstrained seam)` | |
| `FertilizerApplications` | `Id`, `TenantId`, `FieldId (FK)`, `CropSeasonId (FK, null)`, `ProductId (FK)`, `RateKgPerHectare (decimal(8,2))`, `ApplicationDate (date)`, `Cost (decimal(10,2), null)`, `OperatorPersonId (FK, null)`, `InventorySourceRef (uniqueidentifier, null, unused seam)` | |
| `NutrientBalanceSnapshots` | `Id`, `TenantId`, `FieldId (FK)`, `SnapshotDate (date)`, `SensedNitrogen/Phosphorus/Potassium (decimal(6,2))`, `AppliedNitrogen/Phosphorus/PotassiumTrailing30d (decimal(8,2))`, `NitrogenStatus/PhosphorusStatus/PotassiumStatus (int, enum)` | Written by scheduled evaluator, not user input |

### 3.3 Indexes

| Table | Index | Purpose |
|---|---|---|
| `WeatherObservations` | `(TenantId, FacilityId, ObservedAt DESC)` | Dashboard's "current conditions" and history-range queries — the dominant access pattern |
| `WeatherForecastDaily` | `(TenantId, FacilityId, ForecastFor, GeneratedAt DESC)` | "Latest forecast for date X" lookup |
| `CropSeasons` | `(TenantId, FieldId, Status)` | Enforces/queries the "one active season per Field" invariant (Section 2.2) efficiently |
| `FertilizerApplications` | `(TenantId, FieldId, ApplicationDate DESC)` | Application log and trailing-30-day aggregation for `NutrientBalanceEvaluator` |
| `NutrientBalanceSnapshots` | `(TenantId, FieldId, SnapshotDate DESC)` | Trend chart queries |
| `GrowthStageEvents` | `(TenantId, CropSeasonId, ObservedDate)` | Timeline rendering per season |

All new tables carry the standard ABP audit/soft-delete columns (`IsDeleted`, `DeletionTime`, `CreationTime`, `CreatorUserId`, `LastModificationTime`, `LastModifierUserId`) consistent with `FullAuditedAggregateRoot<Guid>`, and all carry `TenantId` per `IMustHaveTenant` — **no new table opts out of the existing multi-tenant filter**, which is enforced automatically by ABP's tenant filter as long as `IMustHaveTenant` is implemented (matching `Node`/`NodeData`'s existing pattern), not something this design has to hand-build.

### 3.4 Foreign Keys & Referential Integrity

- All FKs to `Facility`, `Person`, `GeoFence`, `Alert` are standard EF Core FKs against the existing tables — no changes to those tables' schemas.
- `HarvestRecords.CropSeasonId` is a **unique** FK (one harvest per season, matching the domain model's `CropSeason.Harvest` single-reference property).
- `FertilizerApplications.CropSeasonId` and `EvapotranspirationReadings.CropSeasonId` are **nullable** FKs — both can exist without an active season (e.g., pre-planting soil prep fertilizer, or Et0 computed for a fallow Field).
- `FertilizerProducts.SupplierId` and `FertilizerApplications.InventorySourceRef` are **intentionally unconstrained** (`uniqueidentifier` with no FK constraint) — they're forward seams for Phase 3/5 modules that don't exist yet; adding a real FK constraint against a table that doesn't exist isn't possible, and a soft reference is the honest representation of "this will be a real relationship later."

### 3.5 Multi-Tenant Strategy

Unchanged from the existing platform: ABP's shared-database, discriminator-column multi-tenancy (`TenantId` on every row, automatic query filtering via `IMustHaveTenant`). No new tenancy model introduced. The one Phase 1-relevant nuance: `WeatherAlertRule` and `FertilizerProduct` are tenant-scoped, not shared across tenants even though weather providers and fertilizer product catalogs are logically the same data for every tenant in the same region — this is a deliberate simplicity choice for Phase 1 (avoid introducing a "shared reference data" concept this early); revisit as a possible shared-catalog optimization only if duplicate per-tenant fertilizer product entry proves to be real customer friction.

### 3.6 Migration Plan (design only — no migrations executed by this document)

Following the existing dated-migration naming convention exactly as seen in the repository's migration history:

1. `Added_Weather_Observations` — `WeatherObservations`, `WeatherForecastDaily`, `EvapotranspirationReadings`
2. `Added_Weather_Alert_Rules` — `WeatherAlertRules`, plus extend the existing `Alert` entity's `AlertSource` enum with `Weather` (an enum extension, not a schema change, unless `AlertSource` is stored as a string lookup table rather than an int — verify against the existing `Alert` entity before writing this migration)
3. `Added_Fields` — `Fields` table, FK to `Facilities` and to the existing `GeoFences` table
4. `Added_Crop_Reference_Data` — `CropTypes`, `SeedSuppliers`, `SeedVarieties`
5. `Added_Crop_Seasons` — `CropSeasons`, `GrowthStageEvents`, `HarvestRecords`
6. `Added_Fertilizer_Products` — `FertilizerProducts`
7. `Added_Fertilizer_Applications` — `FertilizerApplications`
8. `Added_Nutrient_Balance_Snapshots` — `NutrientBalanceSnapshots`

**Sequencing rationale:** each migration is scoped to one aggregate cluster, matching the granularity already visible in the existing migration history (e.g., `Added_Alerts`, `Added_Gis_GeoFences`, `Added_Operational_Monitoring` as separate migrations rather than one large one) — this keeps each migration independently reviewable and revertible, and mirrors how the team has already been shipping schema changes for this domain.

---

## 4. Application Layer

### 4.1 AppService Contracts

Following the existing `INodeAppService`/`NodeAppService` split (interface in a shared/contracts location, implementation attributed with `[AbpAuthorize]`) and the `AsyncCrudAppService<TEntity, TDto, TKey, TGetAllInput, TCreateInput, TUpdateInput>` base class where the module is straightforward CRUD, with hand-written services where it isn't.

```csharp
// Straightforward CRUD — use AsyncCrudAppService, matching NodeAppService's pattern
[AbpAuthorize(PermissionNames.Pages_Fields)]
public class FieldAppService : AsyncCrudAppService<Field, FieldDto, Guid, PagedFieldResultRequestDto, CreateFieldDto, FieldDto>, IFieldAppService
{
    // + GetByFacility(Guid facilityId) — not expressible as generic CRUD, custom method
}

[AbpAuthorize(PermissionNames.Pages_Crops)]
public class CropTypeAppService : AsyncCrudAppService<CropType, CropTypeDto, Guid, PagedResultRequestDto, CreateCropTypeDto, CropTypeDto>, ICropTypeAppService { }

[AbpAuthorize(PermissionNames.Pages_Crops)]
public class SeedVarietyAppService : AsyncCrudAppService<SeedVariety, SeedVarietyDto, Guid, PagedResultRequestDto, CreateSeedVarietyDto, SeedVarietyDto>, ISeedVarietyAppService { }

// Not plain CRUD — the aggregate has a lifecycle (Plant/LogGrowthStage/Harvest/Close), so this
// is a hand-written AppService in the NodeAppService style (custom methods calling domain
// factory/behavior methods), not AsyncCrudAppService.
public interface ICropSeasonAppService : IApplicationService
{
    Task<CropSeasonDto> Plant(PlantCropSeasonInput input);
    Task<CropSeasonDto> GetDetail(EntityDto<Guid> input);
    Task<PagedResultDto<CropSeasonDto>> GetByField(GetCropSeasonsByFieldInput input);
    Task<CropSeasonDto> LogGrowthStage(LogGrowthStageInput input);
    Task<CropSeasonDto> Harvest(HarvestCropSeasonInput input);
    Task<CropSeasonDto> Close(EntityDto<Guid> input);
    Task<List<CropRotationEntryDto>> GetRotationHistory(EntityDto<Guid> fieldId);   // computed query, Section 2.2 note
}

[AbpAuthorize(PermissionNames.Pages_Crops_Manage)]
public class CropSeasonAppService : ApplicationService, ICropSeasonAppService
{
    // constructor-injects IRepository<CropSeason, Guid>, IRepository<Field, Guid>, etc.
    // — matches NodeAppService's constructor-injection pattern; NOT IocManager.Instance.Resolve
    //   (Product Plan Section 9 flagged that anti-pattern in the existing NodeDataAppService —
    //   this design explicitly does not repeat it)
}

public interface IWeatherAppService : IApplicationService
{
    Task<WeatherCurrentDto> GetCurrent(EntityDto<Guid> facilityId);
    Task<List<WeatherForecastDto>> GetForecast(EntityDto<Guid> facilityId);           // next 7 days, latest GeneratedAt per ForecastFor
    Task<PagedResultDto<WeatherObservationDto>> GetHistory(GetWeatherHistoryInput input);
    Task<List<EvapotranspirationDto>> GetEvapotranspiration(GetEtInput input);
}

public interface IWeatherAlertRuleAppService : IApplicationService
{
    Task<WeatherAlertRuleDto> Create(CreateWeatherAlertRuleInput input);
    Task<List<WeatherAlertRuleDto>> GetForFacility(EntityDto<Guid> facilityId);
    Task Deactivate(EntityDto<Guid> input);
}

public interface IFertilizerAppService : IApplicationService
{
    Task<FertilizerProductDto> CreateProduct(CreateFertilizerProductInput input);
    Task<PagedResultDto<FertilizerProductDto>> GetProducts(PagedResultRequestDto input);
    Task<FertilizerApplicationDto> RecordApplication(RecordFertilizerApplicationInput input);
    Task<PagedResultDto<FertilizerApplicationDto>> GetApplicationsByField(GetApplicationsByFieldInput input);
}

public interface INutrientBalanceAppService : IApplicationService
{
    Task<NutrientBalanceSnapshotDto> GetLatest(EntityDto<Guid> fieldId);
    Task<List<NutrientBalanceSnapshotDto>> GetHistory(GetNutrientHistoryInput input);
}
```

### 4.2 DTO Conventions

Matches the existing `NodeDto`/`NodeDetailDto`/`CreateNode` split (a list/summary DTO, a richer detail DTO, and a distinct create-input DTO) — e.g., `CropSeasonDto` (list view: Field name, crop, status, planting date) vs. `CropSeasonDetailDto` (adds stage history, harvest record) vs. `PlantCropSeasonInput` (create-only fields, no `Id`/`Status`, which are server-assigned).

All input DTOs use standard `System.ComponentModel.DataAnnotations` attributes for shape validation (`[Required]`, `[Range]`, `[StringLength]`) consistent with the existing `Facility.Name`'s `[Required][StringLength(100, MinimumLength = 2)]` pattern — domain-level invariants (e.g., "expected harvest after planting") stay in the domain entity's factory methods (`DomainRuleException`), not duplicated into DTO validation, matching the existing separation of concerns already visible in `Facility`/`Node`.

### 4.3 Permissions (extends `PermissionNames.cs`)

```csharp
public const string Pages_Weather = "Pages.Weather";
public const string Pages_Weather_Configure = "Pages.Weather.Configure";

public const string Pages_Fields = "Pages.Fields";
public const string Pages_Fields_Manage = "Pages.Fields.Manage";

public const string Pages_Crops = "Pages.Crops";
public const string Pages_Crops_Manage = "Pages.Crops.Manage";
public const string Pages_Crops_Harvest = "Pages.Crops.Harvest";

public const string Pages_Nutrients = "Pages.Nutrients";
public const string Pages_Nutrients_Apply = "Pages.Nutrients.Apply";
```

Registered in the existing `AppAuthorizationProvider` (the ABP `AuthorizationProvider` implementation that presumably already registers `Pages_Nodes`, `Pages_Alerts`, etc. — not located in this pass, but the extension point is standard ABP and this design assumes it exists there rather than proposing a new registration mechanism).

### 4.4 Validation

**Governed by ADR-008** (`/docs/adr/ADR-008-Validation-Responsibility.md`), adopted platform-wide following Sprint 0's investigation (see `/docs/implementation/phase1/Sprint0-003-ValidationPipeline.md` for the raw findings that produced it): the original assumption that DTO validation already followed an established, `Facility.Name`-consistent pattern was wrong. `Facility.Name`'s `[Required][StringLength]` attributes are on the **domain entity**, not a DTO. Inspection of `CreateNode` and `CreateNodeData` found **zero validation attributes on any existing input DTO** — where checks exist at all, they're manual and inconsistent (`NodeDataAppService.CreateAsync` throws `ArgumentNullException` for a blank serial number, which is both the wrong exception type and — per Section 8.2's correction — does not reach the client as a clean error). Phase 1 **establishes** the convention below; it does not inherit one.

- **Shape validation:** standard `DataAnnotations` attributes (`[Required]`, `[StringLength]`, `[Range]`) on every new Phase 1 input DTO — ASP.NET Core/ABP's model binding honors these automatically, no new middleware required. This is additive to existing behavior, not a parallel system.
- **Cross-field checks that can't be a data annotation** (e.g., "expected harvest date after planting date"): explicit check at the top of the AppService method, thrown as `UserFriendlyException` directly — not `ArgumentNullException`, correcting rather than repeating `NodeDataAppService.CreateAsync`'s existing misuse.
- **Domain invariants:** inside entity factory/behavior methods (`DomainRuleException`), per Section 2 — unchanged, this part of the original assumption was correct.
- **Cross-aggregate invariants** (e.g., "one active `CropSeason` per `Field`"): explicit repository query inside the AppService method *before* calling the domain factory, since a single aggregate can't see its siblings — same pattern the existing `INodeSerialNumberAvailabilityChecker` domain service already uses for the analogous "serial number must be unique" cross-aggregate check on `Node`.
- **`DomainRuleException` handling at the AppService boundary:** confirmed in Sprint 0 (Section 8.2) — wrap in try/catch, rethrow as `UserFriendlyException(ex.Message)`, matching the existing convention used 24 times across the codebase (e.g. `NodeAppService.cs:139`).

### 4.5 Query Patterns — Pagination & Filtering

Reuses the existing `PagedResultRequestDto`/`PagedNodeResultRequestDto` convention (`SkipCount`, `MaxResultCount`, entity-specific filter properties) already evidenced by the Flutter client's literal query string (`SkipCount=0&MaxResultCount=1000`). New paged inputs (`PagedFieldResultRequestDto`, `GetCropSeasonsByFieldInput`, etc.) follow the same shape: inherit `PagedResultRequestDto`, add filter properties (`FacilityId`, `Status`, date ranges) as nullable optional filters, resolved via `System.Linq.Dynamic.Core`'s `.WhereIf(...)` pattern already used in `NodeAppService` (`GetAll().WhereIf(...)`).

---

## 5. Integration Layer

### 5.1 Weather Provider Abstraction

```csharp
public interface IWeatherProvider
{
    Task<WeatherProviderCurrentResult> GetCurrentAsync(decimal latitude, decimal longitude);
    Task<List<WeatherProviderForecastDayResult>> GetForecastAsync(decimal latitude, decimal longitude, int days = 7);
    Task<List<WeatherProviderHistoricalResult>> GetHistoryAsync(decimal latitude, decimal longitude, DateTime from, DateTime to);
}
```
Registered via ABP's standard DI (`IocManager` conventional registration or explicit `IocManager.IocContainer.Register(...)` in the module's `PreInitialize`) — concrete implementation (`OpenWeatherMapProvider`, or whichever provider ADR-001 selects) lives in `Farmru.IotMonitoring.Application` or a new `Farmru.IotMonitoring.Integrations` project, injected wherever `IWeatherProvider` is needed. This satisfies ADR-001's swappability requirement structurally, independent of which provider ADR-001 ultimately picks.

**Open item flagged, not resolved by this document:** ADR-001 (weather provider selection) must be closed before this interface's concrete implementation is written — this section defines the *contract*, not the vendor.

### 5.2 Background Jobs

**Resolved in Sprint 0 (see `/docs/implementation/phase1/Sprint0-001-BackgroundJobs.md`):** no third-party job framework (Hangfire, Quartz, ABP's `IBackgroundJobManager`) exists. `MonitoringExecutionHistory`'s cycle is driven by `OperationalMonitoringHostedService` — a plain .NET Generic Host `BackgroundService` in `Web.Host`, registered via `services.AddHostedService<...>()` in `Startup.cs`, looping on a fixed `Task.Delay` interval and iterating active tenants per cycle inside `OperationalMonitoringEngine.RunFullMonitoringCycleAsync()`.

**Decision:** extend this exact pattern rather than introducing a second scheduling paradigm. Four sibling `BackgroundService` classes are added:
1. `WeatherSyncHostedService` (hourly) → `IWeatherSyncEngine`
2. `WeatherAlertEvaluationHostedService` (15 min) → `IWeatherAlertEvaluationEngine`
3. `EvapotranspirationHostedService` (daily) — folded into the weather engine or its own minimal engine, decided during Sprint 1 implementation
4. `NutrientBalanceHostedService` (daily) → `INutrientBalanceEvaluationEngine`

Each engine interface lives in the Application layer and follows `OperationalMonitoringEngine`'s per-tenant unit-of-work loop shape exactly. **Known, accepted limitation:** no distributed lock/single-instance guarantee, matching the existing pattern's constraint — fine under the current single-instance deployment, revisit only if the platform is horizontally scaled.

**Job list for Phase 1:**
| Job | Frequency | Action |
|---|---|---|
| `WeatherSyncJob` | Hourly, per active Facility | Calls `IWeatherProvider`, writes `WeatherObservation` + refreshes `WeatherForecastDaily` (new `GeneratedAt` row, per append-only rule) |
| `WeatherAlertEvaluationJob` | After each `WeatherSyncJob` run (chained) or every 15 min | Evaluates active `WeatherAlertRule`s against latest observation/forecast, raises `Alert` on breach |
| `EvapotranspirationCalculationJob` | Daily | Computes `Et0` (and `Etc` where an active `CropSeason` exists) from the day's weather observations |
| `NutrientBalanceEvaluationJob` | Daily, per `Field` with an active `CropSeason` | Reads latest `NodeData` for Nodes in the Field's Facility + trailing-30-day `FertilizerApplication` sum, writes `NutrientBalanceSnapshot`, raises `NutrientDeficiencyDetectedEvent` on status change to `Deficient` |

### 5.3 SignalR Updates

**No new hubs.** Reuses the existing `AlertNotificationHub` for weather-driven alerts (they're just `Alert` rows with `AlertSource = Weather`, per Section 2.1 — the existing hub's tenant-group push logic doesn't need to know the difference). No Phase 1 module needs a dedicated real-time channel of its own — Crop Management and Fertilizer are inherently lower-frequency, user-driven-write workflows, not telemetry streams, so they're served fine by standard request/response plus the existing alert channel for the one case (weather alerts) that is genuinely push-worthy.

### 5.4 Event Publishing

Uses ABP's local event bus (`IEventBus`/`Abp.Events.Bus`) — domain entities raise events via the standard ABP `DomainEvents` pattern (e.g., `CropSeason.Harvest(...)` internally calls something like `DomainEvents.EventBus.Trigger(new CropSeasonHarvestedEvent(...))`, or the AppService publishes after a successful `SaveChangesAsync`, depending on which convention the existing codebase already follows for `Alert`/`Incident` — **verify against the existing `Incident` workflow's event-raising code in Sprint 1** before choosing which of the two ABP patterns to follow, since consistency with what's already there matters more than which pattern is theoretically better).

Events defined in Phase 1 (all currently have **no subscribers** — they exist so Phase 2+/the future Timeline module can subscribe without Phase 1 code needing to change):
`WeatherObservationRecordedEvent`, `WeatherAlertRaisedEvent`, `CropSeasonPlantedEvent`, `GrowthStageLoggedEvent`, `CropSeasonHarvestedEvent`, `FertilizerAppliedEvent`, `NutrientDeficiencyDetectedEvent`.

### 5.5 Caching Strategy

- **Weather forecast caching:** `WeatherAppService.GetForecast` reads from the already-persisted `WeatherForecastDaily` table (populated by `WeatherSyncJob`), never calls `IWeatherProvider` directly on a user request — this is the primary caching mechanism and also the cost-control mechanism flagged in Product Plan Section 19 (provider cost scales with Facility count, not with user request volume, since sync frequency is fixed and independent of how many users look at the dashboard).
- **Reference data caching:** `CropType`, `SeedVariety`, `SeedSupplier`, `FertilizerProduct` are low-write-frequency lookup tables — candidates for ABP's built-in entity/output caching (`[CacheAggregateSource]`-style or `ICacheManager`, whichever convention the existing codebase uses for similar lookup data, e.g. how `Person`'s title/gender reflists are cached, if they are — confirm in Sprint 1) to reduce redundant DB round-trips from the Angular dropdown/autocomplete pattern already established (the Flutter app's `autocomplete_services.dart` suggests a similar pattern already exists client-side and could hint at the server-side convention too).

---

## 6. Angular Technical Design

### 6.1 Module Structure

Mirrors the existing `node` module exactly (own `NgModule`, own routing module, `SharedModule` + `ServiceProxyModule` imports, feature components as declarations):

```
angular/src/app/
├── weather/
│   ├── weather.module.ts
│   ├── weather-routing.module.ts
│   ├── weather-dashboard/              (current conditions + 7-day forecast, per Facility)
│   │   ├── weather-dashboard.component.ts/html
│   ├── weather-history/                (time series + export)
│   └── weather-alert-rules/            (CRUD for WeatherAlertRule, mirrors edit-node's form pattern)
├── fields/
│   ├── fields.module.ts
│   ├── fields-routing.module.ts
│   ├── field-list/
│   ├── create-field/                   (includes the polygon-draw step, reusing GIS module's existing Leaflet + geofence editing component rather than building a second map-draw widget)
│   └── edit-field/
├── crops/
│   ├── crops.module.ts
│   ├── crops-routing.module.ts
│   ├── crop-season-wizard/             (step wizard: crop type -> variety -> supplier -> planting -> population, per Product Plan 13.6)
│   ├── crop-season-detail/             (growth stage timeline, harvest entry)
│   ├── planting-calendar/              (cross-Facility calendar view)
│   └── crop-reference-data/            (admin CRUD for CropType/SeedVariety/SeedSupplier)
└── nutrients/
    ├── nutrients.module.ts
    ├── nutrients-routing.module.ts
    ├── fertilizer-application-log/
    ├── record-application/
    └── nutrient-balance-dashboard/     (reuses existing Ng2GoogleChartsModule gauge/line pattern from agricultural-dashboard, per Product Plan 13.4's "one shared chart language" recommendation — not a new charting approach)
```

Each module is lazy-loaded from `app-routing.module.ts` exactly as `node`/`alerts`/`gis`/`incidents` already are, and slots into the **Field Operations** and **Agronomy Intelligence** IA zones defined in Product Plan Section 4 (this is the first real test of that IA restructure — Phase 1 should migrate the top-level nav to the zoned structure now, while there are only 4 new modules to place, rather than waiting until there are 20).

### 6.2 Routing

```typescript
// crops-routing.module.ts — same RouterModule.forChild + AppRouteGuard shape as node-routing.module.ts
RouterModule.forChild([
    { path: '', component: CropSeasonListComponent, canActivate: [AppRouteGuard], data: { permission: 'Pages.Crops' } },
    { path: 'plant', component: CropSeasonWizardComponent, canActivate: [AppRouteGuard], data: { permission: 'Pages.Crops.Manage' } },
    { path: 'detail/:id', component: CropSeasonDetailComponent, canActivate: [AppRouteGuard], data: { permission: 'Pages.Crops' } },
])
```
(`node-routing.module.ts` as inspected doesn't show `canActivate`/`data` on its child routes — this design adds that guard consistently at the child-route level rather than only at the parent, since Crop Management has a genuine manage-vs-view permission split that Node's routing, as currently written, doesn't yet enforce at this granularity. Worth backporting to `node-routing.module.ts` for consistency, flagged as a small cleanup item, not required for Phase 1 sign-off.)

### 6.3 Components & State Management

No NgRx or other global state library is evidenced in the existing app (`node.module.ts`'s imports show `ServiceProxyModule` + component-local state, consistent with ABP Angular's default template). Phase 1 modules follow the same convention: **service-proxy-backed, component-local state**, with a thin Angular service per module (e.g., `CropSeasonStateService`) only where a wizard genuinely needs to hold in-progress multi-step form state across steps (the `Step Wizard` component from Product Plan Section 27) — not a general state-management introduction.

### 6.4 Services

Auto-generated service proxies (NSwag, via the existing `ServiceProxyModule` pattern) for every new AppService in Section 4 — no hand-written HTTP services, consistent with how `node.module.ts` consumes `NodeServiceProxy` today (inferred from the `ServiceProxyModule` import; the actual generated proxy class wasn't directly inspected in this pass, but the pattern is standard ABP Angular scaffolding, regenerated via the existing `nswag` folder already present at `angular/nswag`).

### 6.5 Guards

Reuses `AppRouteGuard` (`@shared/auth/auth-route-guard`) unmodified — no new guard type needed, only new `Pages.*` permission strings (Section 4.3) fed into existing routes.

### 6.6 Responsive Layouts & Reusable Components

Per Product Plan Sections 13/27/28: the **Step Wizard**, **KPI Tile**, **Weather Widget**, and **Trend/Line Chart Card** components are net-new shared components with no current equivalent in the codebase and should be built once in `shared/` (not inside the `weather`/`crops`/`nutrients` module folders) so Phase 2+ modules can consume them immediately — this is the concrete first delivery against Product Plan Section 27's "build the shared library starting Phase 1, Sprint 1" recommendation, not a future promise.

---

## 7. Flutter Technical Design

### 7.1 Scope Decision for Phase 1 Mobile

Per Product Plan Section 5.17, the mobile app's priority gaps (real map, push notifications, offline sync reliability) are **Phase 3** work. Phase 1 mobile scope is deliberately narrow: **read-only visibility** into Weather and Crop/Nutrient data for a technician already in the field, not full CRUD parity with web. Building Crop Season wizards or Fertilizer application entry on mobile before the offline-sync foundation (Phase 3) exists risks exactly the field-data-loss risk flagged in Product Plan Section 19.

### 7.2 Navigation & Screen Hierarchy

Follows the existing GetX-based navigation and the existing `home_screen.dart` drawer/bottom-nav shell — no new navigation framework introduced.

```
lib/screens/
├── weather/                            (new)
│   └── weather_screen.dart             (current conditions + forecast for the technician's assigned Facility/Node context — mirrors AverageReadings.dart's card layout)
└── crops/                              (new)
    ├── crop_season_list_screen.dart    (read-only list, per Field)
    └── crop_season_detail_screen.dart  (read-only stage timeline + latest nutrient snapshot — reuses fl_chart per NodeDetails.dart's existing chart pattern)
```
No Fertilizer application-entry screen in Phase 1 mobile (write path stays web-only until the offline-sync foundation exists, per 7.1).

### 7.3 Offline Synchronization & Local Storage

Since Phase 1 mobile is read-only for these modules, this is simpler than the existing `AlertService`/`IncidentService` offline-queue pattern (no `_pendingActions` needed, since there's nothing to queue). Data is fetched on screen load and optionally cached via `shared_preferences` (matching `UserSettings`'s existing use) for a "last known" display if the device is offline — not a true offline-first sync engine, which is explicitly deferred to Phase 3 per Product Plan 5.17 and 12.

### 7.4 Maps & QR Scanning

Not in Phase 1 scope (Field boundary drawing is a web-only, GIS-module-reused capability per Section 6.1; QR scanning is Phase 3 per Product Plan 5.17). No new Flutter map/camera dependency is added in Phase 1 — avoids taking on the "placeholder map" risk (Product Plan's audit finding on `operational_map_screen.dart`) a second time in a new module before the real map SDK decision (Phase 3) is made.

### 7.5 Services (Dart)

```dart
class WeatherService {
  static Future<WeatherCurrentResponse?> GetCurrent(String facilityId) async {
    var response = await BaseClient().get('api/services/app/Weather/GetCurrent?facilityId=$facilityId');
    // ... same jsonDecode + typed-model pattern as NodeService.GetSensorData
  }
  static Future<List<WeatherForecastResponse>?> GetForecast(String facilityId) async { /* ... */ }
}

class CropSeasonService {
  static Future<List<CropSeasonResponse>?> GetByField(String fieldId) async { /* ... */ }
  static Future<CropSeasonDetailResponse?> GetDetail(String cropSeasonId) async { /* ... */ }
}
```
Exactly matches `NodeService`'s static-method-plus-`BaseClient`-plus-typed-model-parsing shape — no new HTTP client pattern introduced.

### 7.6 Widget Tree (representative — Weather screen)

```
WeatherScreen (StatefulWidget, GetX-managed)
└── Scaffold
    ├── AppBar (consistent with existing screens' header pattern)
    └── body: RefreshIndicator
        └── ListView
            ├── CurrentConditionsCard   (new shared widget: temp/humidity/wind, mirrors AverageReadings.dart's card style)
            ├── ForecastStrip           (new shared widget: 7-day horizontal scroll of mini-cards)
            └── EtWidget                (new: today's Et0/Etc, small stat card)
```

---

## 8. API Contracts

All endpoints follow the existing ABP dynamic web API convention confirmed in the Flutter client: `GET/POST api/services/app/{ServiceName}/{MethodName}`, response envelope `{ result: <payload>, success: true, error: null, ... }` (ABP's standard `AjaxResponse` wrapper). New endpoints are added to Swagger automatically via the existing Swashbuckle setup in `Web.Host` — no separate OpenAPI authoring effort.

### 8.1 Representative Request/Response Schemas

```
GET  api/services/app/Weather/GetForecast?facilityId={guid}
200 → { result: [ { forecastFor: "2026-08-01", tempMinCelsius: 8.5, tempMaxCelsius: 22.1,
                     precipitationProbabilityPercent: 30, frostRisk: "None", heatStress: "None" }, ... ],
        success: true, error: null }

POST api/services/app/CropSeason/Plant
Body: { fieldId: "guid", cropTypeId: "guid", seedVarietyId: "guid" | null,
        plantingDate: "2026-08-01", expectedHarvestDate: "2026-11-15",
        expectedYieldKg: 4500.0 | null, plantPopulationPerHectare: 65000 | null }
200 → { result: { id: "guid", status: "Planned", ... }, success: true, error: null }
400 (domain rule violated, e.g. harvest date before planting) →
     { result: null, success: false,
       error: { message: "Expected harvest date must be after planting date.", details: null, validationErrors: null } }

POST api/services/app/FertilizerApplication/RecordApplication
Body: { fieldId: "guid", cropSeasonId: "guid" | null, productId: "guid",
        rateKgPerHectare: 120.0, applicationDate: "2026-08-10", cost: 850.00 | null, operatorPersonId: "guid" | null }
200 → { result: { id: "guid", ... }, success: true, error: null }
```

### 8.2 Error Model

**Governed by ADR-008** (`/docs/adr/ADR-008-Validation-Responsibility.md`); raw investigation in `/docs/implementation/phase1/Sprint0-002-ExceptionPipeline.md`: `DomainRuleException` does **not** derive from `UserFriendlyException` and no global exception filter maps it — but the codebase has a consistent, established **manual per-call-site convention**, used 24 times across `NodeAppService`, `AlertAppService`, `IncidentAppService`, `FacilityAppService`, and others:
```csharp
try { /* call a domain method that may throw DomainRuleException */ }
catch (DomainRuleException ex) { throw new UserFriendlyException(ex.Message); }
```
Every new Phase 1 AppService method calling a `DomainRuleException`-capable domain method (`CropSeason.Plant`, `Field.Create`, `WeatherAlertRule.Create`, `FertilizerApplication.Apply`, etc.) must follow this exact pattern. This is now a confirmed, mandatory convention, not an open question — the standard ABP error envelope (`error.message` set from `UserFriendlyException`'s message, HTTP 400-class) is reached correctly as long as this wrap is present. (A global exception filter to remove the per-call-site repetition risk is a reasonable follow-up, but is explicitly out of Phase 1 scope — see the Sprint 0 record for the reasoning.)

### 8.3 Versioning

Per Product Plan Section 24: internal AppServices (everything in this document) stay unversioned/coordinated-deploy, consistent with current practice — Phase 1 introduces no external-facing API surface, so the device-ingestion versioning work (Section 24's actual priority item) is tracked as the separate hardening item in Section 1.1, not duplicated here.

---

## 9. Acceptance Criteria

### 9.1 Functional Acceptance (representative Given/When/Then, one per module)

**Weather:**
```
Given a Facility with valid latitude/longitude
When the WeatherSyncJob runs for that Facility
Then a WeatherObservation is recorded with ObservedAt within the last hour
  And a WeatherForecastDaily row exists for each of the next 7 days with the current GeneratedAt timestamp

Given a WeatherAlertRule for Frost with ThresholdValue = 2.0°C on a Facility
When a WeatherForecastDaily is recorded with TempMinCelsius = 1.0°C for that Facility
Then an Alert is created with AlertSource = Weather and Severity matching the rule
  And the Alert appears via the existing AlertNotificationHub within 2 seconds (Product Plan Section 26 target)
```

**Crop Management:**
```
Given a Field with no open CropSeason
When a user plants a CropSeason with a valid CropType and PlantingDate before ExpectedHarvestDate
Then the CropSeason is created with Status = Planned
  And a GrowthStageEvent for stage "Planted" is automatically logged

Given a Field with an existing CropSeason in Status = Growing
When a user attempts to plant a second CropSeason on the same Field
Then the request is rejected with a domain-rule error, no new CropSeason is created

Given a CropSeason in Status = Growing
When a user records a Harvest with ActualYieldKg
Then the CropSeason transitions to Status = Harvested
  And GetRotationHistory(fieldId) includes this season ordered correctly against prior closed seasons
```

**Fertilizer & Nutrient Management:**
```
Given a Field's Facility has Nodes reporting NPK telemetry
When the NutrientBalanceEvaluationJob runs for that Field
Then a NutrientBalanceSnapshot is written reflecting sensed values and trailing-30-day applied totals
  And unparseable NodeData string values are skipped, not thrown, per Section 2.3's data-quality note

Given a NutrientBalanceSnapshot's NitrogenStatus flips from Adequate to Deficient
When the evaluator completes
Then a NutrientDeficiencyDetectedEvent is published (verifiable via event bus subscriber in tests, even though no Phase 1 subscriber consumes it yet)
```

### 9.2 UX Acceptance

- Every new list/dashboard screen has a designed empty state (Product Plan 13.9) — e.g., "No Fields yet" with a direct "Add Field" CTA, not a blank table.
- Crop Season creation uses the shared Step Wizard component (Section 6.6), not a single long form.
- Weather and Nutrient dashboards use the shared chart/gauge visual language already established by `agricultural-dashboard` (Product Plan 13.4) — no new, inconsistent chart styling introduced.
- All new status indicators (`CropSeasonStatus`, `NutrientBalanceStatus`, `FrostRisk`/`HeatStress`) render with icon + text, not color alone (Product Plan 13.8 WCAG requirement).

### 9.3 Performance Targets (Phase 1 slice of Product Plan Section 26)

| Target | Value |
|---|---|
| Weather dashboard load (Facility-scoped) | < 2s P95 |
| `WeatherSyncJob` full run (all active Facilities) | Completes within its scheduling interval with headroom (i.e., an hourly job must reliably finish well under 60 min at Phase 1's expected Facility count) |
| Crop Season list/detail load | < 2s P95 |
| `NutrientBalanceEvaluationJob` per-Field evaluation | < 5s per Field, parallelizable across Fields |

### 9.4 Security Requirements (Phase 1 slice of Product Plan Section 25)

- Device authentication hardening on `NodeDataAppService.CreateAsync` (Section 1.1) ships in Phase 1 — this is a **release-blocking** item for Phase 1 sign-off, not optional cleanup, since Section 5.3's `NutrientBalanceEvaluationJob` (and every Phase 2+ module after it) inherits `NodeData`'s trustworthiness. **Audited and designed in Sprint 0** (see `/docs/implementation/phase1/Sprint0-005-DeviceAuthentication.md`): confirmed zero authorization on the endpoint today and no device-credential concept anywhere in the domain model. Design: `Node.ApiKey` generated at registration, header-based (`X-Device-Key`) validation in `CreateAsync`, `RotateApiKey()` domain method, migration + backfill for existing Nodes, plus endpoint-scoped rate limiting. **Enforcement rollout is gated on confirming how a credential reaches already-deployed field devices** — a provisioning/ops decision, not a code question; the schema/code work can proceed in Sprint 1 regardless.
- All new AppServices carry explicit `[AbpAuthorize(PermissionNames.X)]` attributes — no endpoint ships with authorization commented out, unlike the pre-existing `NodeDataAppService` finding.
- New external integration (`IWeatherProvider`'s concrete implementation) API key: **no secrets-management mechanism currently exists** (confirmed in Sprint 0 — `appsettings.json` holds only a plaintext connection string, no Key Vault or equivalent integration found). Establishing a real secrets store is now a prerequisite for Sprint 2, not an assumed-solved item — track alongside ADR-001's weather provider selection (Section 21).

---

## 10. Implementation Plan

### 10.1 Sprint Breakdown (2-week sprints, 6 sprints ≈ 12 weeks for Phase 1)

| Sprint | Focus | Key deliverables |
|---|---|---|
| **Sprint 0 (spike, not full sprint) — COMPLETE** | Grounding/open-item resolution | Confirmed background-job framework (5.2 — extend existing `BackgroundService` pattern), confirmed+corrected `DomainRuleException`→HTTP mapping (8.2 — existing manual convention, adopt it), corrected DTO validation pipeline (4.4 — no prior convention existed, Phase 1 establishes one), audited+designed device auth hardening (9.4 — pending field-rollout confirmation), narrowed ADR-001 to 6 vendor criteria (owner decision still open). Full findings: `/docs/implementation/phase1/Sprint0-Report.md` |
| **Sprint 1** | Domain + DB foundation | All Section 2 entities implemented, all Section 3 migrations written and applied to a dev DB, device-auth hardening (9.4) started |
| **Sprint 2** | Weather module, backend | `IWeatherProvider` + concrete implementation, `WeatherSyncJob`, `WeatherAlertEvaluationJob`, `WeatherAppService`, `WeatherAlertRuleAppService`; device-auth hardening completed |
| **Sprint 3** | Weather module, frontend + Field | Angular `weather` module, Angular `fields` module, Flutter `weather` screen |
| **Sprint 4** | Crop Management, backend + frontend | `CropSeasonAppService` + reference-data AppServices, Angular `crops` module (wizard, detail, calendar), Flutter read-only crop screens |
| **Sprint 5** | Fertilizer & Nutrient Management | `FertilizerAppService`, `NutrientBalanceEvaluationJob`, `NutrientBalanceAppService`, Angular `nutrients` module |
| **Sprint 6** | Hardening, cross-cutting, acceptance | Shared component library polish (Step Wizard, KPI Tile, Weather Widget), event-publishing wiring (5.4), full Section 9 acceptance pass, performance validation against 9.3 targets |

### 10.2 Effort Estimates (rough, team-size-dependent — directional t-shirt sizing per deliverable, not a committed estimate)

| Deliverable | Size |
|---|---|
| Domain entities + migrations (Section 2/3) | L |
| Device auth hardening | M — but schedule risk if ADR-004-equivalent protocol questions reopen; scope here is auth only, not protocol migration |
| Weather backend (provider abstraction + jobs + AppServices) | L |
| Crop Management backend | L |
| Fertilizer backend | M |
| Angular: weather + fields modules | M |
| Angular: crops module (incl. Step Wizard build) | L (first use of the new shared Step Wizard component — expect it to take longer than subsequent modules that reuse it) |
| Angular: nutrients module | M |
| Flutter: read-only weather + crop screens | S |
| Shared component library (KPI Tile, Weather Widget, Trend Chart Card) | M, but front-loaded value — every later phase gets cheaper because of this |

### 10.3 Dependencies

- Sprint 2 (Weather backend) blocked on ADR-001 (weather provider) resolution.
- Sprint 1 blocked on Sprint 0's background-job-framework finding, since entity design for `WeatherForecastDaily`'s append-only pattern is fine independent of it, but job scheduling code in Sprint 2 is not.
- Angular `crops` module's Step Wizard (Sprint 4) blocked on nothing external, but should not start before Sprint 3's Angular Weather module ships, since Weather is the simpler module and should surface any Angular-side integration surprises (service proxy generation, `AppRouteGuard` permission wiring) first, cheaply.
- Flutter work (Sprint 3-5) is independent of Angular work and can run in parallel with a separate mobile developer/pair, per the existing team split implied by the codebase having distinct Angular and Flutter codebases already maintained.

### 10.4 Risks (Phase-1-specific, narrower than Product Plan Section 19)

| Risk | Mitigation |
|---|---|
| Background-job framework unknown (Section 5.2) | Sprint 0 spike is the direct mitigation — this is called out as the single highest-uncertainty item in this whole design |
| `NodeData` string-typed sensor fields (Section 2.3) complicate `NutrientBalanceEvaluationJob` | Defensive parsing now; recommend (don't require) a follow-up migration to numeric types, filed as a separate backlog item, not blocking Phase 1 |
| Weather provider cost at scale unknown until ADR-001 closes | Get provider pricing before Sprint 2 starts, not after — a Product Plan Section 19 risk this design inherits directly |
| `DomainRuleException` → HTTP error mapping unverified (Section 8.2) | Sprint 0 spike; if unmapped, this is a small but necessary addition to the exception-handling middleware, done once, benefiting every future module too |
| Step Wizard being built for the first time inside a real feature (crops module) rather than in isolation | Accept the Sprint 4 estimate risk (flagged as L, "expect longer") rather than pretending it's a known quantity |

### 10.5 Definition of Done (per module)

- [ ] Domain entities implemented with factory methods + `DomainRuleException` guards per Section 2's invariants
- [ ] EF Core migration written, applied, and reviewed (matches Section 3.6's naming convention)
- [ ] AppService(s) implemented with correct `[AbpAuthorize]` attributes (Section 4.3 permissions registered)
- [ ] Angular module built, lazy-loaded, routed with `AppRouteGuard`, using shared components where specified (Section 6)
- [ ] Flutter screens built where in Phase 1 mobile scope (Section 7.1's read-only boundary respected)
- [ ] Domain events published (Section 5.4), even with zero current subscribers
- [ ] All Section 9.1 acceptance scenarios pass (automated test where the existing test project structure supports it, manual verification otherwise — the existing `aspnet-core/test` project's current coverage of `Node`/`Facility` wasn't audited in this pass and should set the baseline expectation for new-module test coverage, not a different, lower bar)
- [ ] Section 9.2 UX acceptance checked against a real screen, not just code review (empty states, status-badge icon+text, chart consistency)
- [ ] Section 9.3 performance targets validated against a realistic dataset, not an empty dev DB
- [ ] No `[AbpAuthorize]` omitted or commented out anywhere in new code (Section 9.4)

---

*End of Phase 1 Technical Design. Scope is fixed to Weather Intelligence, Crop Management, and Fertilizer & Nutrient Management, per Product Plan v1.0 Sections 5.1–5.3. Any additional module pulled into Phase 1 requires a change request against the Product Plan before it can be added here.*
