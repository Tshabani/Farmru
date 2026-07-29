# ADR-008: Validation Responsibility

**Status:** Accepted
**Date:** 2026-07-29
**Supersedes:** nothing (first ADR on this topic — no prior validation-responsibility decision was ever recorded, per Sprint 0-003's finding that no consistent convention existed in the codebase)
**Related:** Farmru Product Plan v1.0 Section 21 (ADR log); Phase 1 Technical Design Sections 4.4, 8.2; `/docs/implementation/phase1/Sprint0-002-ExceptionPipeline.md`, `Sprint0-003-ValidationPipeline.md`

## Context

Phase 1 Sprint 0 investigated how input validation and domain-invariant enforcement actually work in the existing codebase, expecting to find and reuse an established pipeline. It found two different things:

1. **Exception translation at the AppService boundary is a real, consistent, if uncentralized, convention.** `DomainRuleException` (thrown from domain entity factory/behavior methods, e.g. `Facility.SetName`, `Node.ReplaceSerialNumber`, `CropSeason.Plant`) is caught and rethrown as `Abp.UI.UserFriendlyException` at 24 call sites across `NodeAppService`, `AlertAppService`, `IncidentAppService`, `FacilityAppService`, `FacilityAppointmentAppService`, `PersonAppService`, `MonitoringAppService`, `OrganisationAppService`, and `GeoSpatialAppService`. This works today and should be treated as the platform's real, if implicit, standard.
2. **DTO-level shape validation has no convention at all.** Existing input DTOs (`CreateNode`, `CreateNodeData`) carry zero `DataAnnotations` attributes. Where a check exists, it's ad hoc and inconsistent — e.g. `NodeDataAppService.CreateAsync` throws `ArgumentNullException` for a missing serial number, the wrong exception type, which (per finding 1) also does not translate to a clean client-facing error the way `UserFriendlyException` does.

Left undocumented, this split (one layer consistently disciplined, the other layer undisciplined) would keep being independently rediscovered — or worse, inconsistently reinvented — by every future module, exactly the failure mode ADRs exist to prevent. This ADR exists because the finding is platform-wide, not Phase 1-specific: every module built after Phase 1 (Disease Risk, Pest Monitoring, Equipment, Inventory, Marketplace, Workflow Designer, ...) will hit the same four questions Phase 1 just had to answer from scratch.

## Decision

Farmru adopts a four-layer validation responsibility model, effective for all new development from Phase 1 onward:

| Layer | Responsibility | Mechanism | Failure surfaces as |
|---|---|---|---|
| **1. DTO (request shape)** | Required fields, string length, ranges, format — anything checkable from a single field in isolation | `System.ComponentModel.DataAnnotations` attributes on the input DTO (`[Required]`, `[StringLength]`, `[Range]`) | ASP.NET Core model-binding validation error (400), handled automatically, no custom code needed |
| **2. AppService (cross-field / cross-DTO-property rules)** | Checks that need more than one field on the same input to evaluate (e.g., "expected harvest date must be after planting date") | Explicit `if` check at the top of the AppService method, thrown directly as `UserFriendlyException(message)` | Clean 400-class error via ABP's standard `UserFriendlyException` handling |
| **3. Domain entity (invariants)** | Rules that protect the aggregate's own consistency, regardless of caller (e.g., "a decommissioned Node cannot have its serial number replaced") | `DomainRuleException` thrown from the entity's factory/behavior method (private setters, no way to bypass) | **Must be caught at the AppService call site** — see Decision Rule below |
| **4. Repository / cross-aggregate rules** | Rules that require looking at sibling aggregates the entity being modified can't see on its own (e.g., "a Node's serial number must be unique across all Nodes," "a Field cannot have two open CropSeasons at once") | Explicit repository query in the AppService, performed *before* calling the domain factory/behavior method — following the existing `INodeSerialNumberAvailabilityChecker` domain-service pattern | Same as Layer 2 — `UserFriendlyException` thrown directly by the AppService after the query determines the rule is violated |

**Decision Rule — the mandatory boundary translation:** every AppService method that calls a domain method capable of throwing `DomainRuleException` (Layer 3) **must** wrap that call and translate:
```csharp
try
{
    // call a domain factory/behavior method
}
catch (DomainRuleException ex)
{
    throw new UserFriendlyException(ex.Message);
}
```
This is not optional per-module style — it is the platform convention, evidenced by 24 existing call sites, and is now formally the required pattern rather than an implicit habit. A new AppService method that omits this wrap is a defect: the caller gets a masked, unhelpful 500 instead of the domain's actual, already-well-written error message.

## Alternatives Considered

- **Introduce FluentValidation or a similar validation framework.** Rejected for now: no existing code uses it, and introducing a second validation paradigm alongside `DataAnnotations` (which ABP already wires up natively) adds a dependency and a learning-curve cost without solving a problem `DataAnnotations` doesn't already solve for Layers 1–2. Revisit only if Layer 2's manual-`if`-checks pattern becomes genuinely unwieldy at higher complexity (e.g., a validation rule needing to compose across many optional fields) — not a Phase 1 problem.
- **Introduce a global `IExceptionFilter` for `DomainRuleException`.** Would remove the "24 call sites, easy to forget on the 25th" repetition risk identified in Sprint 0-002. Rejected for Phase 1 specifically because it changes global exception-handling behavior for the *entire* existing application, not just new modules — a change with a much larger blast radius than a feature phase should carry as a side effect. **Recorded here as a legitimate follow-up candidate**, to be proposed and reviewed on its own terms, potentially as ADR-009, not bundled into Phase 1.
- **Do nothing / leave each module to reinvent its own convention.** Rejected — this is the status quo Sprint 0 found (DTO validation genuinely had no convention), and is exactly the outcome an ADR exists to close off.

## Consequences

- **Positive:** every module built from Phase 1 onward has an unambiguous answer to "where does this validation go" — no more per-module rediscovery.
- **Positive:** the existing `UserFriendlyException` translation convention, previously implicit and undocumented, is now a citable, enforceable rule — code review can point at this ADR rather than at "well, that's how `NodeAppService` happens to do it."
- **Negative / accepted cost:** Layers 2–4 remain manual (no compiler or static-analysis enforcement that the try/catch wrap is present on every domain-calling method). This is an accepted, bounded risk for Phase 1; ADR-009 (global exception filter, if pursued) would close this gap platform-wide.
- **Neutral:** existing DTOs (`CreateNode`, `CreateNodeData`, and others not yet audited) are **not** retroactively updated by this ADR — it governs new development. Backfilling `DataAnnotations` onto existing DTOs is a safe, low-risk cleanup opportunity (adding a `[Required]` to a field that every valid existing caller already supplies changes nothing for correct callers, only improves the error for incorrect ones) but is explicitly out of scope here, to avoid this ADR quietly becoming a refactor mandate.

## Implementation tasks resulting

- [ ] Reference this ADR from Product Plan Section 21 (ADR log) as ADR-008.
- [ ] Reference this ADR from Phase 1 Technical Design Sections 4.4 and 8.2 in place of the Sprint 0 record citations (the ADR is now the durable source; the Sprint 0 records remain as the investigation trail that produced it).
- [ ] Apply the four-layer model to every Phase 1 AppService method as it's built (Sprints 1–5).
- [ ] Consider ADR-009 (global `DomainRuleException` filter) as a separate, later proposal — not scheduled, not blocking.
