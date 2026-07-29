# Sprint 0-005 — Device Authentication Hardening

## What was assumed
Both the Product Plan (Section 25) and Technical Design (Section 1.1, 9.4) treated telemetry-ingestion authentication as the single highest-priority, release-blocking security item for Phase 1, based on an earlier audit finding that `NodeDataAppService` had `[AbpAuthorize()]` commented out.

## What was found in the code
Confirmed directly, with full context this time:

```csharp
// Farmru.IotMonitoring.Application/Services/NodeDatas/NodeDataAppService.cs, line 28
//[AbpAuthorize()]
public class NodeDataAppService : AsyncCrudAppService<...>, INodeDataAppService
```

The entire class — including `CreateAsync`, the telemetry-ingestion entry point every field device calls — has **no authorization attribute active**. `CreateAsync`'s only identity check is that the posted `SerialNumber` must match an existing `Node` row (`_nodeRepository.FirstOrDefaultAsync(x => x.SerialNumber == input.SerialNumber)`); there is no secret, token, or credential tied to that serial number. **Any caller who knows or guesses a valid serial number can post arbitrary telemetry for that device**, with no rate limiting and no per-device credential.

Further, confirmed there is **no existing device-credential concept anywhere in the domain model**: `Node` (`Domains/Nodes/Node.cs`) has `SerialNumber`, `DisplayName`, `DeviceStatus`, `HealthStatus`, `FirmwareVersion` — no `ApiKey`, `Secret`, or credential field of any kind. And confirmed there is **no secrets-management infrastructure** in the solution — `appsettings.json` holds only a plaintext `ConnectionStrings` section; no Key Vault, no secret-store integration found anywhere in `Web.Host`.

This means hardening this endpoint is not a one-line fix (re-enabling `[AbpAuthorize()]` alone would break real device ingestion, since devices don't currently authenticate as ABP users/tenants at all — the endpoint is deliberately open today because there's no other identity mechanism for a device to use). A real fix requires: (a) a device-credential concept, (b) a validation path in `CreateAsync` that doesn't depend on ABP's user/session auth, and (c) somewhere safe to issue/store that credential.

## Decision
**Audited and designed in this Sprint 0 pass; implementation deliberately not applied to the repository in this pass.** This is a security-sensitive change to a live ingestion endpoint that real field devices call today — shipping it requires coordinating a credential-issuance step for existing registered Nodes (they'd all need a generated key before enforcement goes live, or ingestion breaks for every device in the field simultaneously). That coordination is a rollout decision, not something to make unilaterally mid-investigation. Recommended design, ready for Sprint 1:

1. Add `Node.ApiKey` (a generated, opaque secret, e.g. a GUID or random 32-byte token) — generated in `Node.Register(...)`'s factory method, following the exact same "generate at creation" convention already used for the aggregate's other invariants, and exposed to the owning tenant once at creation/rotation time (never returned in list/detail DTOs thereafter, to avoid leaking it into standard API responses).
2. Add a `RotateApiKey()` domain method (mirrors `ReplaceSerialNumber`'s shape) so a compromised device credential can be rotated without recreating the `Node`.
3. `NodeDataAppService.CreateAsync` validates a device-supplied key (header, e.g. `X-Device-Key`, rather than a body field, so it doesn't get logged/exported alongside telemetry payloads) against `Node.ApiKey` for the resolved `SerialNumber`, throwing `UserFriendlyException("Invalid device credentials.")` on mismatch — **not** re-enabling `[AbpAuthorize()]`, since that attribute gates ABP user/tenant session auth, which devices don't have; this needs its own lightweight check, not the user-auth pipeline.
4. Add basic rate limiting on this endpoint (ASP.NET Core's built-in rate-limiting middleware, scoped to this route) as a second layer, independent of the key check — protects against a single compromised or malfunctioning device flooding ingestion.
5. **Rollout requires a migration path for already-registered Nodes**: generate and backfill an `ApiKey` for every existing `Node` row as part of the same migration that adds the column, and — this is the part that needs a business/ops decision, not just code — a plan for getting that key onto each physical device already deployed in the field (firmware update, config push, or manual reissue, depending on how devices are currently provisioned, which wasn't in scope for this code-level audit).

## Does the Technical Design change?
No structural change — Technical Design Section 1.1 and 9.4 already scoped this as Phase 1, release-blocking. This record adds the concrete design (steps 1–5 above) that Section 1.1 left as "hardening TBD," and makes explicit that **the field-device rollout coordination is the actual scheduling risk**, not the code change itself, which is small.

## Implementation tasks resulting
- [ ] Confirm with whoever owns device provisioning/firmware how a rotated/issued key would actually reach a physical Node already in the field — this answer determines whether hardening can ship as a clean cutover or needs a transition window (e.g., accept both old-unauthenticated and new-keyed requests for a defined period).
- [ ] Once the rollout path is confirmed: implement steps 1–4 above in Sprint 1 (domain model + migration) and Sprint 2 (AppService enforcement), per Technical Design Section 10.1's existing sprint placement for this item.
- [ ] Do not flip the enforcement on in production until the field-device credential rollout (step 5) is actually complete — ship the capability and the migration ahead of turning on rejection of unkeyed requests, to avoid a hard cutover that silently stops all telemetry ingestion.
