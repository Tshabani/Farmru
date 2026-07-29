# Sprint 0 Report — Phase 1 Validation Sprint

**Scope:** verify the five uncertainties the Phase 1 Technical Design flagged as blocking or risky, before Sprint 1 development starts. This was a code-investigation and decision-recording exercise, not a development sprint — no new product/UI features were built. One planned item (device auth) was deliberately audited and designed but **not applied to the repository**, since enforcing it touches a live field-device-facing endpoint and needs a rollout decision outside engineering's authority to make alone.

## Findings summary

| # | Item | Outcome | Technical Design impact |
|---|---|---|---|
| 001 | Background job infrastructure | **Resolved.** Existing pattern found: `OperationalMonitoringHostedService` — a plain .NET `BackgroundService`, no Hangfire/Quartz. Decision: extend with sibling hosted services for Weather/Nutrient jobs, same shape. | Section 5.2 updated from "two options, unresolved" to a confirmed decision. |
| 002 | `DomainRuleException` → HTTP mapping | **Resolved, assumption partly wrong.** No global exception filter exists — but a consistent **manual per-call-site convention** (`catch (DomainRuleException ex) { throw new UserFriendlyException(ex.Message); }`) is already used 24 times across the codebase and works correctly. Not missing, just manual. | Section 8.2 corrected from "open question" to "confirmed mandatory pattern." Section 4.4/10.5 get a one-line addition. |
| 003 | DTO validation pipeline | **Resolved, assumption wrong.** No DTO-level validation convention exists at all today (checked `CreateNode`, `CreateNodeData` — zero attributes; existing manual checks are inconsistent, e.g. `NodeDataAppService.CreateAsync` misuses `ArgumentNullException` for a validation failure). Phase 1 must **establish** the convention, not inherit one. | Section 4.4 corrected — DataAnnotations + `UserFriendlyException` for cross-field checks, explicitly a new convention. |
| 004 | Weather provider (ADR-001) | **Not resolved — correctly a vendor/commercial decision, not a code finding.** Confirmed genuinely greenfield (no existing integration, no config placeholder). Narrowed to 6 concrete evaluation criteria, weighted toward Southern African coverage and lightning data given the platform's actual hosting/customer geography. | No Technical Design change — `IWeatherProvider` contract is already provider-agnostic by design. |
| 005 | Device authentication hardening | **Audited and designed, not yet implemented.** Confirmed `NodeDataAppService` has zero authorization on telemetry ingestion and no device-credential concept exists in the domain model at all. Full design produced (Node.ApiKey, header-based check, rate limiting, migration + backfill). Implementation held pending confirmation of how a credential reaches already-deployed field devices — a provisioning/ops question, not a code question. | No structural change — this was already scoped as Phase 1 release-blocking; this record adds the concrete design. |

## What changed in the Technical Design as a result
Two corrections applied (both are *narrowing of uncertainty into a confirmed fact*, not scope changes):
- **Section 5.2** (Background Jobs): the "confirm in Sprint 1" language is replaced with the confirmed `BackgroundService`-pattern decision from 001.
- **Section 4.4 / 8.2 / 9.4 / 10.5** (Validation & Exception handling): corrected to state the *actual* existing conventions (manual try/catch→`UserFriendlyException` for domain exceptions; DataAnnotations-to-be-established for DTO shape validation) rather than the assumed-but-unverified conventions the original design guessed at.

Everything else in the Technical Design stands as written — Sprint 0 did not surface anything that invalidates the domain model, database design, AppService contracts, Angular/Flutter design, or sprint plan in Section 10.

## What is still open before Sprint 1 can fully proceed
1. **Weather provider selection** (004) — owner outside engineering, targeted to close within days, blocks Sprint 2 only (not Sprint 1).
2. **Device credential field-rollout plan** (005) — needs input from whoever owns device provisioning/firmware; blocks turning on enforcement, not the code/migration work itself, which can proceed in Sprint 1.

## Recommendation
Proceed to Sprint 1 as planned. Both open items are scoped narrowly enough that they block only their originally-planned later sprint (Sprint 2 for weather provider; the *enforcement* — not the schema/code — of device auth), not Sprint 1's domain-model-and-migration foundation work. No re-planning of Section 10.1's sprint sequence is needed.
