---
title: Farmru — Digital Agriculture Platform Product Plan
---

# Farmru Digital Agriculture Platform
## Product Planning & UX Modernization Document

*Prepared as a planning artifact only — no production code. All proposals build on the existing ABP + Angular + Flutter architecture. Livestock functionality is explicitly out of scope.*

---

## 1. Executive Summary

Farmru today is a working **agri-IoT operations platform**: multi-tenant, built on ASP.NET Boilerplate, with an Angular web app and a Flutter mobile app, real-time telemetry via SignalR, and a functioning domain model covering Organisations → Facilities → Nodes → Telemetry, plus Alerts, Incidents, GeoFences, Tasks, Personnel, and a monitoring/GIS layer. This is a solid operational core — most competitors start from nothing.

What Farmru lacks, relative to category leaders (Climate FieldView, John Deere Operations Center, Trimble Ag, ArcGIS), is the **agronomic and business layer on top of the operational layer**: weather intelligence, crop lifecycle management, nutrient/fertilizer planning, disease and pest risk, satellite/drone imagery, an AI agronomist, predictive maintenance, equipment/inventory, marketplace, financials, sustainability/compliance, and a true multi-farm command center — plus a UI/UX bar that matches modern enterprise SaaS.

This document is a **planning exercise, not a build spec**. It sequences 22 new capability areas plus a platform-wide UX modernization pass into five phases, defines the domain model extensions and API surface needed for each, and lays out personas, journeys, permissions, and success metrics — all without touching or replacing the existing ABP domain, which becomes the foundation the new modules attach to via new aggregates and cross-references (`FacilityId`, `NodeId`, `OrganisationId`) rather than a rewrite.

**Core thesis:** Farmru's differentiator is that it already owns the *ground-truth sensor layer* (soil telemetry) that platforms like FieldView largely lack, and already owns the *ops workflow* (Incidents/Tasks/SignalR) that GIS-only tools like ArcGIS lack. The highest-leverage roadmap is to fuse that sensor+ops foundation with agronomic intelligence (weather, crop, nutrient, disease, AI) before spending on hardware-adjacent bets (drones, marketplace) that are farther from the current moat.

---

## 2. Product Vision

> **Farmru is the operations system of record for commercial farming — the place where soil truth, weather, crop lifecycle, equipment, people, and money meet, so that every action a farm takes is explainable, timely, and provably worth its cost.**

Three pillars carry the vision:

1. **Sense** — IoT telemetry, satellite/drone imagery, weather, disease/pest risk: the platform knows more about field conditions than any single person can.
2. **Decide** — an AI Agronomist and rules engine turn raw signals into plain-language, confidence-scored recommendations and automated actions, closing the gap between "data" and "decision."
3. **Act & Prove** — Tasks, Incidents, Equipment, Inventory, Financials and Compliance close the loop: recommendations become tracked work, and outcomes become ROI and ESG evidence.

Farmru should feel, in daily use, less like a sensor dashboard and more like **mission control for a farm business** — an executive opens it for portfolio-level KPIs, an agronomist opens it for field-level reasoning, and a technician opens it as a field checklist with a map and a camera.

---

## 3. Feature Roadmap (Prioritized by Business Value)

Prioritization weighs: (a) leverage of existing Node/Facility/Alert/Incident data already flowing, (b) revenue/retention impact, (c) build complexity/dependency on third parties, (d) regulatory or trust-building value.

| Tier | Modules | Rationale |
|---|---|---|
| **P0 — Foundational multipliers** | Weather Intelligence, Crop Management, Fertilizer & Nutrient Management, Farm Activity Timeline | These make every existing telemetry point *meaningful* (a moisture reading means nothing without crop stage + rain forecast). Low third-party risk (weather APIs are commoditized), high leverage of existing Facility/Node model. |
| **P1 — Intelligence layer** | AI Agronomist, Disease Risk Intelligence, Pest Monitoring, Predictive Maintenance, Automation Rules Engine | Converts P0 data into recommendations and autonomous actions — the core differentiation vs. dashboards-only competitors. |
| **P1.5 — Process layer** | Workflow Designer (5.23) | Sequences the single-action automations from P1 into stateful, multi-role business processes — ships immediately after the Rules Engine so the two share a design pass rather than diverging. |
| **P2 — Imagery & spatial** | Satellite Imagery (NDVI/NDRE), Multi-Farm Command Center, Farm Digital Twin, Benchmarking | High visual/demo impact, moderate integration complexity (third-party imagery providers), builds on existing GIS module. |
| **P3 — Operations & assets** | Equipment Management, Inventory Management, Technician Mobile Experience upgrade, Yield Prediction | Extends the existing Incident/Task workflow to physical assets; yield prediction needs a season of P0/P2 data to be credible. |
| **P4 — Business & ecosystem** | Financial Analytics, Marketplace, Carbon & Sustainability, Compliance, Drone Integration | Highest business value long-term but depend on P0–P3 data maturity (financials need cost data others feed; marketplace needs supplier network; drones are capital-intensive for customers). |

Sequencing across five delivery phases is detailed in **Section 18**.

---

## 4. Information Architecture

Current Angular IA (flat top-level modules: Node, Alerts, Monitoring, GIS, Incidents, Facility, Organisation, Person) does not scale to 22 additional modules. Proposed IA reorganizes around **workflow, not entity type**, with a persistent left rail of 6 top-level zones instead of ~10 flat items:

```
Farmru
├── 1. Home / Command Center        (role-based landing: Executive | Agronomist | Technician | Ops)
├── 2. Field Operations
│   ├── Facilities & Fields
│   ├── Crops (seasons, planting, growth, harvest)
│   ├── Nodes & Telemetry
│   ├── Weather
│   ├── GIS / Digital Twin
│   └── Satellite & Drone Imagery
├── 3. Agronomy Intelligence
│   ├── AI Agronomist (recommendations feed)
│   ├── Disease Risk
│   ├── Pest Monitoring
│   ├── Nutrient & Fertilizer
│   └── Yield Prediction
├── 4. Operations & Response
│   ├── Alerts
│   ├── Incidents
│   ├── Tasks
│   ├── Automation Rules
│   └── Equipment & Maintenance
├── 5. Supply & Finance
│   ├── Inventory
│   ├── Marketplace
│   └── Financial Analytics
├── 6. Enterprise
│   ├── Organisations & Facilities admin
│   ├── Personnel & Roles
│   ├── Compliance & Certifications
│   ├── Carbon & Sustainability
│   ├── Benchmarking
│   └── Reports
└── Settings / Admin (existing ABP tenant/user/role screens, unchanged)
```

Each zone is lazy-loaded (consistent with the current Angular module pattern) and permission-gated per zone and per sub-item, extending the existing `AppRouteGuard` + `Pages.*` permission convention already used for Nodes/Alerts/Monitoring/Gis/Incidents/Facilities.

---

## 5. Module-by-Module Functional Specifications

Each module below follows a consistent template: **Purpose · Key User Stories · Core Workflow · Primary Screens · Domain Model Additions · API Surface (new AppServices) · Permissions · Integration Points**. Domain model additions are named to slot next to the existing `Facility`, `Node`, `NodeData`, `Alert`, `Incident`, `Organisation`, `Person` entities in `Farmru.IotMonitoring.Core`.

### 5.1 Weather Intelligence

**Purpose:** Give every telemetry reading and every recommendation a weather context; provide proactive weather-driven alerts distinct from soil-sensor alerts.

**User Stories:**
- As a farm manager, I see a 7-day forecast per Facility on the dashboard, so I can plan irrigation and spraying.
- As an agronomist, I get a frost/heat-stress/lightning alert before it happens, not after.
- As an executive, I see a weather overlay on the multi-farm map so I can spot regional risk (e.g., an approaching storm front affecting 12 facilities).
- As a technician, I see current conditions before dispatch, so I don't drive into a lightning warning.

**Core Workflow:** Scheduled background job polls a weather provider per Facility (by lat/long, already stored on `Facility`) → normalizes into `WeatherObservation`/`WeatherForecast` → evaluates against `WeatherAlertRule` thresholds → raises `Alert` (reusing the existing Alert entity/type enum, extended with `WeatherAlertType`) → pushes via the existing `AlertNotificationHub` SignalR channel.

**Primary Screens:**
- Weather widget on Facility detail and Command Center (current conditions + 7-day forecast, gauge-style consistent with existing agricultural-dashboard gauges).
- Weather History screen (time series, exportable, mirrors existing NodeData Excel export pattern).
- Weather Alerts configuration screen (thresholds per Facility/region, mirrors existing `AlertThresholdConfiguration` UX).
- Map weather overlay (radar/precipitation layer toggle on the existing Leaflet GIS map).

**Domain Model Additions:**
- `WeatherObservation` (FacilityId, timestamp, temp, humidity, windSpeed/direction, precipitation, pressure, UV index)
- `WeatherForecastDaily` / `WeatherForecastHourly` (FacilityId, validFrom, precipitationProbability, tempMin/Max, windGust, frostRisk, heatStressIndex)
- `EvapotranspirationReading` (FacilityId, date, ET0/ETc, cropCoefficient reference)
- `WeatherAlertRule` (FacilityId or OrganisationId scope, alertType enum: Frost/Wind/Heat/Lightning/RainSevere, threshold, severity)
- Extend `Alert.AlertType` with weather-specific values (`WeatherFrost`, `WeatherHeatStress`, `WeatherHighWind`, `WeatherLightning`, `WeatherSevereRain`) — corrected per ADR-009 (`Alert` has no separate `AlertSource` concept; see `/docs/adr/ADR-009-Alert-Classification-Strategy.md`).

**API Surface:** `WeatherAppService` (GetCurrent, GetForecast, GetHistory, GetEvapotranspiration), `WeatherAlertRuleAppService` (CRUD).

**Permissions:** `Pages.Weather`, `Pages.Weather.Configure` (thresholds), read access inherited from Facility-level tenant permission.

**Integration Points:** Third-party weather API (e.g., a commercial forecast/history provider with lightning + agri-specific indices) as an external HTTP integration behind an `IWeatherProvider` interface — consistent with ABP's dependency-injection pattern, swappable provider.

---

### 5.2 Crop Management

**Purpose:** Attach an actual crop lifecycle to Facilities/Fields so telemetry and recommendations become stage-aware ("moisture is low" means something different at germination vs. maturity).

**User Stories:**
- As a farm manager, I define Fields within a Facility, and plant a Crop Season per Field.
- As an agronomist, I track growth stage transitions and compare expected vs. actual yield.
- As a planner, I see a Planting Calendar across all Facilities to sequence labor/equipment.
- As an executive, I see crop rotation history per Field to catch compliance/soil-health risk.

**Core Workflow:** Facility is subdivided into `Field` (new sub-entity, geofenced polygon reusing existing `GeoFence` capability) → a `CropSeason` is opened against a Field with `CropType`, `SeedVariety`, planting date, expected yield → `GrowthStageEvent`s are logged (manually or AI-suggested from NDVI later) → `HarvestRecord` closes the season with actual yield → `CropRotationHistory` is derived automatically from closed seasons per Field.

**Primary Screens:**
- Field management (create/edit polygon on map, nested under Facility detail).
- Crop Season wizard (crop type → variety → seed supplier → planting date → expected yield → plant population).
- Growth Stage timeline (visual stage tracker, e.g. BBCH-style stage stepper).
- Harvest entry screen (actual yield, quality grade, notes).
- Planting Calendar (Gantt/calendar view across Facilities, filterable by crop type/region).
- Crop Rotation history view per Field (last N seasons, warns on repeat-crop disease risk).

**Domain Model Additions:**
- `Field` (FacilityId, name, polygon/geoFenceId, areaHectares, soilType)
- `CropType`, `SeedVariety` (SeedSupplierId), `SeedSupplier`
- `CropSeason` (FieldId, CropTypeId, SeedVarietyId, plantingDate, expectedHarvestDate, expectedYield, plantPopulation, status)
- `GrowthStageEvent` (CropSeasonId, stage, observedDate, source: Manual/AI)
- `HarvestRecord` (CropSeasonId, harvestDate, actualYield, qualityGrade)
- `CropRotationHistory` (materialized/query view over closed CropSeasons per Field)

**API Surface:** `FieldAppService`, `CropSeasonAppService`, `CropTypeAppService`, `SeedVarietyAppService`, `HarvestAppService`, `GrowthStageAppService`.

**Permissions:** `Pages.Crops`, `Pages.Crops.Manage`, `Pages.Crops.Harvest`.

**Integration Points:** Feeds Yield Prediction (5.20), Disease Risk (5.4, stage is an input), Fertilizer Scheduling (5.3), NDVI overlays (5.6).

---

### 5.3 Fertilizer & Nutrient Management

**Purpose:** Turn the existing NPK sensor readings from a display-only gauge into an actionable nutrient management loop.

**User Stories:**
- As an agronomist, I see nutrient balance (applied vs. sensed vs. crop-stage requirement) per Field.
- As a manager, I track fertilizer inventory consumption and cost per application.
- As the system, I recommend an application (product, rate, timing) when a deficiency is detected, and let the user schedule it.

**Core Workflow:** `NodeData` NPK readings + `CropSeason` stage requirement table → deficiency/surplus computed → `NutrientRecommendation` generated (feeds AI Agronomist, 5.8) → user schedules a `FertilizerApplication` → deducts from `FertilizerInventoryItem` stock (shared model with Inventory Management, 5.11) → `NutrientBalanceHistory` updated for trend charting.

**Primary Screens:**
- Nutrient Balance dashboard per Field (applied vs. required vs. sensed, stacked bar/line).
- Fertilizer Application log (date, product, rate, area, cost, operator).
- Fertilizer Scheduling calendar (recommended + planned applications).
- Deficiency alert card feeding into the AI Agronomist feed.

**Domain Model Additions:**
- `FertilizerProduct` (name, NPK composition, unitCost, supplierId)
- `FertilizerApplication` (FieldId/CropSeasonId, productId, rateKgHa, applicationDate, cost, operatorPersonId)
- `NutrientRecommendation` (FieldId, nutrient, deficiencyLevel, recommendedProduct, recommendedRate, confidence)
- `NutrientBalanceHistory` (rolling computed snapshot per Field)

**API Surface:** `FertilizerAppService`, `NutrientRecommendationAppService`, extends `NodeDataAppService` aggregation queries already used for the agricultural-dashboard.

**Permissions:** `Pages.Nutrients`, `Pages.Nutrients.Apply`, `Pages.Nutrients.Recommend`.

**Integration Points:** Inventory Management (stock deduction), AI Agronomist (recommendation authoring), Financial Analytics (cost per hectare).

---

### 5.4 Disease Risk Intelligence

**Purpose:** Predict disease pressure before symptoms are visible, using existing telemetry (temperature/humidity/soil moisture) plus weather and crop stage — no new hardware required.

**User Stories:**
- As an agronomist, I see a disease probability score per Field/crop, updated daily.
- As a technician, I get an "inspect Field X for early blight" task auto-created when risk crosses threshold.
- As a manager, I see severity trending over the season and correlate with any treatment applied.

**Core Workflow:** Nightly job evaluates known disease models (e.g., leaf-wetness-duration-based blight models, humidity/temp-based rust models) per crop type using `NodeData` + `WeatherObservation` + `CropSeason.stage` → produces `DiseaseRiskAssessment` (probability, severity band) → if above threshold, creates an `Alert` and optionally a `Task`/`Incident` for inspection → `TreatmentRecord` closes the loop.

**Primary Screens:**
- Disease Risk dashboard: heat-map grid of Fields × disease models × risk level.
- Field-level risk detail (probability trend line, contributing factors explained in plain language — ties to AI Agronomist explainability pattern).
- Inspection & Treatment log.

**Domain Model Additions:**
- `DiseaseModel` (cropTypeId, name, algorithm reference/parameters)
- `DiseaseRiskAssessment` (FieldId, diseaseModelId, date, probability, severity)
- `TreatmentRecord` (FieldId, diseaseRiskAssessmentId nullable, product, date, cost, effectivenessNote)

**API Surface:** `DiseaseRiskAppService` (GetAssessments, GetHistory), `TreatmentAppService`.

**Permissions:** `Pages.DiseaseRisk`, `Pages.DiseaseRisk.Treat`.

**Integration Points:** Weather Intelligence, Crop Management (stage), AI Agronomist (explanation layer), Incidents (auto-created inspection tickets reuse existing Incident workflow).

---

### 5.5 Pest Monitoring

**Purpose:** Track physical pest traps and outbreaks spatially, complementing sensor-based disease risk with field-observed pest data.

**User Stories:**
- As a technician, I register a pest trap at a GPS location and log inspection counts.
- As an agronomist, I see a pest hotspot map across a Facility/region.
- As a manager, I track spray history and its correlation with outbreak decline.

**Core Workflow:** `PestTrap` registered at a location (reuses Facility/Field geolocation pattern) → periodic `TrapInspection` logs pest counts by species → counts above threshold create `PestOutbreak` record + `Alert` → `SprayRecord` logged against outbreak → hotspot map aggregates outbreaks spatially (reuses existing Leaflet clustering already used for Nodes).

**Primary Screens:**
- Trap registry & map (pins on existing GIS map, new layer).
- Trap Inspection entry (mobile-first — ties into Technician Mobile Experience, 5.17).
- Pest Outbreak list + detail.
- Hotspot heat map (Facility and multi-farm level).
- Spray history log.

**Domain Model Additions:**
- `PestTrap` (FacilityId/FieldId, location, trapType, species targeted)
- `TrapInspection` (PestTrapId, date, count, inspectorPersonId)
- `PestOutbreak` (FieldId, species, severity, detectedDate, status)
- `SprayRecord` (FieldId/OutbreakId, product, date, cost, applicatorPersonId, complianceDocRef)

**API Surface:** `PestTrapAppService`, `PestOutbreakAppService`, `SprayRecordAppService`.

**Permissions:** `Pages.Pests`, `Pages.Pests.Inspect`, `Pages.Pests.Spray`.

**Integration Points:** GIS map layer, Compliance module (spray records feed chemical-application compliance, 5.15), AI Agronomist.

---

### 5.6 Satellite Imagery Integration

**Purpose:** Add a remote-sensing layer (NDVI/NDRE/biomass/soil moisture maps) over Facilities/Fields, giving spatial context that point-sensor Nodes cannot.

**User Stories:**
- As an agronomist, I overlay NDVI on the Field map to spot stress zones between Node locations.
- As a manager, I see biomass trend over the season per Field.
- As the system, I suggest a new Node placement where imagery shows a persistent stress zone but no Node exists.

**Core Workflow:** Scheduled/on-demand job requests latest satellite pass for a Field's polygon from a provider → stores `SatelliteImageLayer` (tile/COG reference + index type + capture date) → computed zonal stats (`SatelliteZoneStat`) per Field → surfaced as map overlay and trend chart; optionally correlated with Node telemetry for calibration.

**Primary Screens:**
- Imagery layer toggle on existing GIS map (NDVI/NDRE/Biomass/Soil Moisture Map, date slider).
- Field imagery history (thumbnail timeline + trend chart).
- "Node placement suggestion" panel showing stress zones without sensor coverage.

**Domain Model Additions:**
- `SatelliteImageLayer` (FieldId, captureDate, indexType, tileUrl/rasterRef, providerRef)
- `SatelliteZoneStat` (SatelliteImageLayerId, zoneId, meanIndexValue, minMax)

**API Surface:** `SatelliteImageryAppService` (GetLatest, GetHistory, RequestCapture).

**Permissions:** `Pages.Imagery`.

**Integration Points:** Third-party satellite imagery provider (e.g., a Sentinel-Hub-class API) behind `ISatelliteImageProvider`; GIS map (existing Leaflet); Crop Management (per-Field, per-season overlays); AI Agronomist (imagery as a recommendation input).

---

### 5.7 Drone Integration

**Purpose:** Support higher-resolution, on-demand imagery and inspection where satellite revisit/resolution is insufficient, and let drone findings feed the same Incident workflow as sensors.

**User Stories:**
- As an operations manager, I plan a drone mission over a Field and track its status.
- As an agronomist, I view the resulting orthomosaic/thermal imagery overlaid on the Field.
- As the system, I auto-create an Incident when a drone pass detects an anomaly (e.g., irrigation leak visible thermally).

**Core Workflow:** `DroneMission` planned (Field, flight pattern, scheduled date, pilot/operator) → mission executed externally (drone hardware/flight-planning software is out of scope; Farmru is the record-of-truth and imagery consumer) → `DroneImageryAsset` (orthomosaic, thermal, RGB) uploaded/linked → anomaly detection (manual tagging in MVP, automated later) creates `Incident` referencing the imagery asset.

**Primary Screens:**
- Mission planning & status list.
- Mission detail with map flight path.
- Imagery viewer (ortho/thermal layers on Field map, same viewer pattern as satellite).
- "Create Incident from imagery" action with image annotation.

**Domain Model Additions:**
- `DroneMission` (FieldId, scheduledDate, status, pilotPersonId, flightPatternRef)
- `DroneImageryAsset` (DroneMissionId, assetType: Ortho/Thermal/RGB, storageRef, capturedDate)
- Extend `Incident` with optional `SourceImageryAssetId`.

**API Surface:** `DroneMissionAppService`, `DroneImageryAppService`.

**Permissions:** `Pages.Drones`, `Pages.Drones.PlanMission`.

**Integration Points:** File/blob storage for large imagery assets (new infra concern — see Section 15), Incident module, shares imagery viewer component with Satellite module (5.6).

---

### 5.8 AI Agronomist

**Purpose:** The connective-tissue module — turns every other module's raw output into a single, explainable, confidence-scored recommendation feed, in the "pH = 5.1 → your soil acidity is increasing, consider lime" style.

**User Stories:**
- As a farm manager, I open one feed and see prioritized, plain-language recommendations across nutrients, disease, pests, irrigation, and maintenance.
- As an agronomist, I can see *why* a recommendation was made (contributing factors, confidence score) and accept/dismiss/override it, with my feedback improving future recommendations.
- As an executive, I see a rolled-up "recommendations acted on vs. ignored" metric as a proxy for platform trust.

**Core Workflow:** `RecommendationEngine` (a service layer, not necessarily ML at MVP — starts as a rules/expert-system layer over Nutrient/Disease/Pest/Weather/Maintenance modules, evolves toward a learned model as history accumulates) consumes structured inputs from every domain module → emits `Recommendation` records (category, plain-language text, reasoning trace, confidence score, suggested action) → user feedback (`RecommendationFeedback`: accepted/dismissed/overridden) is captured and used to retrain/recalibrate confidence over time (explicit "historical learning" loop, phased — rules engine first, statistical calibration second, ML model third).

**Primary Screens:**
- Recommendation feed (card list, prioritized, filterable by Facility/category/confidence) — the natural new Home/Command-Center centerpiece.
- Recommendation detail (reasoning trace: "based on: pH trend −0.3 over 14 days, forecast rain 60% in 3 days, crop stage = flowering").
- Feedback affordance (accept → creates Task; dismiss → reason capture; override → free text).
- Agronomist "trust" analytics screen for admins (acceptance rate over time, by category).

**Domain Model Additions:**
- `Recommendation` (scopeType: Field/Facility/Node, scopeId, category, text, reasoningJson, confidenceScore, generatedDate, status)
- `RecommendationFeedback` (RecommendationId, action, personId, note, timestamp)
- `RecommendationRule` (versioned rule definitions per category, admin-editable — bridges to Automation Rules Engine, 5.19)

**API Surface:** `RecommendationAppService` (GetFeed, GetDetail, SubmitFeedback), `RecommendationRuleAppService` (admin CRUD).

**Permissions:** `Pages.Recommendations`, `Pages.Recommendations.ConfigureRules`.

**Integration Points:** Every intelligence module (5.1, 5.3, 5.4, 5.5, 5.9, 5.20) is both a producer of inputs and a consumer of the shared recommendation surface; Automation Rules Engine (5.19) can auto-execute high-confidence recommendations; Tasks module for "accept → task."

**Explainability note (architecture-level, not implementation):** every `Recommendation` must store its reasoning as structured data (contributing signals + weights/thresholds), not just prose, so the UI can render a factor breakdown and so recommendations remain auditable — important for both user trust and, later, compliance/insurance use cases.

---

### 5.9 Predictive Maintenance

**Purpose:** Apply the same "sense → decide → act" pattern to the Nodes themselves, not just crops — predicting device failure before an outage causes a data gap.

**User Stories:**
- As an ops manager, I see a Device Health Score per Node, combining battery trend, signal quality, and sensor-drift indicators.
- As the system, I flag a Node whose battery is degrading faster than its solar charging compensates, and predict a replacement window.
- As a technician, I get a maintenance task auto-created before the device actually fails.

**Core Workflow:** Existing `NodeData` (battery/solar voltage, signal) + `Node` health/status fields are trended → `DeviceHealthScore` computed periodically (battery degradation slope, solar charging efficiency, signal stability, sensor-drift vs. expected calibration curve) → threshold breach creates a `MaintenancePrediction` (predicted failure window) → auto-creates a `Task`/`Incident` for proactive replacement, reusing `NodeReplacementHistory` already in the domain.

**Primary Screens:**
- Fleet Health dashboard (Node list sortable by health score, red/amber/green).
- Node Health detail (battery/solar/signal trend charts, drift chart vs. calibration baseline).
- Maintenance prediction calendar (predicted replacement windows across the fleet).

**Domain Model Additions:**
- `DeviceHealthScore` (NodeId, date, batteryScore, solarScore, signalScore, driftScore, overallScore)
- `MaintenancePrediction` (NodeId, predictedIssueType, predictedWindowStart/End, confidence)
- (Reuses existing `NodeReplacementHistory`.)

**API Surface:** `DeviceHealthAppService`, `MaintenancePredictionAppService`.

**Permissions:** `Pages.DeviceHealth`.

**Integration Points:** Feeds Automation Rules Engine (auto-create replacement task), Equipment Management (shared maintenance-task UX pattern), Monitoring module (extends existing `MonitoringExecutionHistory` job types).

---

### 5.10 Equipment Management

**Purpose:** Extend asset tracking beyond IoT Nodes to physical farm equipment, using the same maintenance-workflow muscle memory built for Nodes.

**User Stories:**
- As a manager, I register tractors, pumps, irrigation equipment, and implements per Facility.
- As a technician, I log service history, fuel, and attachments against a piece of equipment.
- As a manager, I get a warranty-expiry and service-due alert.

**Core Workflow:** `Equipment` registered per Facility/Organisation → `ServiceRecord`/`FuelLog`/`WarrantyRecord` logged against it → due-date rules (service interval, warranty expiry) create `Alert`/`Task` via the same rule pattern as 5.9.

**Primary Screens:**
- Equipment registry (list/detail, categorized: Tractors/Pumps/Irrigation/Vehicles/Implements).
- Service history log + attachment upload (manuals, invoices, photos).
- Fuel log.
- Warranty tracker with expiry alerts.
- Equipment utilization view (feeds Financial Analytics 5.13).

**Domain Model Additions:**
- `Equipment` (FacilityId, category, make/model/serial, purchaseDate, status)
- `ServiceRecord` (EquipmentId, date, description, cost, performedByPersonId)
- `FuelLog` (EquipmentId, date, litres, cost, odometer/hours)
- `WarrantyRecord` (EquipmentId, provider, expiryDate, terms)
- `EquipmentAttachment` (EquipmentId, fileRef, type)

**API Surface:** `EquipmentAppService`, `ServiceRecordAppService`, `FuelLogAppService`.

**Permissions:** `Pages.Equipment`, `Pages.Equipment.Service`.

**Integration Points:** Financial Analytics (equipment cost/utilization), Automation Rules (service-due alerts), Inventory (parts consumption during service).

---

### 5.11 Inventory Management

**Purpose:** Track consumable and spare-part stock across warehouses so Fertilizer, Pest, and Equipment modules have a real stock backend instead of free-text logging.

**User Stories:**
- As a manager, I see current stock levels of seeds/fertilizers/chemicals/fuel/spare parts per warehouse.
- As the system, I raise a reorder alert when stock crosses a minimum threshold.
- As a manager, I transfer stock between warehouses/facilities and see a full movement audit trail.

**Core Workflow:** `Warehouse` per Facility/Organisation holds `StockItem` balances → `StockMovement` records every in/out (application, transfer, purchase receipt, adjustment) → `ReorderRule` (min/max per item per warehouse) triggers `Alert` when breached.

**Primary Screens:**
- Warehouse/stock overview (by category: Seeds/Fertilizers/Chemicals/Fuel/Spare Parts).
- Stock item detail (movement history, current balance, reorder threshold).
- Transfer wizard (warehouse A → B).
- Reorder alert list (links to Marketplace, 5.12, for one-click RFQ).

**Domain Model Additions:**
- `Warehouse` (FacilityId/OrganisationId, name, location)
- `StockItem` (WarehouseId, productRef [Fertilizer/Seed/Chemical/Fuel/Part], quantity, unit)
- `StockMovement` (StockItemId, type: Receipt/Application/Transfer/Adjustment, quantity, date, refEntity)
- `ReorderRule` (StockItemId, minThreshold, maxThreshold)

**API Surface:** `WarehouseAppService`, `StockItemAppService`, `StockMovementAppService`.

**Permissions:** `Pages.Inventory`, `Pages.Inventory.Transfer`.

**Integration Points:** Fertilizer Application (5.3) and Spray Records (5.5) deduct stock; Equipment service parts consumption (5.10); Marketplace (5.12) for reordering; Financial Analytics (stock valuation).

---

### 5.12 Marketplace

**Purpose:** Close the loop from "we need more fertilizer" to "we ordered it," connecting inventory shortfalls to real supplier transactions.

**User Stories:**
- As a manager, I request quotes (RFQ) from multiple suppliers for a stock item.
- As a manager, I compare quotes and place an order.
- As the system, I keep purchase history linked to the inventory item for cost trending.

**Core Workflow:** `Supplier` and `SupplierCatalogueItem` registered (own-tenant curated list initially, not an open marketplace at MVP) → `RFQ` raised (optionally auto-suggested from a Reorder Alert) → suppliers respond with `Quote` → user compares and converts to `PurchaseOrder` → receipt updates Inventory `StockMovement`.

**Primary Screens:**
- Supplier directory & catalogue browse.
- RFQ creation & tracking.
- Quote comparison table.
- Purchase order list/detail + history.

**Domain Model Additions:**
- `Supplier`, `SupplierCatalogueItem` (product, price, leadTime)
- `RFQ` (StockItemId/productRef, quantity, requestedDate, status)
- `Quote` (RFQId, SupplierId, price, leadTime, validUntil)
- `PurchaseOrder` (RFQId/QuoteId, status, orderedDate, receivedDate)

**API Surface:** `SupplierAppService`, `RFQAppService`, `QuoteAppService`, `PurchaseOrderAppService`.

**Permissions:** `Pages.Marketplace`, `Pages.Marketplace.Order` (approval-level permission, financial exposure).

**Integration Points:** Inventory (reorder trigger, stock receipt), Financial Analytics (purchase cost data). **Note:** MVP should be a *closed* supplier network per tenant/region (curated), not an open two-sided marketplace — that is a separate business/legal undertaking (payments, supplier onboarding, trust & safety) outside this platform-engineering scope; flagged in Risks (Section 19).

---

### 5.13 Financial Analytics

**Purpose:** Convert operational data already being captured (fertilizer cost, equipment fuel/service cost, harvest yield, water/irrigation) into farm-business financial KPIs.

**User Stories:**
- As an executive, I see cost-per-hectare and profitability per Field/Facility/Season.
- As a manager, I see ROI on a fertilizer program or irrigation investment.
- As a finance user, I export financial reports per Organisation/Facility.

**Core Workflow:** A `CostEntry`/`RevenueEntry` ledger (fed automatically from Fertilizer Applications, Spray Records, Fuel Logs, Service Records, Labor if tracked, and manually for revenue/sale price) is aggregated per CropSeason/Field/Facility → computed KPIs (cost/ha, profit/ha, ROI, water cost, fertilizer cost, equipment utilization $/hr) are surfaced in dashboards and exports.

**Primary Screens:**
- Financial dashboard (cost/ha, profit/ha, ROI by Field/Facility/Season, trend and comparison charts).
- Cost breakdown drill-down (by category: fertilizer/water/fuel/equipment/labor).
- Yield-profitability scatter (yield vs. cost vs. price).
- Export/reporting screen (PDF/Excel, reusing the existing ClosedXML export pattern from NodeData).

**Domain Model Additions:**
- `CostEntry` (scopeType/scopeId, category, amount, date, sourceRefEntity)
- `RevenueEntry` (CropSeasonId, amount, date, buyer/notes)
- Computed/materialized `FinancialSummary` per Field/Facility/Season for dashboard performance.

**API Surface:** `FinancialAnalyticsAppService` (GetCostBreakdown, GetProfitability, GetROI, Export).

**Permissions:** `Pages.Financials` (typically restricted to Manager/Executive roles — see Section 16).

**Integration Points:** Pulls from Fertilizer, Equipment, Inventory, Crop Management (yield), Marketplace (purchase cost); feeds Benchmarking (5.22) and Carbon/Sustainability (5.14, cost-per-emission-avoided type metrics).

---

### 5.14 Carbon & Sustainability

**Purpose:** Provide ESG-grade reporting from operational data already captured (fertilizer type/quantity, fuel use, water use), positioning Farmru for buyer/regulator/certifier requirements increasingly attached to commercial agriculture contracts.

**User Stories:**
- As an executive, I see a carbon footprint estimate per Facility/Season, with the biggest contributing factors.
- As a compliance officer, I generate an ESG/regulatory report for a certification body or buyer.
- As a manager, I see a nitrogen-use-efficiency score and water footprint trend.

**Core Workflow:** Emission-factor calculations applied to existing operational data (fertilizer N applied × emission factor, fuel burned × emission factor, irrigation volume) → `SustainabilityMetric` computed per Field/Facility/Season → rolled into a `SustainabilityScore` and exportable `ESGReport`.

**Primary Screens:**
- Sustainability dashboard (carbon footprint, water footprint, N-efficiency, trend over seasons).
- Sustainability Score card per Facility (with contributing-factor breakdown — same explainability pattern as AI Agronomist).
- ESG/regulatory report generator (templated exports).

**Domain Model Additions:**
- `EmissionFactor` (reference table: fertilizer type, fuel type → CO2e factor)
- `SustainabilityMetric` (scopeType/scopeId, metricType: Carbon/Water/NitrogenEfficiency, value, period)
- `ESGReport` (generated report record, template, period, exportRef)

**API Surface:** `SustainabilityAppService` (GetMetrics, GetScore, GenerateReport).

**Permissions:** `Pages.Sustainability`, `Pages.Sustainability.Report`.

**Integration Points:** Fertilizer (5.3), Equipment/Fuel (5.10), Financial Analytics (cost of sustainability initiatives), Compliance (5.15, certification evidence overlap).

---

### 5.15 Compliance

**Purpose:** Provide an audit trail and certification-tracking layer so chemical applications, operator certifications, and food-safety/organic/GAP requirements are provable, not just implicit in operational logs.

**User Stories:**
- As a compliance officer, I see every chemical application logged with applicator certification validity at time of application.
- As an auditor, I export a full audit trail for a Facility/Season for a certification body.
- As a manager, I get alerted before an operator's certification expires.

**Core Workflow:** `Certification` tracked per Person (operator licenses, spray certifications) → cross-checked at the point of `SprayRecord`/`FertilizerApplication` creation (soft warning if applicator's certification is expired/missing) → `ComplianceRecord`/`AuditTrailEntry` aggregates chemical applications, certifications, and relevant events into exportable audit packages per standard (GAP, Organic, Food Safety).

**Primary Screens:**
- Certification registry per Person (expiry tracking, renewal alerts).
- Compliance dashboard per Facility (chemical application log, certification status, open compliance flags).
- Audit export wizard (select Facility/Season/standard → generate package).

**Domain Model Additions:**
- `Certification` (PersonId, type, issuer, issueDate, expiryDate)
- `ComplianceStandard` (e.g., GAP/Organic/FoodSafety reference definitions)
- `AuditTrailEntry` (aggregation/query layer over existing SprayRecord/FertilizerApplication/Certification — largely a reporting view, not new transactional data).

**API Surface:** `CertificationAppService`, `ComplianceAppService` (GetDashboard, GenerateAuditExport).

**Permissions:** `Pages.Compliance`, `Pages.Compliance.Audit`.

**Integration Points:** Pest/Spray Records (5.5), Fertilizer (5.3), Personnel (existing `Person` entity extended with Certifications), Sustainability (5.14) — many compliance frameworks share evidence with ESG reporting.

---

### 5.16 Multi-Farm Command Center

**Purpose:** Give an enterprise customer (or Farmru itself, as an operations partner) a portfolio-level view across hundreds of Facilities and thousands of Nodes — the executive/ops-center counterpart to the existing single-Facility monitoring dashboard.

**User Stories:**
- As a regional director, I see fleet health, active alerts, and active incidents rolled up across all Facilities I'm responsible for.
- As an operations lead, I see technician availability/location alongside active incidents to dispatch efficiently.
- As an executive, I see a regional summary map with drill-down to any Facility.

**Core Workflow:** Aggregation layer over existing Facility/Node/Alert/Incident/Monitoring data (largely a new reporting/dashboard AppService layer, not new transactional entities) — pre-aggregated summary tables refreshed on a schedule for performance at scale (hundreds of Facilities × thousands of Nodes).

**Primary Screens:**
- Command Center home (KPI tiles: total Facilities, Nodes online/offline %, active Alerts by severity, active Incidents by status, weather risk regions).
- Regional map (clustered Facility pins, colored by health/alert status, existing Leaflet clustering extended).
- Technician availability panel (map + list, feeds Dispatch, existing Incident dispatch screen extended to multi-facility scope).
- Drill-down breadcrumb into any single Facility's existing dashboard.
- Multi-monitor / kiosk layout mode (see Section 13.9).

**Domain Model Additions:**
- `FacilitySummarySnapshot` (materialized rollup: nodeCount, onlinePct, activeAlerts by severity, activeIncidents by status, refreshed periodically) — a performance/reporting construct, not new business data.
- `RegionalGrouping` (optional hierarchy: Organisation → Region → Facility, if not already implicitly modeled).

**API Surface:** `CommandCenterAppService` (GetPortfolioSummary, GetRegionalMap, GetTechnicianAvailability).

**Permissions:** `Pages.CommandCenter` (typically Regional Director/Executive/Ops Lead roles).

**Integration Points:** Everything — this is a read-layer over Facility, Node, Alert, Incident, Monitoring, Weather, Personnel. Primary architectural risk is query performance at scale (addressed in Section 15).

---

### 5.17 Technician Mobile Experience

**Purpose:** Upgrade the Flutter app from a solid CRUD/workflow client (per the current audit: login, alerts, incidents, tasks, offline-queue-partial) into a genuinely field-first tool — the current "Operational Map" screen is a list, not a map, and there's no offline map, navigation, QR/NFC, or camera/voice/signature capture.

**User Stories:**
- As a technician, I see an interactive map with my assigned incidents/tasks and can navigate to them, even with poor connectivity.
- As a technician, I scan a QR/NFC tag on a physical Node to pull up its detail instantly instead of searching a list.
- As a technician, I attach a photo, voice note, and digital signature to close out an Incident/Task, and it syncs when I'm back online.

**Core Workflow:** Replace the current placeholder map screen with a real map SDK (offline tile caching for low-connectivity rural use) → add route/navigation to a selected Facility/Node/Incident → add QR/NFC scan-to-open for Nodes/Equipment → extend Incident/Task closeout forms with photo, voice note, and signature capture, stored locally and synced via the existing (currently partial) offline-queue pattern already present in `AlertService`/`IncidentService`.

**Primary Screens:**
- Interactive map (replacing `operational_map_screen.dart`'s list view) with offline tile support and clustering.
- Turn-by-turn navigation handoff (to device's native maps app, or embedded routing).
- QR/NFC scan screen (camera-based scan → deep link to Node/Equipment detail).
- Task/Incident closeout form (photo capture, voice note recorder, signature pad).
- Sync status indicator (surfacing the existing but currently silent pending-action queue).

**Domain Model Additions:**
- `IncidentAttachment`/`TaskAttachment` (type: Photo/VoiceNote/Signature, fileRef, capturedOffline flag, syncedAt) — extends existing Incident "attachments/timeline" already noted in the current audit.
- `NodeQrTag`/`EquipmentQrTag` (entityId, tagCode) if QR codes are Farmru-issued rather than manufacturer serials.

**API Surface:** Extends existing `IncidentAppService`/`TaskAppService` with attachment upload endpoints; new `DeviceLookupAppService.GetByTag(tagCode)`.

**Permissions:** Inherits existing Technician role scoping; no new permission class needed, this is UX depth on existing permitted actions.

**Integration Points:** Maps SDK (new mobile dependency), device camera/microphone (Flutter platform plugins), existing offline-queue infrastructure (needs to be made connectivity-trigger-driven rather than manual-call-only, per the audit finding that `syncPendingActions()` isn't confirmed wired to connectivity events), Automation Rules Engine (closeout can trigger next-step automation).

---

### 5.18 Farm Digital Twin

**Purpose:** A unifying visualization concept — not a new data source, but a new *composition* of every layer (Facility → Fields → Sensors → Weather → Equipment → Crop → Infrastructure → Operations) into one navigable spatial/temporal model.

**User Stories:**
- As an agronomist, I open a Facility and see one interactive view where I can toggle layers (soil sensors, weather, imagery, equipment locations, active incidents) rather than navigating between separate modules.
- As a new user, the Digital Twin view is the fastest way to understand a Facility's current state at a glance.

**Core Workflow:** Purely a **composition/visualization layer** — it reads from Facility, Field, Node, WeatherObservation, Equipment, CropSeason, Incident, SatelliteImageLayer (all defined above) and renders them as togglable map layers plus a time slider (scrub through the season to see how each layer evolved). No new transactional writes originate here.

**Primary Screens:**
- Digital Twin view per Facility: base map (existing Leaflet) + layer toggle panel (Sensors / Weather / Equipment / Crop Boundaries / Imagery / Active Incidents) + time slider.
- Layer-specific popovers reusing each module's existing detail card components (Node detail card, Incident card, etc.) so this is compositional, not a rebuild.

**Domain Model Additions:** None beyond what's listed in prior sections — this is explicitly a UI composition, which should be called out to the engineering team as a **low-risk, high-visual-impact** module once the underlying data layers (5.1–5.11) exist.

**API Surface:** A `DigitalTwinAppService.GetLayers(facilityId, date)` orchestration endpoint that composes existing per-module AppServices server-side (or is composed client-side — an architecture decision for the build phase, not this planning doc).

**Permissions:** Inherits Facility-level view permission; layer visibility can be permission-gated per layer (e.g., Financial layer hidden from Technician role) if a "financial overlay" is ever added.

**Integration Points:** Everything spatial (5.1, 5.2, 5.6, 5.7, 5.9, 5.10); best scheduled **after** the source modules exist, not before.

---

### 5.19 Automation Rules Engine

**Purpose:** Let users define IF/AND/THEN automations across any module (not hard-coded alert logic), turning recommendations into autonomous or semi-autonomous action — the platform's most direct lever on reducing manual monitoring effort.

**User Stories:**
- As a manager, I build a rule: "IF soil moisture < 20% AND rain probability < 25% THEN [notify manager, create task, escalate after 2 hours]" without writing code.
- As an admin, I test/simulate a rule against historical data before activating it.
- As an auditor, I see a log of every rule execution and its resulting actions.

**Core Workflow:** `AutomationRule` defines a condition tree (`RuleCondition`, referencing any metric already exposed by another module's AppService — moisture, weather forecast, device health score, disease risk, etc.) and an action list (`RuleAction`: notify, create Task, create Incident, escalate, open irrigation *[if/when an irrigation-control integration exists — otherwise represented as a recommended manual action]*) → a rule evaluation job runs on a schedule/event trigger → `RuleExecutionLog` records every firing for audit → a **Rule Simulator** lets an admin dry-run a rule against historical NodeData/Weather without triggering real actions.

**Primary Screens:**
- Rule Builder (visual, drag/drop condition blocks: metric → operator → value, AND/OR grouping, action list, escalation timing) — the most novel UI component in this plan; should follow a proven "visual rule builder" interaction pattern (block-based, not a raw JSON/DSL editor) for non-technical agronomist users.
- Rule list (active/inactive, last fired, fire count).
- Rule Simulator (pick a historical date range → preview what would have fired).
- Rule Execution Log (audit view).

**Domain Model Additions:**
- `AutomationRule` (scope, name, conditionTreeJson, actions, escalationConfig, isActive)
- `RuleExecutionLog` (AutomationRuleId, firedAt, conditionsSnapshot, actionsTaken, outcome)

**API Surface:** `AutomationRuleAppService` (CRUD, Simulate, GetExecutionLog).

**Permissions:** `Pages.Automation`, `Pages.Automation.Activate` (activating a rule with real-world actions is a higher-trust permission than authoring one).

**Integration Points:** Every module with a threshold/metric becomes a rule condition source; every module with a create-action (Task, Incident, Alert) becomes a rule action target; directly powers "accept recommendation → auto-execute" from the AI Agronomist (5.8) for high-confidence, low-risk recommendations.

---

### 5.20 Yield Prediction

**Purpose:** Forecast expected yield per CropSeason using the accumulated telemetry, weather, imagery, and historical-yield data from other modules — explicitly sequenced *after* those modules exist and have at least one season of data.

**User Stories:**
- As a manager, I see a predicted yield range (with confidence) partway through the season, not just at harvest.
- As an agronomist, I see the top risk factors currently suppressing predicted yield (e.g., "nutrient deficiency in Field 4 is projected to reduce yield 8%").
- As an executive, I compare predicted vs. actual yield accuracy over time to build trust in the model.

**Core Workflow:** `YieldPredictionModel` (per crop type) ingests weather history/forecast, historical yield (from `HarvestRecord`), soil data (`NodeData`), fertilizer applications, irrigation, and NDVI trend (`SatelliteZoneStat`) → produces `YieldPrediction` (expected yield, confidence interval, contributing risk factors) per CropSeason, refreshed periodically through the season → compared against `HarvestRecord.actualYield` at season close to track model accuracy (`YieldPredictionAccuracy`).

**Primary Screens:**
- Yield Prediction card on CropSeason detail (predicted range, trend over the season, risk factor breakdown — same explainability pattern as AI Agronomist).
- Portfolio yield forecast view (aggregated across Facilities/regions, feeds Command Center and Financial Analytics revenue projections).
- Model accuracy tracking screen (admin/data-science facing).

**Domain Model Additions:**
- `YieldPredictionModel` (cropTypeId, version, modelType/parameters reference)
- `YieldPrediction` (CropSeasonId, predictedDate, expectedYield, confidenceLow/High, topRiskFactors)
- `YieldPredictionAccuracy` (CropSeasonId, predictedYield, actualYield, errorPct)

**API Surface:** `YieldPredictionAppService` (GetPrediction, GetPortfolioForecast, GetAccuracyHistory).

**Permissions:** `Pages.YieldPrediction`.

**Integration Points:** Depends on Crop Management (5.2), Weather (5.1), Nutrient Management (5.3), Satellite Imagery (5.6) all being populated with at least one season of real data — explicitly a **Phase 3–4** module, not earlier, or its predictions will be built on empty history and undermine platform trust.

---

### 5.21 Farm Activity Timeline

**Purpose:** A single chronological "story" of everything that happened on a Field/Facility — planting, fertilization, irrigation, weather events, alerts, incidents, disease/pest events, harvest, maintenance — currently scattered across separate module screens.

**User Stories:**
- As an agronomist, I scrub through a season's timeline for a Field to understand what led to a yield outcome.
- As a manager, I filter the timeline by event type (only fertilizer applications, or only incidents) for a report.
- As an auditor, I export a filtered timeline as a compliance/traceability record.

**Core Workflow:** Purely an **aggregation/read layer** (like the Digital Twin) over events already created by other modules — `GrowthStageEvent`, `FertilizerApplication`, irrigation events (if/when tracked), `WeatherObservation` significant events, `Alert`, `Incident`, `DiseaseRiskAssessment`/`TreatmentRecord`, `HarvestRecord`, `ServiceRecord`. A lightweight `TimelineEventIndex` (denormalized, populated by a domain-event handler each module already raises, or by a scheduled indexer) makes cross-module chronological querying performant without every screen doing N cross-service calls.

**Primary Screens:**
- Timeline view per Field/Facility (vertical scrolling timeline, icon-coded by event type, expandable detail per entry).
- Filter/report panel (event type, date range, export to PDF/Excel).

**Domain Model Additions:**
- `TimelineEventIndex` (scopeType/scopeId, eventType, eventDate, summaryText, sourceEntityRef) — denormalized index, not source-of-truth data.

**API Surface:** `TimelineAppService` (GetTimeline, Export).

**Permissions:** Inherits Field/Facility view permission; export may require an elevated permission (`Pages.Timeline.Export`) for compliance-sensitive contexts.

**Integration Points:** Every transactional module above is a producer; best implemented via ABP's existing event bus pattern (`IEventHandler` per domain event) so new modules automatically feed the timeline without tight coupling.

---

### 5.22 Benchmarking

**Purpose:** Let users compare performance across Farms, Regions, Seasons, and Crops — the analytical capstone that makes all the other modules' data comparably useful, not just locally useful.

**User Stories:**
- As an executive, I compare yield and profitability across Facilities in the same region/crop to find best practices worth replicating.
- As an agronomist, I compare this season's water/fertilizer efficiency against last season for the same Field.
- As a regional director, I rank Facilities by equipment utilization to prioritize investment.

**Core Workflow:** A cross-cutting analytics layer over `HarvestRecord`, `FinancialSummary`, `SustainabilityMetric`, `Equipment` utilization, and `NutrientBalanceHistory` — computing normalized comparison metrics (yield/ha, profit/ha, water efficiency, fertilizer efficiency) grouped by Facility/Region/Season/CropType, with percentile ranking.

**Primary Screens:**
- Benchmarking dashboard (selectable comparison axis: Farm vs Farm, Region vs Region, Season vs Season, Crop vs Crop; metric selector; ranked bar/table view).
- Facility scorecard (a single Facility's percentile rank across all benchmark metrics — good candidate for the Executive persona's landing widget).

**Domain Model Additions:** None new — this is a query/reporting layer over `FinancialSummary` (5.13), `SustainabilityMetric` (5.14), `HarvestRecord` (5.2), `Equipment` utilization (5.10). Consider a `BenchmarkSnapshot` materialized table for performance at multi-hundred-Facility scale (same pattern as `FacilitySummarySnapshot` in 5.16).

**API Surface:** `BenchmarkingAppService` (Compare, GetScorecard).

**Permissions:** `Pages.Benchmarking` (cross-tenant benchmarking, if ever offered as an anonymized industry-wide comparison, is a distinct and much higher-sensitivity feature — flagged in Risks, Section 19 — MVP scope is within-tenant, cross-Facility only).

**Integration Points:** Depends on Financial Analytics, Sustainability, Crop Management, Equipment all being populated — a **Phase 4–5** module.

---

### 5.23 Workflow Designer

**Purpose:** Complement the Automation Rules Engine (5.19, which is *reactive*: condition → action) with a *process* layer that lets organizations model multi-step, multi-role business processes end-to-end — e.g. Weather Alert → Create Incident → Assign Agronomist → Field Inspection → Approve Treatment → Create Purchase Request → Complete → Close Incident — without engineering changes per tenant. Rules fire single actions; Workflows sequence *stateful, multi-actor* processes with approvals and handoffs.

**User Stories:**
- As an operations lead, I define a multi-step process (trigger → sequence of steps, each with an owner role, required approvals, and SLA) that matches how my organization actually operates, without asking engineering for a custom workflow.
- As an agronomist, when a workflow step is assigned to me, I see exactly what's expected (approve/reject/complete) and the process advances automatically on my action.
- As a manager, I see any in-flight workflow instance's current step, history, and bottlenecks (e.g., "12 instances stuck at Approve Treatment for >48h").
- As an admin, I version a workflow definition — in-flight instances continue on the version they started with, new instances use the latest.

**Core Workflow:** A `WorkflowDefinition` (versioned, tenant-scoped) declares a trigger (any event already in the system — Alert raised, Incident created, Recommendation accepted, manual start) and an ordered/branching set of `WorkflowStepDefinition`s (owner role, action type: Task/Approval/Notification/Create-Entity, SLA, conditional branching on outcome). On trigger, a `WorkflowInstance` is created and steps advance as `WorkflowStepInstance`s are completed/approved/rejected, each capable of creating real domain records (an Incident, a Task, a Purchase Request via 5.12) as a step's action — i.e., Workflow steps *call* the same AppServices every other module already exposes, rather than owning new business logic themselves.

**Primary Screens:**
- Workflow Designer (visual canvas, node-and-connector step editor — same interaction family as the Automation Rule Builder in 5.19, reused rather than reinvented: drag steps, connect branches, assign owner roles, set SLAs).
- Workflow Definition library (list, versions, activate/deprecate).
- Workflow Instance monitor (Kanban-style, current step per instance, bottleneck/SLA-breach highlighting — visually consistent with the existing Incident kanban board already in the Angular app).
- My Tasks/Approvals inbox (a workflow-aware view layered on top of the existing Task module, so a step assigned to a user shows up where they already look for work).

**Domain Model Additions:**
- `WorkflowDefinition` (OrganisationId, name, version, triggerType, triggerConfig, isActive)
- `WorkflowStepDefinition` (WorkflowDefinitionId, order/branchRef, ownerRole, actionType, actionConfig, slaHours)
- `WorkflowInstance` (WorkflowDefinitionId+version, triggerSourceEntityRef, status, startedDate)
- `WorkflowStepInstance` (WorkflowInstanceId, WorkflowStepDefinitionId, status, assignedPersonId, completedDate, outcome, resultingEntityRef)

**API Surface:** `WorkflowDefinitionAppService` (CRUD, versioning, activate/deprecate), `WorkflowInstanceAppService` (Start, GetInstances, AdvanceStep, GetBottleneckReport).

**Permissions:** `Pages.Workflows.Design` (typically Ops Lead/Admin), `Pages.Workflows.Participate` (any role that can be assigned a step, scoped by their existing role permissions on the underlying action — e.g., a step that creates a Purchase Request still requires the actor to hold `Pages.Marketplace.Order`).

**Integration Points:** Shares its condition/trigger vocabulary and builder UX with the Automation Rules Engine (5.19) — the two should be designed together so a tenant isn't forced to choose the right tool from two incompatible systems. Every module with a create-action (Incident, Task, Purchase Request, Certification renewal, etc.) becomes a possible workflow step target. Feeds the Farm Activity Timeline (5.21) as a first-class event source. This module is why the **Bounded Context Map (Section 22)** and **Module Dependency Matrix (Section 29)** below matter — Workflow Designer is the one module that can legitimately touch almost every other bounded context, so its integration contract needs to be explicit rather than ad hoc.

**Sequencing note:** depends on the Automation Rules Engine's shared substrate existing first (Phase 2) and benefits from several action-producing modules (Incidents, Tasks, Marketplace) being in place — recommended for **Phase 3**, immediately after the Rules Engine, rather than alongside it, so the two aren't designed in a rush together.

---

## 6. User Personas

| Persona | Role | Primary Goals | Primary Surface |
|---|---|---|---|
| **Elena, Regional Director (Executive)** | Oversees 40–200 Facilities across a region for an agribusiness enterprise customer | Portfolio health at a glance, ROI justification, risk exposure, board-ready reporting | Multi-Farm Command Center, Financial Analytics, Benchmarking |
| **Thabo, Agronomist** | Advises on crop/nutrient/disease decisions across a cluster of Facilities | Fast diagnosis, trustworthy recommendations, explainability, season-over-season learning | AI Agronomist feed, Crop/Nutrient/Disease/Pest modules, Digital Twin |
| **Nomvula, Farm Manager** | Runs day-to-day operations of 1–5 Facilities | Keep everything on schedule, catch problems early, manage cost | Home Command Center (facility-scoped), Alerts, Incidents, Tasks, Weather, Financials |
| **Sipho, Field Technician** | Executes inspections, maintenance, incident response on the ground | Clear task list, easy navigation, fast data capture, works with poor signal | Flutter mobile app — map, tasks, incidents, offline capture |
| **Priya, Compliance/Sustainability Officer** | Ensures certifications, audit readiness, ESG reporting | Complete, exportable, defensible records | Compliance, Sustainability, Timeline export |
| **Farmru Ops/Support (internal)** | Monitors platform health, tenant onboarding, escalations | System-wide device/tenant health, not a single farm's data | Command Center (Farmru-internal super-tenant view), Monitoring |

---

## 7. User Journeys

**Journey A — "Something's wrong in Field 4" (Thabo, Agronomist → Sipho, Technician)**
1. AI Agronomist feed surfaces: *"Field 4 disease risk (early blight) rising — 68% probability, confidence high, driven by 3 days of leaf-wetness conditions + susceptible growth stage."*
2. Thabo reviews reasoning trace, accepts recommendation → creates an inspection Task, auto-assigned to nearest technician (Sipho) based on Facility/region.
3. Sipho gets a push notification on mobile, opens map, navigates to Field 4, scans the nearest Node's QR tag to confirm location context, takes photos, logs a Treatment Record.
4. Timeline for Field 4 now shows: risk detected → task created → inspection completed → treatment applied, in one chronological view.
5. Two weeks later, Disease Risk trend shows decline; Thabo marks the recommendation "effective" — feeding the AI Agronomist's feedback loop.

**Journey B — "Board meeting Monday" (Elena, Executive)**
1. Opens Command Center: portfolio KPI tiles show 187 Facilities, 94% Nodes online, 12 active high-severity alerts (3 weather, 9 device), 4 open incidents past SLA.
2. Drills into Benchmarking: this quarter's top-quartile Facilities by profit/ha, filtered by maize.
3. Opens Financial Analytics: regional cost-per-hectare trend, exports a PDF for the board deck.
4. Checks Sustainability dashboard: portfolio carbon footprint trending down 6% YoY, generates an ESG summary for a buyer requirement.

**Journey C — "Irrigation decision" (Nomvula, Farm Manager)**
1. Weather Intelligence shows 25% rain probability tomorrow; current soil moisture is 18%, below the crop-stage threshold.
2. An Automation Rule she built earlier fires: notifies her, creates a Task, and (if an irrigation-control integration exists) opens irrigation; if not, the Task is the actionable unit for a technician to open irrigation manually.
3. She reviews the Rule Execution Log next morning to confirm it fired correctly and irrigation happened.

---

## 8. Domain Model Extensions (Summary)

All new entities are additive and reference existing aggregates (`Organisation`, `Facility`, `Node`, `NodeData`, `Alert`, `Incident`, `Person`, `GeoFence`) by foreign key — no existing table is restructured. Grouped by module in Section 5; the following cross-cutting entities are shared infrastructure rather than belonging to a single module:

- **Attachment/File storage** — a generalized `Attachment` pattern (used by Incidents, Equipment, Drone imagery, Technician mobile captures) should be unified into one blob-storage-backed entity/service rather than reinvented per module.
- **Event/Timeline indexing** — `TimelineEventIndex` should be populated via ABP's existing domain event bus, with every new module raising an event on creation of a significant record, rather than the Timeline module polling every other module's tables directly.
- **Recommendation/Rule shared substrate** — `Recommendation` (5.8) and `AutomationRule` (5.19) intentionally share a condition/metric vocabulary so that a metric exposed for one is automatically available to the other.
- **Snapshot/materialization pattern** — `FacilitySummarySnapshot` (5.16) and `BenchmarkSnapshot` (5.22) both address the same scaling concern (hundreds of Facilities, thousands of Nodes) and should share a common scheduled-materialization infrastructure component.

---

## 9. API and Integration Considerations

**Internal (ABP AppService layer):** Each module above defines its own AppService(s), consistent with the existing pattern (`NodeAppService`, `AlertAppService`, etc.). New composition-layer services (`DigitalTwinAppService`, `CommandCenterAppService`, `TimelineAppService`) orchestrate reads across multiple existing AppServices rather than duplicating query logic.

**External integrations required:**
| Integration | Used by | Notes |
|---|---|---|
| Weather provider (forecast + history + lightning + agri indices) | 5.1, 5.4, 5.20 | Behind `IWeatherProvider` abstraction for swappability |
| Satellite imagery provider (NDVI/NDRE/biomass) | 5.6, 5.20 | Behind `ISatelliteImageProvider`; consider cost-per-hectare pricing impact at scale |
| Blob/object storage | 5.7, 5.17, 5.10 attachments | Needed regardless of provider choice; currently no evidence of this in the audited stack |
| SMS/Push notification provider | Cross-cutting for Alerts/Recommendations on mobile | Audit found **no push notification package** in the current Flutter app — this is a prerequisite gap to close before several modules (Weather Alerts, Disease Risk, Automation) are useful in the field |
| Payment/supplier integration | 5.12 Marketplace | Out of scope for this platform team; treat as a partnership/business decision, not just engineering |
| Maps SDK w/ offline tiles | 5.17 | Replaces the current placeholder "operational map" list screen |

**Correctness note carried over from the current-state audit:** the existing telemetry ingestion path (`NodeDataAppService.CreateAsync`) has `[AbpAuthorize]` commented out and uses plain REST rather than a device-appropriate protocol (MQTT/CoAP). Before layering 22 new modules on top of this ingestion path — several of which (Predictive Maintenance, Disease Risk, Automation Rules) depend on trustworthy, tamper-resistant telemetry — this should be hardened (device auth/API keys, and evaluation of MQTT for scale/battery efficiency on field devices). This is a **pre-requisite hardening item**, not a new module, and is called out explicitly in Section 19 (Risks).

---

## 10. Dashboard Concepts

Four role-tuned dashboards replace the current single generic agricultural-dashboard:

1. **Executive Command Center** — portfolio KPI tiles, regional map, financial/sustainability summary, benchmarking leaderboard.
2. **Agronomist Workspace** — AI Agronomist recommendation feed as the centerpiece, disease/pest risk grid, nutrient balance charts, crop calendar.
3. **Operations Dashboard (Facility Manager)** — today's alerts/incidents/tasks, weather-driven action items, equipment/inventory status.
4. **Technician Mobile Home** — assigned tasks/incidents on a map, nearest-first sorting, offline status indicator.

Each is a curated composition of existing and new widgets (gauge charts, trend lines, map layers, card lists) already proven in the current agricultural-dashboard/monitoring/alerts modules — no new charting technology required beyond what's in Section 14.

---

## 11. Reporting Requirements

- **Operational exports** (existing pattern, extend): NodeData Excel export (already exists via ClosedXML) → extend the same pattern to Weather History, Fertilizer Application log, Timeline, Financial breakdown.
- **Compliance/audit exports:** structured PDF packages per certification standard (5.15), must be tamper-evident (versioned, timestamped, ideally hash-referenced) given their legal/certification purpose.
- **Executive/board reports:** schedulable PDF snapshot of Command Center + Financial + Sustainability dashboards, emailed on a cadence (weekly/monthly) — a natural extension of ABP's existing background job infrastructure.
- **Regulatory reports (ESG):** template-driven, since regulatory formats vary by jurisdiction — design the `ESGReport` entity (5.14) with a `templateRef` from day one rather than hardcoding one format.

---

## 12. Mobile Experience

Covered in depth in Section 5.17. Summary of priority gaps to close, ranked by field-usability impact:
1. Real map with offline tiles (replaces placeholder list screen) — **highest impact**, since the app is used in low-connectivity rural settings.
2. Push notifications (currently absent — blocks real-time alerting value in the field).
3. Photo/voice/signature capture on Incident/Task closeout.
4. QR/NFC scan-to-open for Nodes/Equipment.
5. Making the existing offline-queue sync automatic on connectivity restore, not just callable.
6. Replace placeholder branding assets (stock photo header, leftover example-package file) — low effort, real trust/polish impact.

---

## 13. UI/UX Modernization Recommendations

The current Angular app (per audit) has real functional depth — permission-gated lazy-loaded modules, working SignalR real-time updates, Leaflet mapping, Google Charts gauges — but an **information architecture and visual system that hasn't been revisited since the original ABP/AdminLTE scaffold**, plus at least one module (agricultural-dashboard) with acknowledged prototype/sample data and inline logic rather than a server-driven design. This section reviews the platform holistically against the bar set by Azure/ArcGIS/FieldView/Deere Ops Center/Power BI.

### 13.1 Information Architecture & Navigation
- **Current issue:** flat top-level module list (Node, Alerts, Monitoring, GIS, Incidents, Facility, Organisation, Person) mixes entity-CRUD modules with workflow modules at the same nav level; will not scale to 22 more modules.
- **Improvement:** adopt the 6-zone grouped IA from Section 4 (Field Operations / Agronomy Intelligence / Operations & Response / Supply & Finance / Enterprise / Home), collapsible sections, with a persistent global search (see 13.5) as the fast path so deep nav depth doesn't hurt power users.
- **User journey:** first-time user lands on a role-appropriate Home; returning user's most-used zone stays expanded by default (remembered per-user).
- **Component hierarchy:** App Shell → Zone Rail (collapsible) → Module Tabs (where a zone has multiple sibling modules, e.g. Agronomy Intelligence's 5 sub-modules) → Content.

### 13.2 Dashboard Layouts & Cards
- **Current issue:** gauge-and-chart dashboard (agricultural-dashboard) is data-dense but not prioritized — everything presented with equal visual weight, and the "recommendation" text is generated inline in the component rather than being a first-class, dismissible, trackable object.
- **Improvement:** adopt a clear visual hierarchy: KPI tiles (glanceable numbers) → primary chart/map (the thing you're here for) → secondary detail (progressive disclosure, expandable cards) → recommendation feed as a distinct, persistent panel (once 5.8 exists) rather than transient text.
- **Expected benefit:** a manager can assess Facility status in under 5 seconds (KPI tiles) before choosing to drill in — matching the "glance, then dive" pattern of Power BI/Deere Ops Center.

### 13.3 Tables
- **Current issue:** standard ABP CRUD tables (per the boilerplate origin) — functional but generic: limited saved views, filter chips, or bulk actions expected in enterprise tools managing thousands of Nodes/Facilities.
- **Improvement:** add column customization, saved filter views (e.g., "My offline Nodes"), inline bulk actions (bulk acknowledge alerts, bulk assign tasks), sticky headers, and virtualized scrolling for the multi-hundred-Facility/thousand-Node scale targeted by the Command Center.

### 13.4 Charts
- **Current issue:** Google Charts gauges + line charts are functional but visually generic (default palette, no consistent design language across modules); the fl_chart-based mobile equivalent has a different visual language entirely, breaking cross-platform consistency.
- **Improvement:** define one shared chart design language (color-by-severity convention reused everywhere: green/amber/red for health, consistent category palette for crop/nutrient types) and apply it identically on web (Google Charts or a swap to a more themeable library) and mobile (fl_chart), so a moisture gauge looks like "the same product" on both.

### 13.5 Map Interactions, Filtering & Search
- **Current issue:** GIS module (Leaflet + clustering) is solid for its scope but map, node list, and alerts are three separate places to look for the same underlying entity; no global search exists.
- **Improvement:** add a global command-palette-style search (⌘K / Ctrl+K pattern, now standard in Azure/enterprise SaaS) that searches across Facilities, Nodes, Fields, Incidents, Equipment by name/ID; unify map filtering controls (status, severity, module layer) into one consistent filter panel component reused across GIS, Digital Twin, and Command Center maps rather than three bespoke implementations.

### 13.6 Forms & Wizards
- **Current issue:** CRUD forms follow ABP's default modal-form pattern, fine for simple entities (Person, Organisation) but not for multi-step domain flows like Crop Season creation (crop → variety → supplier → planting → population) or Drone Mission planning.
- **Improvement:** introduce a shared step-wizard component (progress indicator, back/next, save-draft) for the genuinely multi-step new flows (Crop Season, RFQ→Quote→PO, Automation Rule Builder, Audit Export), reserving single-modal forms for genuinely single-step CRUD (Field, Warehouse, Supplier).

### 13.7 Mobile Responsiveness & Dark Mode
- **Current issue:** no evidence of a dark mode or a documented responsive breakpoint system in the current Angular app; AdminLTE-era layouts often break down awkwardly at tablet widths, which matters for technicians/agronomists using tablets in the field.
- **Improvement:** define breakpoints (mobile <576, tablet 576–1024, desktop 1024–1440, wide/command-center >1440) and a dark theme as a first-class token set from the start of the design system work (Section 14), not retrofitted later — command-center/kiosk use cases (13.9) specifically benefit from dark mode (control-room glare, extended viewing).

### 13.8 Accessibility (WCAG 2.2 AA)
- **Current issue:** no accessibility audit evidence in the current codebase; default ABP/AdminLTE components and ad hoc chart components are unlikely to meet AA without deliberate work (color-only status indicators are a common gap — e.g., red/green severity dots with no text/icon backup).
- **Improvement:** every status/severity indicator gets an icon or text label in addition to color; full keyboard navigation audit of forms/tables/map controls; color contrast audit of the new design system palette (Section 14) at AA minimum; screen-reader labeling pass especially on chart/gauge components which are typically the worst offenders.

### 13.9 Empty States, Loading, Error Handling, Notifications, Micro-interactions
- **Current issue:** typical of ABP-scaffolded apps, these are likely default/minimal (spinner + blank table on empty data) rather than designed.
- **Improvement:** every new module ships with a designed empty state (e.g., "No Fields yet — Add your first Field" with a direct CTA, not a blank table), skeleton loading states (not just spinners) for dashboard widgets, toast-style non-blocking error notifications consistent with the existing SignalR real-time toast pattern already used for alerts, and subtle micro-interactions (state-change animations on Alert acknowledge, Incident status transitions) to reinforce that real-time actions registered.

### 13.10 Role-Based Dashboards / Workspaces
Covered functionally in Section 10; the UX implication is that **the same login should not always land on the same screen** — Home routes by role (Executive → Command Center, Agronomist → Recommendation feed, Manager → Facility ops dashboard), which is a meaningful IA change from the current single generic landing.

### 13.11 Multi-Monitor / Command Center Layouts
- **New requirement, not a current-state fix:** the Multi-Farm Command Center (5.16) should support a kiosk/wall-display layout mode — auto-refreshing, no interactive chrome, larger type, dark theme by default, designed for a NOC-style multi-monitor wall (common in Deere Ops Center-style enterprise deployments) — a distinct CSS layout mode, not a new screen.

---

## 14. Design System Recommendations

- **Token-based foundation:** color (with explicit severity/status semantic tokens: healthy/warning/critical/offline, consistent across web and mobile), typography scale, spacing scale, breakpoints — defined once, consumed by both Angular (CSS custom properties / SCSS tokens) and Flutter (a matching Dart theme constants file), so the two clients stop drifting visually.
- **Component library:** formalize the recurring patterns already implicit in the app (KPI tile, status badge, gauge card, trend chart card, map layer toggle, timeline entry, step wizard) as a documented, reusable component set rather than one-off implementations per module — critical given 22 new modules will otherwise each reinvent these.
- **Iconography:** one consistent icon set across severity levels, module identities (crop/pest/disease/weather/equipment each need a recognizable glyph used consistently in nav, cards, and map pins), and status states.
- **Reference bar:** the explicit product comparisons in the brief (Azure, ArcGIS, Trimble Ag, FieldView, Deere Ops Center, Power BI) share a common trait worth naming: dense information, calm color use (color reserved for meaning — status/severity — not decoration), and heavy reliance on progressive disclosure (summary first, detail on demand). That's the design principle to hold the whole system to, more than any specific visual style.

---

## 15. Enterprise Scalability Considerations

- **Query performance at scale:** Command Center (5.16) and Benchmarking (5.22) both require aggregation across hundreds of Facilities / thousands of Nodes; the plan's `FacilitySummarySnapshot`/`BenchmarkSnapshot` materialization pattern (scheduled rollups, not live cross-joins) is the mitigation — this must be designed before those two modules are built, not after a slow-dashboard incident.
- **Telemetry ingestion volume:** thousands of Nodes reporting periodically is a write-heavy, high-frequency path already in production; before adding Predictive Maintenance/Disease Risk/Automation jobs that all *read* this data on a schedule, evaluate read-replica or time-series-optimized storage for `NodeData`/`WeatherObservation` so analytical jobs don't contend with live ingestion.
- **Imagery/blob storage growth:** Satellite and Drone imagery (5.6, 5.7) introduce large binary assets at a scale existing infrastructure hasn't needed to handle — needs its own storage/CDN strategy and lifecycle/retention policy (raw imagery doesn't need to be retained forever at full resolution).
- **Background job load:** Weather sync, Disease Risk evaluation, Device Health scoring, Automation Rule evaluation, Benchmark snapshotting all add scheduled jobs on top of the existing `MonitoringExecutionHistory` job pattern — needs a job-scheduling capacity plan (stagger schedules, avoid thundering-herd on the same cron tick) as module count grows.
- **Multi-tenancy boundaries:** every new entity must carry tenant/organisation scoping consistent with ABP's existing multi-tenancy filters; Marketplace/Benchmarking are the two modules where a *cross-tenant* view might someday be tempting (industry benchmarks, open marketplace) — treat that as an explicit, separately-governed exception, not a default.

---

## 16. Security and Permissions Matrix

Extends the existing ABP permission convention (`Pages.<Module>[.<Action>]`), Facility-scoped by tenant as today. Representative matrix (not exhaustive — full permission tree to be finalized during design):

| Module | Executive | Agronomist | Facility Manager | Technician | Compliance Officer |
|---|---|---|---|---|---|
| Weather | View | View, Configure | View, Configure | View | View |
| Crop Management | View | Manage | Manage | View | View |
| Fertilizer/Nutrient | View | Recommend, Manage | Apply, Manage | Apply (field entry) | View (audit) |
| Disease/Pest Risk | View | Manage, Treat | View, Treat | Inspect, Treat | View |
| Satellite/Drone Imagery | View | View, Plan Missions | View | View | — |
| AI Agronomist | View, Configure Rules | View, Feedback | View, Feedback | View | — |
| Predictive Maintenance | View | View | View, Schedule | View, Complete | — |
| Equipment | View | — | Manage | Service | View (audit) |
| Inventory | View | — | Manage, Transfer | View | View (audit) |
| Marketplace | Approve Orders | — | Order (below threshold) | — | — |
| Financial Analytics | Full | — | Facility-scoped | — | — |
| Sustainability/Compliance | View, Report | View | View | — | Full, Audit Export |
| Command Center | Full | Region-scoped view | Facility-scoped view | — | — |
| Automation Rules | View | Author | Author, Activate | — | View (audit) |
| Benchmarking | Full | Region-scoped | Facility-scoped | — | — |

**Cross-cutting principle:** Financial and cross-Facility comparative data (Financial Analytics, Benchmarking, Command Center) are the most sensitive new surfaces and should default to Executive/Manager-only visibility, with explicit opt-in extension to other roles per tenant — not the reverse.

---

## 17. Non-Functional Requirements

- **Availability:** real-time modules (Alerts, Automation, Command Center) inherit the existing SignalR-based real-time expectation; target should be defined per module criticality (e.g., weather-alert delivery latency matters more than a benchmarking report's freshness).
- **Data retention:** telemetry, imagery, and audit/compliance data have different retention needs — compliance/audit records likely need multi-year retention for certification purposes; raw high-resolution imagery likely needs a tiered/archival retention policy (Section 15).
- **Auditability:** every automated action (Automation Rules, AI Agronomist auto-executed recommendations) must be logged with enough context to answer "why did the system do this" after the fact — this is both a UX requirement (13, explainability) and a compliance requirement (5.15).
- **Explainability as a hard requirement, not a nice-to-have:** given Section 5.8's design (structured reasoning data, not just prose), this should be treated as a non-functional requirement across every module that generates a recommendation or automated action, not just the AI Agronomist module itself.
- **Localization:** if Farmru expands beyond its current market, weather/crop/compliance terminology and units (metric vs. imperial, regional crop names) need a localization strategy — flagged for scoping discussion, not assumed.
- **Performance budgets:** Command Center and Digital Twin views should have an explicit load-time budget (e.g., initial KPI tiles interactive in <2s even at max scale) given they're the "first thing an executive sees."

---

## 18. Implementation Phases (Phase 1–Phase 5)

Sequenced to respect dependencies identified throughout this document (e.g., Yield Prediction needs Crop Management data first; Digital Twin needs its source layers first; Benchmarking needs Financial/Sustainability data first).

**Phase 1 — Foundational Intelligence (Q1–Q2)**
Weather Intelligence (5.1), Crop Management (5.2), Fertilizer & Nutrient Management (5.3), Farm Activity Timeline (5.21, starts capturing history from day one). Also: telemetry ingestion hardening (auth + protocol evaluation), push notifications for mobile (prerequisite gap).

**Phase 2 — Decision Intelligence (Q2–Q3)**
AI Agronomist (5.8, rules-based MVP), Disease Risk Intelligence (5.4), Pest Monitoring (5.5), Predictive Maintenance (5.9), Automation Rules Engine (5.19). These consume Phase 1 data and give the platform its "brain."

**Phase 3 — Spatial & Field Operations (Q3–Q4)**
Satellite Imagery (5.6), Technician Mobile Experience overhaul (5.17), Equipment Management (5.10), Inventory Management (5.11), Farm Digital Twin (5.18, composes Phase 1–3 layers), Workflow Designer (5.23, immediately after Phase 2's Rules Engine). UI/UX modernization (Section 13) rolled out module-by-module alongside these, not as a separate big-bang release.

**Phase 4 — Business & Enterprise Scale (Q4–Q5)**
Financial Analytics (5.13), Multi-Farm Command Center (5.16), Carbon & Sustainability (5.14), Compliance (5.15), Drone Integration (5.7, opportunistic/customer-driven).

**Phase 5 — Advanced Analytics & Ecosystem (Q5–Q6)**
Yield Prediction (5.20, needs a full season of Phase 1–3 data to be credible), Benchmarking (5.22, needs Phase 4 financial/sustainability data), Marketplace (5.12, pending business/partnership groundwork).

Each phase should ship its UX modernization work (Section 13) for its own new modules concurrently — retrofitting the *existing* modules' UX (13.1–13.9) is recommended as a parallel workstream starting in Phase 1, not deferred to the end.

---

## 19. Risks and Dependencies

| Risk | Impact | Mitigation |
|---|---|---|
| Telemetry ingestion currently lacks robust auth and a broker protocol | Undermines trust in every downstream intelligence module (garbage in, garbage out) | Harden as a Phase 1 prerequisite, not a "someday" item |
| AI Agronomist / Yield Prediction launched before enough historical data exists | Low-confidence or wrong recommendations damage trust in the whole platform, hard to recover | Explicit confidence scoring from day one; gate Yield Prediction to Phase 5; start rules-based (deterministic, explainable) before any ML |
| Command Center / Benchmarking query performance at enterprise scale | Slow dashboards undermine the "executive glance" value proposition | Materialized snapshot pattern designed up front (Section 15), load-tested against realistic scale before launch |
| Third-party weather/imagery provider cost scales with Facility count | Margin risk as tenant base grows | Model provider cost per Facility into pricing before Phase 1 GA, not after |
| Marketplace as an open two-sided market | Legal/payments/trust-and-safety scope far beyond platform engineering | Scope MVP as closed, tenant-curated supplier network only (Section 5.12) |
| Mobile offline-sync reliability | Field data loss/duplication if sync logic isn't robust (audit found current sync isn't confirmed connectivity-triggered) | Prioritize as part of Phase 3 mobile overhaul, with explicit conflict-resolution design |
| Scope size (22 modules + full UX modernization) | Risk of spreading thin, shipping many half-finished modules | Phasing (Section 18) is designed to ship *complete, usable* module clusters per phase rather than thin slices of everything at once |
| Design system drift between Angular and Flutter | Two clients feel like two products | Shared token definitions (Section 14) established before Phase 1 module UI work begins |

---

## 20. Success Metrics (KPIs)

**Adoption & engagement**
- % of Facilities with at least one active Automation Rule
- AI Agronomist recommendation acceptance rate (target trend: increasing over time as trust builds)
- Weekly active technicians using mobile map/task features

**Operational impact**
- Mean time-to-acknowledge / time-to-resolve for Alerts and Incidents (should improve as Automation/Recommendations mature) — track as a distribution (P50/P95), not just a mean, since P95 is what tells you the process is failing for the hard cases
- % reduction in unplanned Node downtime after Predictive Maintenance rollout
- Disease/pest treatment response time from risk detection to treatment logged
- Equipment uptime % (from Equipment Management service/downtime records)
- Workflow Designer: % of instances completing without manual intervention/escalation, mean time-in-step, count of instances stuck past SLA

**Business impact**
- Cost-per-hectare trend across the portfolio (Financial Analytics adoption proxy)
- Yield Prediction accuracy (predicted vs. actual, narrowing error % season over season)
- Measured yield improvement season-over-season for Facilities actively using recommendations, vs. a baseline cohort that isn't (the honest way to attribute impact rather than just citing platform-wide yield trends, which are confounded by weather)
- Water usage reduction (irrigation volume vs. Facility baseline, for Facilities using Weather/Automation-driven irrigation guidance)
- % of Facilities with a complete Compliance/ESG export available on demand

**Platform quality**
- Command Center dashboard load time at scale (P95, against the budget in Section 17)
- WCAG 2.2 AA conformance score across modernized modules
- Cross-platform (web/mobile) design-system consistency (component reuse %, qualitative design review)

**Trust/retention (leading indicators of product-market fit for the intelligence layer specifically)**
- Recommendation dismissal reasons (tracked qualitatively — are users dismissing because recommendations are wrong, or because the action is already handled elsewhere?)
- Rule Execution Log review rate (are users actually checking automation output, a proxy for trust calibration)

---

## 21. Architecture Decision Records (ADR Log)

ADRs are not written here in full (each is properly a standalone, timestamped document authored *at the moment of decision*, not pre-written speculatively) but the plan should commit to the practice from Phase 1 onward, and the following decisions are already identifiable as needing one before Phase 1 build starts:

| ADR # | Decision needed | Key alternatives to weigh | Why it can't be deferred |
|---|---|---|---|
| ADR-001 | Weather data provider | Commercial agri-weather API vs. general-purpose weather API vs. self-hosted model blend | Cost scales with Facility count (Section 19 risk); switching later means re-mapping every `WeatherObservation` consumer |
| ADR-002 | Satellite imagery provider | Sentinel-Hub-class API vs. direct provider partnership vs. multi-provider abstraction | Pricing/resolution tradeoffs directly affect NDVI usefulness and cost-per-hectare economics |
| ADR-003 | Rules/Workflow engine build-vs-buy | Build in-house (as specified in 5.19/5.23) vs. embed an existing open-source workflow/BPMN engine | Determines whether the visual builder is bespoke UI over custom domain logic or a themed wrapper over a third-party engine — very different engineering cost and long-term flexibility tradeoffs |
| ADR-004 | Telemetry protocol hardening | Keep REST + add device API-key auth vs. migrate to MQTT vs. support both | Directly blocks/unblocks Predictive Maintenance and Disease Risk trustworthiness (Section 9) |
| ADR-005 | Time-series storage for `NodeData`/`WeatherObservation` | Keep in SQL Server vs. add a dedicated time-series store (e.g., TimescaleDB-class extension) alongside it | Affects every analytical module's query performance at scale (Section 15/23) |
| ADR-006 | Blob/imagery storage | Cloud object storage (e.g., Azure Blob, given existing ASP.NET Core hosting) vs. alternative | Needed before Drone/Satellite/Mobile-attachment modules can ship (Section 9) |
| ADR-007 | Client-side vs. server-side composition for Digital Twin / Command Center | Orchestration AppService composing existing services server-side vs. client composing multiple existing API calls | Affects latency budget (Section 26) and caching strategy at scale |
| ADR-008 | **Validation responsibility** (DTO shape vs. AppService cross-field vs. domain invariant vs. cross-aggregate rules, and the mandatory `DomainRuleException`→`UserFriendlyException` boundary translation) | Introduce FluentValidation; introduce a global exception filter now; leave each module to reinvent its own convention | Resolved during Phase 1 Sprint 0 when DTO validation was found to have no existing convention at all — recorded as **Accepted** in `/docs/adr/ADR-008-Validation-Responsibility.md`, the first ADR actually closed rather than left as a placeholder in this log |
| ADR-009 | **Alert classification strategy** (how Weather, and later Disease Risk/Pest Monitoring/Predictive Maintenance, classify alerts on the shared `Alert` aggregate) | Introduce a new `AlertSource` field/enum on `Alert` (this document's original assumption) | Resolved during Phase 1 Sprint 1 Checkpoint 2 when implementation found `Alert` has no `AlertSource` concept — recorded as **Accepted** in `/docs/adr/ADR-009-Alert-Classification-Strategy.md`: Phase 1 extends the existing `AlertType` enum instead; `AlertSource` is deferred pending evidence from Phase 2's alert-producing modules |

**Process recommendation:** adopt a lightweight ADR template (Context → Decision → Alternatives Considered → Consequences), stored in-repo alongside the code it governs (e.g., `/docs/adr/`), one file per decision, numbered sequentially, never edited after acceptance — only superseded by a new ADR that references it. This gives future engineers (and future planning documents) a trail of *why*, which this document establishes the need for but deliberately doesn't pre-empt, since real alternatives can only be weighed properly at build time with current vendor pricing/capability in hand.

---

## 22. Bounded Context Map

Mapping the 23 modules above (5.1–5.23) plus the existing system onto bounded contexts clarifies data ownership before code makes the boundaries harder to see. Existing ABP modules already imply four contexts; new modules mostly extend them rather than creating entirely new ones:

```
┌─────────────────────────────┐   ┌─────────────────────────────┐
│  IDENTITY & TENANCY (existing)│  │  FIELD OPERATIONS            │
│  Organisation, Person, Roles  │◄─┤  Facility, Field, Node,      │
│  (owns: who, and which tenant)│  │  NodeData, GeoFence, Weather,│
└───────────────┬───────────────┘  │  Satellite/Drone Imagery     │
                │                  │  (owns: where, and raw sense)│
                │                  └───────────────┬───────────────┘
                │                                  │
                ▼                                  ▼
┌─────────────────────────────┐   ┌─────────────────────────────┐
│  AGRONOMY INTELLIGENCE       │   │  OPERATIONS & RESPONSE       │
│  Crop, Nutrient, Disease,    │──►│  Alert, Incident, Task,      │
│  Pest, Recommendation,       │   │  Automation Rule, Workflow,  │
│  Yield Prediction            │◄──│  Device Health/Maintenance   │
│  (owns: what should happen)  │   │  (owns: what is happening /  │
└───────────────┬───────────────┘   │   what was done about it)   │
                │                  └───────────────┬───────────────┘
                │                                  │
                ▼                                  ▼
┌─────────────────────────────┐   ┌─────────────────────────────┐
│  SUPPLY & ASSETS             │   │  BUSINESS & COMPLIANCE       │
│  Equipment, Inventory,       │──►│  Financial Analytics,        │
│  Marketplace, Suppliers      │   │  Sustainability, Compliance, │
│  (owns: physical stuff)      │   │  Benchmarking, Reporting     │
└─────────────────────────────┘   │  (owns: is it working, is it │
                                   │   provable)                  │
                                   └─────────────────────────────┘

  Cross-cutting (read-only composition over all contexts, own no data):
  Command Center · Digital Twin · Farm Activity Timeline
```

**Ownership rules to enforce architecturally, not just document:**
- Each context owns writes to its own entities; other contexts *reference by ID*, never write across the boundary directly (e.g., Nutrient Management deducts Inventory stock via `InventoryAppService.RecordConsumption(...)`, not a direct table write into `StockMovement`).
- The three cross-cutting composition layers (Command Center, Digital Twin, Timeline) are explicitly **read-only aggregators** — this is the architectural rule that keeps them from becoming an accidental "God module"; if a feature needs to *write* something, that write belongs in one of the five owning contexts above, never in a composition layer.
- Workflow Designer (5.23) is the one legitimate exception that *orchestrates* writes across contexts — but it does so by calling each context's own AppService as a black box (per 5.23's integration note), not by owning cross-context data itself.

---

## 23. Database Growth & Retention Strategy

| Data category | Growth profile | Retention approach |
|---|---|---|
| `NodeData` (telemetry) | Highest volume, continuous, per-Node per-interval | Full-resolution retained for a rolling recent window (e.g., 90 days); rolled up into hourly/daily aggregates (`NutrientBalanceHistory`-style materializations, Section 8) beyond that; raw retained longer only if a time-series store (ADR-005) makes it cheap to do so |
| `WeatherObservation`/`WeatherForecastDaily/Hourly` | High volume, per-Facility | Similar rolling-window + aggregate pattern; forecasts specifically have no long-term value once the date has passed except for model-accuracy backtesting, so archive forecasts alongside actuals for that purpose only |
| `SatelliteImageLayer`/`DroneImageryAsset` (raw imagery) | Large binary, growing fastest in storage bytes even if slow in row count | Tiered storage: recent imagery hot/fast-access, older imagery moved to cool/archive tier; keep computed zonal stats (`SatelliteZoneStat`) indefinitely even after raw imagery is archived, since stats are small and drive trend charts |
| `Alert`, `Incident`, `RuleExecutionLog`, `WorkflowStepInstance` (operational/audit events) | Moderate volume, high *value* per row (audit trail) | Retain indefinitely or per compliance-mandated minimum (Section 15/29 compliance dependency) — these are exactly the records Compliance/Audit exports (5.15) depend on; never silently purge without an explicit, tenant-configurable retention policy |
| `TimelineEventIndex`, `FacilitySummarySnapshot`, `BenchmarkSnapshot` (derived/materialized) | Rebuildable from source data | Can be pruned/rebuilt aggressively since they're not source-of-truth — lowest-risk data to have an aggressive retention policy on |
| `CostEntry`/`RevenueEntry`/`SustainabilityMetric` (financial/ESG) | Low-moderate volume | Retain per typical financial-record statutory minimums (jurisdiction-dependent — flagged, not assumed, same as localization in Section 17) |

**Aggregation strategy:** every high-volume category above should define its rollup granularity *at the same time* its raw entity is designed (Section 8), not retrofitted — e.g., `NodeData` should ship with an hourly-aggregate table from Phase 1, since Predictive Maintenance (Phase 2) and Yield Prediction (Phase 5) both need multi-month trend queries that must not require scanning raw high-frequency rows.

**Archiving:** define an explicit archive tier (cheaper storage, slower access, still queryable for compliance/audit) rather than a binary retain-or-delete choice — this directly serves the Compliance module's multi-year audit requirement without keeping everything hot forever.

**This connects directly to ADR-005** (Section 21) — the retention/aggregation strategy above is materially easier with a purpose-built time-series store than with SQL Server alone at the target scale (Section 26), so that ADR should be resolved before Phase 1's `NodeData`-adjacent schema (Weather, in particular) is finalized.

---

## 24. API Versioning Strategy

- **Internal (AppService) APIs:** the Angular/Flutter clients consuming ABP AppServices directly are effectively "first-party internal" APIs today — versioning discipline here can be lighter (coordinated client/server deploys), but as the module count grows, additive-only changes (new optional fields, new endpoints) should be the default, with breaking changes requiring a documented migration note per the ADR practice above.
- **External/public APIs (new surface, not present today):** several new modules imply genuine external integration surfaces that need real versioning discipline:
  - Device/telemetry ingestion endpoint (`NodeDataAppService.CreateAsync` today) — this is the most external-facing endpoint in the system (talks to physical devices in the field, which can't be redeployed instantly) and should be the first to get explicit versioning (e.g., `/api/v1/nodedata`) before any protocol hardening (ADR-004) ships, since devices in the field are the hardest client to force an upgrade on.
  - Marketplace supplier integration (5.12) — external supplier systems will need a stable, versioned contract independent of internal refactors.
  - Any future partner/third-party integration (e.g., a customer's own BI tool pulling Farmru data) — should go through a deliberately versioned, documented API surface, not raw AppService endpoints.
- **Version lifecycle policy:** adopt a simple N/N-1 support policy for any versioned external endpoint (current version + one prior supported simultaneously), with a published deprecation notice period (e.g., 6 months) before a version is retired — critical for the device-ingestion endpoint specifically, since field hardware upgrade cycles are slow.
- **Deprecation communication:** version deprecation should itself raise a `Recommendation`-style or admin-dashboard notice for tenants/integrators still on an old version, reusing the platform's own notification infrastructure rather than relying on external email announcements alone.

---

## 25. Security Architecture

Extends the current-state audit finding (commented-out `[AbpAuthorize]` on telemetry ingestion) into a full security posture for the platform as it grows to include financial, compliance, and device-facing surfaces:

- **Device authentication:** every physical Node must authenticate with a per-device credential (API key or certificate-based, tied to the Node's identity in the domain model) rather than the current apparently-open ingestion endpoint — this is the single highest-priority security item in the whole plan, since it's both a data-integrity prerequisite for every intelligence module (Section 9) and the most field-exposed attack surface in the system.
- **API security:** standard JWT bearer auth (already in place per the current audit) extended with rate limiting on ingestion endpoints (protects against a compromised/malfunctioning device flooding the system), and explicit `[AbpAuthorize(PermissionName)]` attributes audited across *all* AppServices as a Phase 1 hardening pass, not just the one endpoint already flagged.
- **Secrets management:** API keys for external providers (weather, satellite imagery, SMS/push) and device credentials must live in a secrets store (e.g., Azure Key Vault or equivalent, given the existing Azure-adjacent hosting posture implied by the stack) rather than appsettings/config files — worth an explicit audit of current secret handling as part of Phase 1, since ABP starter templates commonly ship with connection strings in plain config.
- **Encryption:** TLS in transit (assumed already standard); at-rest encryption for the database and blob/imagery storage; field-level encryption consideration for any PII in `Person` records (contact details) if the platform expands into jurisdictions with strict data-protection regimes.
- **Multi-tenant isolation:** the existing ABP tenant-filter pattern must be extended consistently to every new entity (Section 8's cross-cutting note) — the specific new risk is the **composition/aggregation layers** (Command Center, Digital Twin, Timeline, Benchmarking) since they by design query across scopes; these must enforce tenant/organisation boundaries at the query layer even when "compare across my Facilities" superficially resembles "compare across tenants" in the UI — this is worth a dedicated security review pass specifically on the Benchmarking and Command Center AppServices before launch, since they're the two modules most likely to accidentally leak cross-tenant data through an aggregation query.
- **Audit logging:** every write from an automated source (Automation Rules, Workflow steps, AI Agronomist auto-executed recommendations) must log actor-as-system with full context (already designed into `RuleExecutionLog`/`WorkflowStepInstance`/`RecommendationFeedback` in Section 5) — this section's addition is the requirement that *manual* privileged actions (financial approval, compliance export, marketplace order approval) get the same audit rigor, likely via ABP's existing audit-logging module extended to cover the new AppServices.

---

## 26. Performance Targets

Concrete, testable targets — placeholders for numbers that should be finalized with real infrastructure sizing, but directional targets are set now so Phase 1 architecture decisions (ADR-005, snapshot materialization) are made against a number, not a vibe:

| Metric | Target | Rationale |
|---|---|---|
| Dashboard initial interactive load (Facility-scoped) | < 2s P95 | Matches Section 17's "executive glance" non-functional requirement |
| Command Center load at max scale (500 Facilities) | < 3s P95 for KPI tiles; map/detail can progressively load after | Materialized snapshot pattern (Section 15/23) is the mechanism |
| Map rendering (Facility/Node clustering) | < 1.5s to first paint at up to 5,000 clustered markers | Existing Leaflet clustering already handles current scale; target validates it holds at 10x |
| Telemetry ingestion throughput | Sustain 10,000 Node check-ins/minute at target scale, with headroom to 3x burst | Sized to the "thousands of Nodes" scale named in the brief (Section 5.16) |
| Concurrent users per tenant (web) | 200 concurrent without degraded SignalR real-time latency | Reflects an enterprise customer's regional ops team size |
| Alert/Incident real-time delivery latency (SignalR) | < 2s from server event to client toast | Preserves the existing real-time UX already built, as a regression guard while adding load |
| Maximum supported Nodes per tenant (soft target) | 25,000 | Informs ADR-005 (time-series store) and Section 23 retention decisions |
| Maximum supported Facilities per tenant (soft target) | 1,000 | Informs Command Center/Benchmarking snapshot refresh cadence |
| Automation Rule / Workflow evaluation latency | < 5 min from trigger condition true to action taken (batch-evaluated), < 10s for event-triggered rules | Distinguishes scheduled threshold rules from event-driven ones (Section 5.19) |

These targets should be re-validated with load testing at the end of each phase (Section 18) against the actual scale reached, not just assumed correct from Phase 1 planning.

---

## 27. UI Component Inventory

A pre-build inventory of shared components, so 23 modules consume a common library rather than each reinventing its own version of the same pattern (directly extends Section 14's Design System Recommendations with the concrete list to build):

| Component | Used by (examples) | Key variants needed |
|---|---|---|
| KPI Tile | Command Center, every module's dashboard header | value + trend arrow + sparkline; severity-colored variant |
| Metric/Gauge Widget | Agricultural dashboard, Nutrient Balance, Device Health | radial gauge, linear gauge, threshold-banded |
| Status Badge | Node status, Alert severity, Incident status, Workflow step status, Certification validity | color + icon combined (never color-only, per Section 13.8) |
| Trend/Line Chart Card | Weather history, NDVI trend, Financial trend, Yield trend | single-series, multi-series comparison, forecast-band overlay |
| Map (base + layer system) | GIS, Digital Twin, Command Center regional map, Pest hotspot map | clustering, choropleth/heatmap, polygon draw/edit (Fields, GeoFences), layer toggle panel |
| Timeline Entry List | Farm Activity Timeline, Incident history, Workflow instance history | icon-coded by event type, expandable detail |
| Step Wizard | Crop Season creation, RFQ→PO, Drone Mission planning, Audit Export | linear and branching variants |
| Data Table | Every list screen | saved views, column customization, bulk actions, virtualized scroll (Section 13.3) |
| Filter Panel | Map filters, table filters, Recommendation feed filters | consistent facet-filter pattern reused everywhere rather than bespoke per module |
| Recommendation/Insight Card | AI Agronomist feed, Disease Risk cards, Yield risk factors | reasoning-trace expandable section, accept/dismiss/override actions, confidence indicator |
| Rule/Workflow Builder Canvas | Automation Rules Engine, Workflow Designer | shared node-and-connector editor (explicitly called out as reused, not duplicated, in 5.23) |
| Approval/Task Inbox Item | Workflow step assignments, Task module, Incident assignment | consistent "what's asked of me + one-tap action" pattern across web and mobile |
| Empty State | Every module, first-use | illustration/icon + message + primary CTA, per Section 13.9 |
| Global Search Result Item | Command palette (Section 13.5) | entity-type-aware icon + breadcrumb context |

**Build sequencing note:** this inventory should be built as a shared component library **starting in Phase 1**, even though only a subset of modules exist yet — retrofitting a shared library after 8–10 modules have already built one-off versions is far more expensive than establishing it early against the first 3–4 modules.

---

## 28. Design Tokens

Concrete token categories to define once, before Phase 1 module UI work begins (extends Section 14):

- **Color:** brand palette; semantic status tokens (`status-healthy`, `status-warning`, `status-critical`, `status-offline`, `status-info`) mapped consistently across web (SCSS variables) and mobile (Dart theme constants); light and dark variants for every semantic token (Section 13.7).
- **Typography:** type scale (display/heading/body/caption sizes), font family, weight scale — shared across Angular and Flutter, accepting platform-native rendering differences.
- **Spacing & Grid:** base spacing unit (e.g., 4px/8px scale), responsive grid/column system, consistent card/panel padding.
- **Elevation:** shadow/z-index scale for cards, modals, the Rule/Workflow builder canvas's floating panels, map overlay panels.
- **Border Radius:** consistent radius scale (cards, buttons, badges, chart containers) — currently likely inconsistent given the ABP/AdminLTE-to-custom-module transition visible in the audit.
- **Breakpoints:** as defined in Section 13.7 (mobile/tablet/desktop/wide-command-center).
- **Animation/Motion:** duration and easing scale for micro-interactions (Section 13.9) — state-change transitions (Alert acknowledged, Workflow step advanced) should feel consistent in timing across every module rather than each component picking its own.
- **Iconography:** one icon set, with an explicit mapping table from domain concept → icon (crop, pest, disease, weather, equipment, workflow step types, etc.) maintained centrally so a new module doesn't invent a new glyph for a concept that already has one.

Tokens should be implemented as a single source of truth (e.g., a JSON/YAML token file) consumed by both a web build step (generating SCSS variables) and a mobile build step (generating a Dart constants file), so the two clients cannot silently drift — directly addressing the "two clients feel like two products" risk flagged in Section 19.

---

## 29. Module Dependency Matrix

Explicit prerequisite chains (supersedes the informal dependency notes scattered through Section 5, collected here as a single reference):

| Module | Hard prerequisite(s) | Soft/enhancing dependency |
|---|---|---|
| Crop Management (5.2) | Facility/Field geolocation (existing) | — |
| Fertilizer & Nutrient Mgmt (5.3) | Crop Management (stage context), existing NPK telemetry | Inventory (stock deduction) |
| Disease Risk Intelligence (5.4) | Weather Intelligence, Crop Management (stage) | AI Agronomist (explanation layer) |
| Pest Monitoring (5.5) | Facility/Field geolocation | GIS map (hotspot layer), Compliance (spray record evidence) |
| Satellite Imagery (5.6) | Field polygon definition (from Crop Management) | Crop Management (per-season overlays) |
| Drone Integration (5.7) | Blob storage (ADR-006) | Satellite Imagery (shared imagery viewer component) |
| AI Agronomist (5.8) | At least one producing module (Nutrient, Disease, or Weather) | All intelligence modules, over time |
| Predictive Maintenance (5.9) | Existing Node telemetry (battery/solar/signal) | Automation Rules Engine (auto-task creation) |
| Equipment Management (5.10) | — (standalone) | Inventory (parts consumption), Financial Analytics (utilization cost) |
| Inventory Management (5.11) | — (standalone) | Fertilizer, Pest/Spray, Equipment (all deduct stock) |
| **Marketplace (5.12)** | **Inventory Management** (reorder trigger is the primary entry point) | Financial Analytics (purchase cost feed) |
| Financial Analytics (5.13) | Fertilizer, Equipment, Inventory, Crop Management (yield) all producing cost/revenue data | — |
| Carbon & Sustainability (5.14) | Fertilizer, Equipment/Fuel data | Financial Analytics, Compliance |
| Compliance (5.15) | Pest/Spray Records, Fertilizer, Personnel/Certifications | Sustainability (shared evidence) |
| Multi-Farm Command Center (5.16) | Snapshot/materialization infra (ADR-005-adjacent) | Weather (regional risk overlay), Financial/Sustainability (portfolio KPIs) |
| Technician Mobile Experience (5.17) | Maps SDK selection, push notification provider | Automation/Workflow (closeout-triggered next steps) |
| **Farm Digital Twin (5.18)** | **Weather + Crop + Satellite Imagery + Predictive Maintenance + Equipment** (composes all of them) | — (pure composition, build last among its inputs) |
| Automation Rules Engine (5.19) | At least one metric-producing module to write a rule against | Workflow Designer (shared substrate) |
| **Yield Prediction (5.20)** | **Crop Management + Weather + one full season of `HarvestRecord` data** | Nutrient Management, Satellite Imagery (additional input signals) |
| Farm Activity Timeline (5.21) | Domain event bus wiring (architectural, not a module dependency) | Every transactional module, as event producers |
| **Benchmarking (5.22)** | **Financial Analytics + Sustainability + Crop Management (yield)** | Equipment (utilization comparison) |
| Workflow Designer (5.23) | Automation Rules Engine (shared builder/condition substrate) | Every action-producing module, as step targets |

**Reading the bolded rows:** these are the chains most likely to be violated under delivery pressure (e.g., someone proposes shipping Yield Prediction early as a "quick win") — they're the ones worth defending explicitly in sprint planning against scope pressure, since shipping them early produces a feature that *looks* done but is built on empty or shallow data, which is worse for trust than not shipping it yet (Section 19 risk).

---

## 30. Implementation Governance

A short closing note on how this document should be used, since Sections 21–29 turn it from a feature-and-phasing plan into something closer to a build charter:

- **This document is the Phase 0 artifact.** Section 21's ADR log entries (ADR-001 through ADR-007) are the literal first work items — each should be resolved (as a real, dated ADR) before its dependent Phase 1 module's technical design begins.
- **The Bounded Context Map (22) and Module Dependency Matrix (29) should be kept alive, not archived.** As modules ship, update both — they're the two artifacts most likely to catch an accidental architecture violation (a module writing across a context boundary it shouldn't, or a team starting a module out of dependency order) before it's expensive to unwind.
- **The UI Component Inventory (27) and Design Tokens (28) are Phase 1, Sprint 1 work**, not backlog items to get to eventually — every module built before the shared library exists is a module that will need retrofitting later.
- **Next concrete step**, unchanged from the prior version of this document: a Phase 1 detailed technical design (entity schemas, AppService contracts, Angular module scaffolding, Flutter screen specs) for Weather Intelligence, Crop Management, and Fertilizer & Nutrient Management — now additionally informed by ADR-001, ADR-004, and ADR-005 above, which should be resolved first since they materially shape those three modules' schemas.

---

## 31. Feature Dependency Diagram

Section 29's matrix is the reference table; this is the same information as a graph, which reads faster for spotting what blocks what at a glance.

```text
Weather ────────────┬──────────────► Disease Risk
                     │                     │
Crop Mgmt ───────────┼──────────────► AI Agronomist ──► Recommendation Feed
                     │                     ▲
Nutrient Mgmt ───────┘                     │
                                            │
Satellite Imagery ──────────────────► Yield Prediction
        │                                  ▲
        │                                  │
Crop Mgmt (Field polygons) ────────────────┘
        │
        └──► Drone Integration (shares imagery viewer)

Existing Node Telemetry ───────────► Predictive Maintenance ──► Automation Rules Engine
                                                                        │
                                                                        ▼
                                                              Workflow Designer
                                                                        │
                          ┌─────────────────────────────────────────────┤
                          ▼                                             ▼
                   Every action-producing module              Farm Activity Timeline
                   (Incident, Task, Purchase Request)          (all modules feed this)

Inventory Mgmt ─────────────────────► Marketplace
        ▲
        │
Fertilizer / Pest-Spray / Equipment Service  (all deduct stock)

Fertilizer + Equipment + Crop Mgmt (yield) ──► Financial Analytics ──► Benchmarking
                                                        │                   ▲
                                                        ▼                   │
                                              Carbon & Sustainability ──────┘

Weather + Crop + Satellite Imagery
    + Predictive Maintenance + Equipment ──► Farm Digital Twin  (pure composition, builds last)

Snapshot/materialization infra (ADR-005) ──► Multi-Farm Command Center
```

**How to read it:** anything with arrows pointing *into* it has a hard prerequisite; anything that's purely an arrow *source* with no incoming arrows (Weather, Crop Management's core, Inventory, existing Node Telemetry, Automation Rules Engine) is safe to start without waiting on other new modules — these are the natural Phase 1/Phase 2 anchor points already reflected in Section 18's phasing, and this diagram is the visual proof of why that phasing is ordered the way it is, not an arbitrary business call.

---

## 32. Capability Heat Map

A single-glance view of where every capability sits on the roadmap — the artifact executives and customers actually read first.

| Capability | MVP (Phase 1) | Phase 2 | Phase 3 | Phase 4 | Mature (Phase 5+) |
|---|:---:|:---:|:---:|:---:|:---:|
| IoT Telemetry & Alerting *(existing)* | ✓ | | | | |
| Incidents, Tasks, GIS *(existing)* | ✓ | | | | |
| Weather Intelligence | ✓ | | | | |
| Crop Management | ✓ | | | | |
| Fertilizer & Nutrient Management | ✓ | | | | |
| Farm Activity Timeline | ✓ | | | | |
| AI Agronomist (rules-based) | | ✓ | | | |
| Disease Risk Intelligence | | ✓ | | | |
| Pest Monitoring | | ✓ | | | |
| Predictive Maintenance | | ✓ | | | |
| Automation Rules Engine | | ✓ | | | |
| Workflow Designer | | | ✓ | | |
| Satellite Imagery | | | ✓ | | |
| Technician Mobile Overhaul | | | ✓ | | |
| Equipment Management | | | ✓ | | |
| Inventory Management | | | ✓ | | |
| Farm Digital Twin | | | ✓ | | |
| Financial Analytics | | | | ✓ | |
| Multi-Farm Command Center | | | | ✓ | |
| Carbon & Sustainability | | | | ✓ | |
| Compliance | | | | ✓ | |
| Drone Integration | | | | ✓ | |
| Yield Prediction | | | | | ✓ |
| Benchmarking | | | | | ✓ |
| Marketplace | | | | | ✓ |
| AI Agronomist (ML-driven) — see Section 36 | | | | | ✓ |

This is a direct restatement of Section 18's phases in matrix form — kept as a separate artifact because it's the one page worth pulling out on its own for a board deck or customer conversation, without the surrounding narrative.

---

## 33. Product Maturity Matrix (Competitive Positioning)

A directional comparison against category leaders, useful for investor/customer conversations — **not a verified competitive audit** (competitor capabilities shift and vary by region/tier; this should be re-validated against current competitor documentation before external use, not asserted from this planning session alone).

| Capability | Farmru (this plan) | Climate FieldView | Trimble Ag | John Deere Ops Center | Esri ArcGIS (ag-configured) |
|---|---|---|---|---|---|
| Ground-truth soil IoT telemetry | **✓ — existing, core moat** | Partial (mostly imagery/yield-monitor-derived) | ✓ | Partial | ✗ (not a sensing platform) |
| Real-time ops (Alerts/Incidents/SignalR) | **✓ — existing, core moat** | ✗ (analytics-first, not ops-ticketing) | Partial | Partial | ✗ |
| Weather Intelligence | Planned (Phase 1) | ✓ | ✓ | ✓ | Partial (via overlays) |
| Crop Management | Planned (Phase 1) | ✓ | ✓ | ✓ | Partial |
| Satellite Imagery (NDVI/NDRE) | Planned (Phase 3) | ✓ | ✓ | ✓ | ✓ — strong |
| Drone Integration | Planned (Phase 4) | Partial | ✓ | Partial | ✓ |
| AI Agronomist (explainable recommendations) | Planned (Phase 2, phased Gen 1→5, Section 36) | Partial (yield/planting insights) | Partial | Partial | ✗ |
| Automation Rules Engine | Planned (Phase 2) | ✗ | Partial | Partial | ✗ |
| **Workflow Designer (BPM-style)** | **Planned (Phase 3) — differentiator** | ✗ | ✗ | ✗ | ✗ |
| **Farm Digital Twin (unified spatial+ops composition)** | **Planned (Phase 3) — differentiator** | ✗ | Partial | Partial | Partial (spatial only, not ops-integrated) |
| Equipment/Inventory/Marketplace | Planned (Phase 3–5) | ✗ | Partial (equipment-centric) | ✓ — strong | ✗ |
| Financial Analytics | Planned (Phase 4) | Partial | Partial | Partial | ✗ |
| Compliance/ESG Reporting | Planned (Phase 4) | Partial | Partial | Partial | Partial |
| Multi-tenant enterprise architecture | **✓ — existing** | N/A (single-org SaaS) | Partial | Partial | ✓ |

**Reading this honestly:** Farmru's real, defensible edge today is the combination of *owned ground-truth sensing* + *real-time operational workflow* — a combination none of the four listed competitors currently offer together (FieldView and ArcGIS lean analytics/imagery without ops-ticketing; Trimble and Deere lean equipment/operations without deep soil-sensor telemetry as a first-party feature). The Workflow Designer and Digital Twin are the two roadmap items most likely to remain genuinely differentiated rather than catching up to table stakes — worth protecting in prioritization if resources get squeezed, rather than the imagery/drone items, which are closer to parity features than differentiators.

---

## 34. Screens Inventory

A living catalogue, not a one-time list — new modules must add their screens here before UI work starts, using this template, to prevent the duplicate-screen risk the review flagged (e.g., two modules independently building "a device detail view").

**Template per screen:** Purpose · Primary Persona · Inputs (what data/params it needs) · Outputs (what it displays/produces) · Key Widgets (from Section 27's inventory) · Navigation (entry points, where it links onward) · Mobile Support (full/partial/web-only, with rationale).

Representative entries (illustrative sample across existing + new modules — the full catalogue should be completed as a spreadsheet/Confluence-style living artifact during Phase 1 design, not exhaustively enumerated in this narrative document):

| Screen | Purpose | Persona | Key Widgets | Mobile Support |
|---|---|---|---|---|
| Facility Detail *(existing, extend)* | Single Facility's full status | Manager, Agronomist | KPI Tile, Weather Widget, Map, Timeline Entry List | Full |
| Node Detail *(existing)* | Device telemetry + health | Technician, Agronomist | Trend Chart, Status Badge, Metric/Gauge Widget | Full (already exists) |
| Crop Season Wizard | Create/manage a planting cycle | Manager, Agronomist | Step Wizard | Full |
| Planting Calendar | Cross-Facility planting schedule | Manager, Executive | Data Table / Gantt view | Partial (view-only) |
| Recommendation Feed | AI Agronomist output | Agronomist, Manager | Recommendation/Insight Card, Filter Panel | Full |
| Disease Risk Dashboard | Field × disease-model risk grid | Agronomist | Heat Map, Status Badge | Partial (view-only) |
| Rule Builder Canvas | Author Automation Rules | Manager, Admin | Rule/Workflow Builder Canvas | Web-only (canvas editing) |
| Workflow Designer Canvas | Author multi-step workflows | Ops Lead, Admin | Rule/Workflow Builder Canvas | Web-only |
| Workflow Instance Monitor | Track in-flight processes | Ops Lead, Manager | Kanban, Status Badge | Partial (view + approve actions) |
| My Tasks/Approvals Inbox | Workflow-aware task list | All assignable roles | Approval/Task Inbox Item | Full — this is the one Workflow screen technicians actually need on mobile |
| Equipment Registry | Farm asset tracking | Manager, Technician | Data Table, Status Badge | Full |
| Inventory/Warehouse Overview | Stock levels by category | Manager | KPI Tile, Data Table | Partial |
| Command Center Home | Portfolio KPI rollup | Executive | KPI Tile, Map, Regional Summary Card | Web-only (kiosk/desktop use case) |
| Digital Twin View | Composed Facility spatial view | Agronomist, Manager | Map + Layer Selector, Time Slider | Partial (view-only, layer toggle) |
| Global Command Palette (⌘K) | Cross-entity search | All | Global Search Result Item | Full (platform-appropriate input) |
| QR/NFC Scan Screen | Scan-to-open Node/Equipment | Technician | Camera capture | Mobile-only (no web equivalent needed) |

**Governance rule:** before any new module's UI design starts, its screens must be added to this catalogue and checked against existing rows for overlap — this is a cheap five-minute step that prevents the exact duplicate-screen risk raised in review.

---

## 35. Component Catalogue

Section 27 already established the **UI Component Inventory** as a Phase 1, Sprint 1 deliverable; this section is that same catalogue confirmed complete against the specific list from review, with the few not already explicit now named directly:

Already covered in Section 27: KPI Tile, Metric/Gauge Widget, Status Badge, Trend/Line Chart Card, Map (base + layer system), Timeline Entry List, Step Wizard, Data Table, Filter Panel, Recommendation/Insight Card, Rule/Workflow Builder Canvas, Approval/Task Inbox Item, Empty State, Global Search Result Item.

**Adding explicitly, to close the list out completely:**
- **Weather Widget** — current-conditions + mini-forecast card, a specialized composition of KPI Tile + Trend Chart, reused on Facility Detail, Command Center, and the Digital Twin layer panel.
- **Alert Card / Incident Card** — distinct from the generic Recommendation Card (different action set: acknowledge/resolve/escalate vs. accept/dismiss/override), but sharing the same underlying card shell/status-badge pattern.
- **Heat Map** — a distinct rendering mode from the base Map component (Section 27), used by Disease Risk, Pest hotspots, and Benchmarking's percentile views; should be built as a layer type *within* the shared Map component rather than a separate map implementation.
- **Layer Selector** — the toggle panel controlling which overlays (Weather, Imagery, Equipment, Crop Boundaries, Incidents) are visible on any map instance; one implementation, reused by GIS, Digital Twin, and Command Center's regional map.
- **GIS Toolbar** — draw/edit tools for polygons (Fields, GeoFences) and measurement tools; scoped to map instances that support editing (not the read-only Command Center map).
- **Analytics Card** — a chart-plus-summary-stat composite (distinct from the plain Trend Chart Card by including a headline number + delta), used across Financial Analytics, Sustainability, and Benchmarking.

**Ownership rule:** one team/owner should be accountable for this catalogue's consistency (a "design system owner," even if not a dedicated role at current team size) so components don't fork into near-duplicate variants across modules — the single biggest practical risk to a component catalogue is that it's documented once and then drifts as each module team makes small "just for us" tweaks.

---

## 36. Future AI Roadmap

The AI Agronomist (5.8) is deliberately specified to start as a rules/expert-system layer, not a model — making that progression explicit as a five-generation path keeps stakeholder expectations realistic while giving a credible long-term vision:

| Generation | Approach | What changes for the user | Prerequisite |
|---|---|---|---|
| **Gen 1 — Expert Rules** *(Phase 2 MVP)* | Deterministic threshold/condition rules authored by agronomy domain experts (the `RecommendationRule` entity in 5.8) | Recommendations are correct but rigid — same input always produces the same output | None beyond Phase 1 data modules |
| **Gen 2 — Statistical Calibration** *(Phase 3–4)* | Rule outputs are calibrated against `RecommendationFeedback` history — confidence scores become empirically grounded (e.g., "this rule has been accepted 78% of the time historically") rather than hand-assigned | Confidence scores become trustworthy, not just illustrative | 1–2 seasons of `RecommendationFeedback` data (Section 5.8) |
| **Gen 3 — Machine Learning** *(Phase 5)* | Trained models (e.g., gradient-boosted or similar, appropriate to structured agronomic data) replace or augment specific rule categories where enough labeled outcome data exists (Yield Prediction, 5.20, is the natural first ML consumer) | Recommendations start capturing interactions between factors that hand-written rules can't express | Multiple seasons of structured outcome data across enough Facilities for generalization |
| **Gen 4 — LLM-Assisted Explanation** *(post-Phase 5)* | A language model is layered *on top of* Gen 2/3's structured outputs to generate richer, more conversational plain-language explanations and answer free-form agronomist questions against the platform's own data — explicitly not replacing the structured reasoning trace (Section 5.8's explainability requirement stays a hard requirement; the LLM narrates it, it doesn't decide) | Users can ask "why is Field 4 flagged?" in natural language and get a grounded answer, not just read a pre-formatted card | Gen 2/3 structured recommendation data as grounding context (mitigates hallucination risk) |
| **Gen 5 — Autonomous Farm Optimization** *(long-term vision, not currently scoped)* | The system doesn't just recommend but closes the loop end-to-end for well-understood, low-risk decisions (e.g., automatically adjusting an irrigation schedule within pre-approved bounds, no human step) — effectively the AI Agronomist "graduating" into direct Automation Rules Engine / Workflow Designer actions at high confidence | Manual monitoring shifts to exception-handling only, for the specific decision classes proven safe | Multi-season track record of Gen 3 accuracy, explicit organization opt-in per decision class, full audit trail (Section 25) |

**Why this matters as a stakeholder-facing artifact:** it prevents the common trap of a customer or investor assuming "AI Agronomist" means Gen 3–5 capability on day one, while also giving a credible, evidence-gated path to get there — each generation's prerequisite is a real, checkable condition (season count, data volume), not a date.

---

## 37. Customer Editions

Defining editions now — even though commercial packaging is a business decision outside this document's engineering scope — prevents the common mistake of discovering entitlement/licensing complexity only after every module already assumes "the tenant has everything."

| Edition | Target customer | Included capability tier (per Section 32's heat map) |
|---|---|---|
| **Community** | Small independent farms, single-Facility, price-sensitive | Existing IoT/Alerts/Incidents core + Weather + Crop Management only (MVP tier) |
| **Professional** | Mid-size commercial operations, multi-Facility | Community + full Phase 2 (AI Agronomist, Disease/Pest, Predictive Maintenance, Automation Rules) |
| **Enterprise** | Large agribusiness, regional/multi-country portfolios | Professional + Phase 3–4 (Workflow Designer, Digital Twin, Multi-Farm Command Center, Financial Analytics, Compliance, Equipment/Inventory) |
| **Government** | Ag ministries, extension programs, regional monitoring bodies | Enterprise capability set, reconfigured around Compliance/Sustainability/Benchmarking reporting as the primary use case rather than single-operator profitability; likely needs the cross-tenant/regional aggregation exception flagged in Section 15 as an explicit, governed feature for this edition specifically |
| **Research Institution** | Universities, ag-tech research programs | A distinct data-access-oriented tier: raw telemetry/imagery export emphasis (Weather, Satellite, Crop, Yield Prediction's underlying data), de-emphasizing the operational modules (Incidents/Workflow/Marketplace) research users don't need |

**Entitlement architecture implication:** this means every module's permission design (Section 16) needs a second axis beyond role — an **edition/entitlement gate** — checked at the same `Pages.*` permission layer, so a Community-tier tenant simply never sees Workflow Designer in navigation rather than seeing it disabled. Worth resolving as an explicit ADR (extending Section 21's log) before Phase 2, since retrofitting entitlement gating onto modules already built without it is meaningfully more work than designing it in from the start.

---

## 38. What Comes Next: Execution Assets

This document has now matured from a feature list into a governance-complete product blueprint (Sections 1–20 product/UX, 21–30 enterprise governance, 31–37 dependency visualization, competitive positioning, and packaging). Deliberately, no further functional scope is being added here — the next deliverables belong to a different, implementation-facing document type and are listed here only as a pointer, not attempted in this planning artifact:

- **UI mockups / high-fidelity design system application** (applying Section 14/28's tokens and Section 27/35's component catalogue to real screens from Section 34's inventory)
- **Domain model diagrams** (formal ERDs per bounded context from Section 22, not just the entity lists in Section 5/8)
- **Architecture diagrams** (deployment/infrastructure view — how ADR-005/006's storage decisions, the existing ABP layers, and new background jobs actually sit together)
- **Database ERDs** (schema-level, derived from Section 5's per-module entity additions)
- **API contracts** (OpenAPI/Swagger specs per new AppService, extending the existing Swashbuckle setup already in Web.Host)
- **Detailed implementation roadmap** (sprint-level breakdown of Phase 1, informed by Section 21's ADRs)
- **Release planning** (what ships in each release within a phase, tied to Section 26's performance targets as exit criteria)
- **Acceptance criteria** (per user story from Section 5, testable Given/When/Then specs)

This is the natural point to pause planning and move to execution — the recommended next single deliverable remains the **Phase 1 technical design** (Weather, Crop Management, Fertilizer & Nutrient Management), now informed by ADR-001/004/005, the Bounded Context Map, and the Screens/Component catalogues above.

---

*End of planning document. Next step, if this direction is approved: a Phase 1 detailed technical design (entity schemas, AppService contracts, Angular module scaffolding, Flutter screen specs) — a separate, implementation-facing document, not covered here by design.*
