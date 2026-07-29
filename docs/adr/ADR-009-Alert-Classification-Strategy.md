# ADR-009: Alert Classification Strategy

**Status:** Accepted
**Date:** 2026-07-29
**Related:** Farmru Product Plan v1.0 Section 21 (ADR log); Phase 1 Technical Design Sections 2.1, 3.6, 5.3, 9.1; Sprint 1 Checkpoint 2 (WeatherAlertRule implementation)

## Context

The Phase 1 Technical Design (Section 3.6, migration #2) assumed the existing `Alert` entity has an `AlertSource` enum that could be extended with a `Weather` value to distinguish weather-driven alerts from telemetry-driven ones. Implementation of Sprint 1 Checkpoint 2 (`WeatherAlertRule` + its integration with `Alert`) found this assumption does not match the codebase: `Alert` (`Farmru.IotMonitoring.Core/Domains/Alerts/Alert.cs`) has only `AlertType` (`DeviceOffline`, `LowBattery`, `SoilMoistureLow`, `TemperatureHigh`, `TemperatureLow`, `TelemetryAnomaly`, `GeoFenceBreach`, `SensorFailure`) and `AlertSeverity`. No `AlertSource` concept — as a field, an enum, or a lookup table — exists anywhere in the solution (confirmed by a full-solution grep).

`AlertType` already does double duty: most of its values describe a sensor condition (`SoilMoistureLow`, `TemperatureHigh`), but a few are effectively source/category markers rather than conditions (`GeoFenceBreach`, `SensorFailure`). This means the existing codebase has never drawn a hard line between "what happened" and "where the alert came from" — `AlertType` has stood in for both since the entity was first written.

This decision matters beyond Weather: Product Plan Phase 2 modules (Disease Risk Intelligence 5.4, Pest Monitoring 5.5, Predictive Maintenance 5.9) all state they "reuse the existing Alert entity" the same way Weather does. Whatever is decided here becomes the pattern those three modules inherit, whether decided deliberately or by accident.

## Decision

**For Phase 1: `AlertType` remains the sole alert classification mechanism. `AlertSource` is not introduced.**

Weather-driven alerts are represented as additional `AlertType` enum values, one per `WeatherAlertRule.WeatherAlertType`:
- `WeatherFrost`
- `WeatherHeatStress`
- `WeatherHighWind`
- `WeatherLightning`
- `WeatherSevereRain`

No new field, enum, or table is added to `Alert`. No existing `Alert` consumer (AppServices, DTOs, Angular alert list/detail, Flutter `AlertService`, SignalR payloads) needs to change to accommodate this — they already handle `AlertType` as an open-ended enum.

## Rationale

- **Matches the existing implementation.** `AlertType` already conflates condition and source-like categories (`GeoFenceBreach`); adding weather categories in the same style is consistent with, not a deviation from, current practice.
- **Avoids unnecessary aggregate expansion for a distinction nothing currently uses.** No screen, query, report, or downstream module today needs to filter or group by "source" independently of "type." Introducing the field now would be speculative.
- **Minimizes migration and cross-cutting impact.** Option B (a real `AlertSource` field) would require schema migration, entity changes, DTO/mapper changes, and touching Angular and Flutter alert-handling code for a distinction with no current consumer — full YAGNI territory given Sprint 1's "reuse existing platform unless there is a compelling reason not to" principle, which has held for every other Sprint 0/1 decision so far (background jobs, exception handling, AppService/DTO conventions).
- **Cheap to reverse with real evidence.** If Phase 2's Disease Risk, Pest Monitoring, and Predictive Maintenance modules each also bolt new categories onto `AlertType` and the resulting enum becomes unwieldy or consumer code starts branching on groups of values (see Alternatives Considered), that is concrete, usage-driven evidence for introducing `AlertSource` later — a decision made from three or four real data points instead of one hypothetical one.

## Alternatives Considered

**Option B — introduce a first-class `AlertSource` enum/field on `Alert`**, separating "source" from "type" as the Technical Design originally assumed. Rejected for Phase 1: architecturally cleaner in the abstract, but this is not a greenfield entity — `Alert` has 24+ existing call sites across `AlertAppService` and others. Paying that migration cost now, for a distinction nothing currently queries, fails the same YAGNI test the platform has applied consistently through Sprint 0/1 (e.g., not introducing FluentValidation, not introducing a global exception filter, not introducing a new scheduling framework). Recorded here as the option to revisit, not dismissed outright — see Future Consideration.

**Do nothing / decide silently at the code level without recording it.** Rejected — this is exactly the kind of decision ADR-008 and the Sprint 0 process exist to make visible rather than leave for the next developer to reverse-engineer from `git blame`, especially since three more Phase 2 modules will independently face the identical choice.

## Consequences

- **Positive:** Weather ships in Phase 1 without any change to `Alert`'s shape or any of its 24+ existing consumers — lowest-risk path.
- **Positive:** the decision is explicit and citable, so Disease Risk, Pest Monitoring, and Predictive Maintenance (Phase 2) don't each independently re-litigate the same question.
- **Accepted cost:** `AlertType` will keep growing (5 new values from Weather alone; more expected from Phase 2 modules) and continues to mix condition-level and source-level meaning. This is a deliberately deferred, not avoided, cost.
- **Neutral:** no changes needed to Angular/Flutter alert UI, `AlertAppService`, or any DTO — `AlertType` is already treated as an open, extensible enum by all existing consumers.

## Future Consideration

**Re-evaluate after Disease Risk Intelligence, Pest Monitoring, and Predictive Maintenance (Phase 2) have added their own `AlertType` values.** If, at that point, consumer code is visibly grouping/branching on clusters of `AlertType` values that really mean "this alert came from module X" (the pattern this ADR's Context section anticipates), that is the trigger to open a follow-up ADR introducing `AlertSource` — backed by real usage across four modules instead of a single hypothetical one from Weather alone.

## Implementation tasks resulting

- [ ] Extend `Farmru.IotMonitoring.Core/Domains/Alerts/AlertEnums.cs`'s `AlertType` enum with the five weather values listed in Decision.
- [ ] Amend Phase 1 Technical Design Sections 2.1, 3.6, 5.3, and 9.1 to replace every `Alert.AlertSource = Weather` reference with the confirmed mechanism (`AlertType.WeatherFrost` / `.WeatherHeatStress` / `.WeatherHighWind` / `.WeatherLightning` / `.WeatherSevereRain`) — tracked as a design correction driven by this ADR, not a scope change.
- [ ] Reference this ADR from Product Plan Section 21 (ADR log) as ADR-009.
