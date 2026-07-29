# Sprint 0-001 — Background Job Infrastructure

## What was assumed
The Phase 1 Technical Design (Section 5.2) refused to assume a background-job mechanism existed and flagged this as the single highest-uncertainty item, requiring a Sprint 0 spike before `WeatherSyncJob`, `WeatherAlertEvaluationJob`, `EvapotranspirationCalculationJob`, and `NutrientBalanceEvaluationJob` could be scheduled.

## What was found in the code
No Hangfire, Quartz, or ABP `IBackgroundJobManager`-based recurring job exists anywhere in the solution. The existing `MonitoringExecutionHistory`/`OperationalMonitoringEngine` cycle (which drives the current Monitoring module) is triggered by a plain **.NET Generic Host `BackgroundService`**:

- `Farmru.IotMonitoring.Web.Host/Monitoring/OperationalMonitoringHostedService.cs` — a `BackgroundService` subclass registered via `services.AddHostedService<OperationalMonitoringHostedService>()` in `Startup.cs` (line 50).
- It waits 45s on startup, then loops: creates a DI scope, resolves `IOperationalMonitoringEngine`, opens a unit of work, calls `RunFullMonitoringCycleAsync()`, catches and logs any exception, then `Task.Delay(TimeSpan.FromMinutes(5))` before repeating.
- `OperationalMonitoringEngine.RunFullMonitoringCycleAsync()` (in the Application layer) iterates all active tenants, opens a per-tenant unit of work with `SetTenantId(tenantId)`, and runs the monitoring sub-jobs (device-offline, telemetry health, alert escalation, etc.), recording each run in `MonitoringExecutionHistory`.
- No distributed lock, leader election, or single-instance guarantee exists — the pattern assumes exactly one running instance of Web.Host. This is consistent with the current single-instance deployment but is a real constraint if the platform is ever horizontally scaled (relevant to Product Plan Section 15's job-scheduling capacity plan, not a Phase 1 blocker).
- Aside, noticed but out of scope: `App_Data/Logs/Logs.txt.1` contains a stack trace through `OperationalMonitoringEngine.RunFullMonitoringCycleAsync` at line 58 from a prior failure. Not investigated further here — worth a five-minute look by whoever owns Monitoring, unrelated to Phase 1 scope.

## Decision
**Reuse the exact same pattern.** Add sibling `BackgroundService` classes in `Farmru.IotMonitoring.Web.Host/Weather/` and `.../Nutrients/`, following `OperationalMonitoringHostedService`'s shape exactly (DI scope per cycle, per-tenant unit-of-work loop, try/catch-and-log, fixed `Task.Delay` interval):

- `WeatherSyncHostedService` — interval `TimeSpan.FromHours(1)`, resolves a new `IWeatherSyncEngine` (Application layer), which internally iterates active Facilities per tenant (mirroring `OperationalMonitoringEngine`'s per-tenant loop) and calls `IWeatherProvider`.
- `WeatherAlertEvaluationHostedService` — interval `TimeSpan.FromMinutes(15)`, resolves `IWeatherAlertEvaluationEngine`.
- `EvapotranspirationHostedService` — interval `TimeSpan.FromHours(24)` (run once daily; exact time-of-day is not controllable with a plain `Task.Delay` loop — acceptable for Phase 1, see note below).
- `NutrientBalanceHostedService` — interval `TimeSpan.FromHours(24)`, resolves `INutrientBalanceEvaluationEngine`.

No new third-party dependency (Hangfire/Quartz) is introduced. This is a deliberate simplicity choice, not an oversight: the existing pattern already solves "run this on a schedule, once, iterating tenants" adequately for Phase 1's four jobs, and introducing a second scheduling paradigm alongside the one already in production would add inconsistency for no immediate benefit.

**Known limitation accepted for Phase 1, flagged for later:** `Task.Delay`-based intervals drift from wall-clock time (a service restart resets the daily jobs' effective time-of-day) and provide no fixed "run at 02:00 UTC" scheduling. If a specific time-of-day matters for a later job (e.g., a nightly Disease Risk job in Phase 2), that is the point to introduce a proper cron-capable scheduler — not retrofitted into Phase 1's four jobs.

## Does the Technical Design change?
No structural change. Section 5.2 update: replace "two viable options, to be resolved in Sprint 1" with the confirmed decision above — the Technical Design's own escape hatch ("if no framework exists yet, ABP's `IBackgroundJobManager`...") is superseded, since a framework (the existing `BackgroundService` pattern) does exist and is the correct one to extend.

## Implementation tasks resulting
- [ ] Create `IWeatherSyncEngine`, `IWeatherAlertEvaluationEngine`, `INutrientBalanceEvaluationEngine` interfaces + implementations in the Application layer, following `OperationalMonitoringEngine`'s per-tenant unit-of-work loop shape.
- [ ] Create the four `BackgroundService` classes in Web.Host, register in `Startup.cs` alongside the existing `AddHostedService<OperationalMonitoringHostedService>()` call.
- [ ] No `EvapotranspirationCalculationJob` engine interface needed as a separate service — fold into `WeatherSyncHostedService`'s daily companion or its own minimal engine; decide during Sprint 1 implementation, not a Sprint 0 blocker.
