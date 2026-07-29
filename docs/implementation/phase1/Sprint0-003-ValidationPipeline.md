# Sprint 0-003 — DTO Validation Pipeline

## What was assumed
The Technical Design (Section 4.4) assumed shape validation happens via standard `System.ComponentModel.DataAnnotations` attributes on input DTOs (`[Required]`, `[Range]`, `[StringLength]`), "consistent with the existing `Facility.Name`'s pattern," and flagged confirming this against an existing input DTO as a Sprint 0/Sprint 1 check rather than assuming it.

## What was found in the code
This assumption was **wrong, and worth catching before Phase 1 wrote a dozen new DTOs against it.** The `[Required][StringLength(100, MinimumLength = 2)]` attributes cited in the Technical Design are on the **domain entity** (`Facility.Name`, enforced via `Facility.SetName`'s `DomainRuleException` guard, not via a DTO), not on any input DTO.

Inspected two representative existing input DTOs:
- `CreateNode` (`Services/Nodes/Dto/CreateNode.cs`) — plain properties (`SerialNumber`, `DisplayName`, `FirmwareVersion`, `Notes`, `Facility`, `TenantId`), **zero validation attributes**.
- `CreateNodeData` (`Services/NodeDatas/Dto/CreateNodeData.cs`) — plain nullable-string properties, **zero validation attributes**.

Where input validation exists at all today, it is done **manually, inline, at the top of the AppService method** — e.g. `NodeDataAppService.CreateAsync`:
```csharp
ArgumentNullException.ThrowIfNull(input);
if (string.IsNullOrWhiteSpace(input.SerialNumber)) throw new ArgumentNullException("Serial Number is required");
```
This is inconsistent even within itself: it throws `ArgumentNullException` for a *missing string value*, which is semantically wrong (that exception type is for null arguments, not for validation failures) and, per Sprint0-002's finding, does **not** go through the `UserFriendlyException` convention — meaning a caller providing a blank serial number today likely gets a masked, unhelpful error rather than a clean "Serial Number is required" message, unlike the `DomainRuleException` call sites which do reach the client cleanly.

**Conclusion: there is no established DTO validation pipeline to "follow."** Shape validation is done ad hoc, inconsistently, per method, when a developer happens to add a check.

## Decision
Phase 1 should **not** invent a new validation framework (no FluentValidation introduction, no `IValidatableObject` pattern) — that would be a bigger architectural change than a feature-module design should carry, and duplicates effort better spent as a deliberate, separately-reviewed cleanup.

Instead, Phase 1 DTOs adopt a **narrow, explicit convention**, consistent with the *spirit* (not the letter, since the letter doesn't really exist) of the current codebase:

1. **Standard `DataAnnotations` attributes on every new input DTO property** (`[Required]`, `[StringLength]`, `[Range]`) — even though no existing DTO does this, it is the ASP.NET Core-native, zero-dependency option, and ABP's model-binding pipeline already honors these automatically (they don't require any new middleware to take effect — this is a strictly additive improvement over the current ad hoc pattern, not a parallel system).
2. Where a check can't be expressed as a data annotation (e.g., "expected harvest date after planting date" — a cross-field check), do the manual check at the top of the AppService method **and route it through `UserFriendlyException` directly** (not `ArgumentNullException`, not a raw uncaught exception) — e.g. `if (input.ExpectedHarvestDate <= input.PlantingDate) throw new UserFriendlyException("Expected harvest date must be after planting date.");` This is a small, deliberate correction to the pattern found in `NodeDataAppService.CreateAsync`, not a reuse of its `ArgumentNullException` misuse.
3. True domain invariants stay in the domain entity's factory/behavior methods via `DomainRuleException`, per the existing (correct) `Facility`/`Node`/`CropSeason` pattern — unchanged from the Technical Design.

## Does the Technical Design change?
**Yes — Section 4.4 is corrected, not just confirmed.** The claim that DTO validation is "consistent with the existing `Facility.Name`'s pattern" is removed (that pattern is domain-level, not DTO-level, and no DTO-level pattern currently exists). Section 4.4 now states plainly that Phase 1 establishes the DTO-level validation convention described above, since none exists yet to inherit.

## Implementation tasks resulting
- [ ] Patch Phase 1 Technical Design Section 4.4 per the above (done as part of this Sprint 0 report).
- [ ] Apply `DataAnnotations` attributes to every new Phase 1 input DTO (`CreateFieldDto`, `PlantCropSeasonInput`, `CreateWeatherAlertRuleInput`, `RecordFertilizerApplicationInput`, etc.) as they're built in Sprints 1–5 — not retroactive to existing DTOs, which is out of Phase 1 scope, though worth flagging to the team as a quiet, low-risk cleanup opportunity (`CreateNode`/`CreateNodeData` could pick up the same attributes with no behavior change to existing valid calls, only better errors on invalid ones).
- [ ] Do **not** touch `NodeDataAppService.CreateAsync`'s existing `ArgumentNullException` misuse as part of Phase 1 — noted here so the inconsistency is documented and intentional to leave alone for now, not missed.
