# Sprint 0-004 — Weather Provider ADR (ADR-001)

## What was assumed
Product Plan Section 21 (ADR-001) and Technical Design Section 5.1 both deferred the concrete weather provider choice, defining only the `IWeatherProvider` contract and requiring ADR-001 to close before `WeatherSyncHostedService`'s concrete implementation is written.

## What was found in the code
No existing weather/external-API integration exists anywhere in the solution to anchor a decision against (no `IWeatherProvider`-shaped interface, no HTTP client wrapper for a third-party API, no provider API key placeholder in `appsettings.json` — confirmed by direct inspection: `appsettings.json` contains only a `ConnectionStrings` section, no `ApiKey`/`Secret`/provider-config sections of any kind). This is a genuinely greenfield integration, not something to reconcile against prior art.

Two facts from the earlier product-level audit are relevant and are restated here because they directly bound this decision:
- The backend is hosted at `farmruapi.technobrainent.co.za` — a South African domain — implying the primary customer base and Facility geolocations are likely concentrated in Southern Africa, which should weight provider selection toward strong regional forecast/history coverage and, ideally, lightning-detection coverage for that region (Product Plan 5.1 explicitly wants lightning alerts, which not every weather API tier includes).
- Provider cost scales with Facility count (Product Plan Section 19 risk, restated in Technical Design 5.5) — this needs real per-Facility/per-call pricing from candidate vendors, not an assumption, before Sprint 2 (Weather backend) starts.

## Decision
**Not closed by this Sprint 0 pass — this is a vendor/commercial decision, not a code-investigation finding, and shouldn't be made unilaterally without pricing and contract terms in hand.** What Sprint 0 *can* responsibly do is narrow the decision to a concrete, time-boxed choice for whoever owns it (product/commercial, not engineering alone):

**Evaluation criteria, in priority order given the findings above:**
1. Regional (Southern Africa) forecast accuracy and history depth
2. Lightning-strike data included (not all tiers of all providers include this — verify explicitly, don't assume)
3. Frost/heat-stress-relevant hourly granularity (not just daily min/max)
4. Historical data API (needed for `WeatherObservation` backfill and later Yield Prediction training data, Product Plan 5.20)
5. Pricing model that scales sanely with Facility count (per-call vs. per-location subscription — matters a lot once the Multi-Farm Command Center's hundreds-of-Facilities target, Product Plan Section 26, is considered)
6. Evapotranspiration (Et0) data or formula support, directly needed by Product Plan 5.1

**Recommendation on process, not on vendor:** get quotes/trial access for 2–3 candidate providers against these six criteria before Sprint 2 starts (per the Technical Design's Section 10.3 dependency — Sprint 2 is blocked on this). This should take days, not weeks — it does not need to hold up Sprints 0–1, which don't depend on it (domain model and migrations, Section 2/3 of the Technical Design, are provider-agnostic by design, since `IWeatherObservation`'s fields were deliberately modeled as normalized values, not provider-specific raw payloads).

## Does the Technical Design change?
No. The `IWeatherProvider` contract (Section 5.1) is intentionally provider-agnostic and needs no change regardless of which vendor is chosen — that was the point of designing it as an interface. Once selected, only the concrete implementation class (not designed in this document) is affected.

## Implementation tasks resulting
- [ ] **Owner decision needed, outside engineering:** select and contract a weather provider against the six criteria above.
- [ ] Once selected: implement the concrete `IWeatherProvider` implementation (e.g. `<VendorName>WeatherProvider`) in Sprint 2, per Technical Design Section 5.1.
- [ ] Store the provider API key via whichever secrets mechanism Sprint0-005's audit establishes (see that record) — do **not** add it as a plaintext `appsettings.json` entry, which is the current pattern for the DB connection string and should not be extended to a new secret without the Sprint0-005 decision first.
