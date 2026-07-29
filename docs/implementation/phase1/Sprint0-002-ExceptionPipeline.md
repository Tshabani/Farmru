# Sprint 0-002 — DomainRuleException → HTTP Error Pipeline

## What was assumed
The Technical Design flagged as an open question whether `DomainRuleException` (a plain `Exception` subclass used throughout the domain layer for invariant violations) reliably surfaces as a clean, user-facing HTTP error, or whether it risks being swallowed into a generic 500 by ABP's default exception handling — since `DomainRuleException` does **not** derive from `Abp.UI.UserFriendlyException`, which is what ABP's default pipeline treats specially.

## What was found in the code
`DomainRuleException` (`Farmru.IotMonitoring.Core/Domains/DomainRuleException.cs`) is confirmed to be a plain `Exception` subclass with no special handling registered anywhere (no custom `IExceptionFilter`, no `AbpExceptionFilter` override, no global `IExceptionSubscriber` found in the solution).

However, the codebase already has an established, **consistently applied convention** for this exact problem, used in essentially every AppService that calls a domain method capable of throwing it — 24 occurrences across `TaskManagementAppService`, `FacilityAppService`, `AlertAppService`, `FacilityAppointmentAppService`, `PersonAppService`, `IncidentAppService`, `NodeAppService`, `MonitoringAppService`, `OrganisationAppService`, `GeoSpatialAppService`:

```csharp
try
{
    var history = node.ReplaceSerialNumber(input.NewSerialNumber, input.Reason, input.Notes);
    // ...
}
catch (DomainRuleException ex)
{
    throw new UserFriendlyException(ex.Message);
}
```
(verbatim, from `NodeAppService.cs:139`)

Every AppService method that can trigger a domain invariant violation wraps the call and manually re-throws as `UserFriendlyException(ex.Message)`, which **is** handled specially by ABP's default pipeline and does surface cleanly to the client as a 400-class error with the original message intact.

This is the opposite of what the Technical Design worried about: there is no *missing* pipeline — there is a **manual, per-call-site convention**, applied consistently by the existing team, rather than a global filter. It works, but it is easy to forget on a new method (nothing enforces it structurally — a developer who calls a domain factory method without the try/catch simply gets a masked 500 with no compiler or runtime warning).

## Decision
**Adopt the existing convention exactly, in every new Phase 1 AppService method that calls a `DomainRuleException`-capable domain method** (`CropSeason.Plant`, `.LogGrowthStage`, `.Harvest`, `Field.Create`, `WeatherAlertRule.Create`, `FertilizerApplication.Apply`, etc.) — wrap in try/catch, rethrow as `UserFriendlyException(ex.Message)`, matching `NodeAppService.cs:139` and the other 23 examples.

**Not introducing a global exception filter in Phase 1.** Converting to a global `IExceptionFilter` that catches `DomainRuleException` automatically would be a strictly better long-term fix (removes the "easy to forget" risk) and is worth raising as a separate tech-debt / cleanup proposal against the *existing* codebase — but changing global exception handling behavior for the whole application is a bigger blast radius than Phase 1's three modules justify, and is exactly the kind of change that should go through its own review rather than ride in as a side effect of a feature-module design. Recommended as a follow-up ticket, not Phase 1 scope.

## Does the Technical Design change?
**Yes — a small, confirmatory edit.** Section 8.2 currently reads as an open question ("verify this mapping exists... worth a Sprint 1 spike"). This is now resolved and should read as a confirmed, mandatory pattern with the exact convention shown above, not a question. Section 4.4 and the Section 10.5 Definition of Done should each get a one-line addition: *"Every new AppService method calling a `DomainRuleException`-capable domain method wraps it in try/catch → `UserFriendlyException(ex.Message)`, per the existing `NodeAppService.cs:139` convention."*

## Implementation tasks resulting
- [ ] Patch Phase 1 Technical Design Sections 4.4, 8.2, and the Section 10.5 DoD checklist with the confirmed convention (done as part of this Sprint 0 report — see redline note in Sprint0-Report.md).
- [ ] No code changes required in Sprint 0 itself — this is a "confirm and adopt," not a "fix."
- [ ] Optional/separate backlog item (not Phase 1): propose a global `IExceptionFilter` for `DomainRuleException` to remove the per-call-site repetition risk across the whole codebase, including the 24 existing call sites.
