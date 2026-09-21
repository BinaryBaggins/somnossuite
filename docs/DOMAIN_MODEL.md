# Domain Model

> **Document role:** Target business/domain model
>
> **Status:** Living document
>
> **Source of truth for:** Target domain concepts, terminology, entities/aggregates/value concepts, and relationships.
>
> **Does not describe:** Current implementation. See [DOMAIN_RULES.md](DOMAIN_RULES.md) for business rules currently enforced by the code, and [DOMAIN_ROADMAP.md](DOMAIN_ROADMAP.md) for known gaps between this target model and the current implementation.

## Animal Classification

`Animal kind`, `animal category`, and `slaughter line` are three distinct concepts and must not be derived from one another.

### Animal Kinds

At least:

- Pig
- Calf
- Large cattle / relevant large-cattle kinds

### Animal Categories

There are two operational categories:

| Category | Meaning         |
| -------- | --------------- |
| `KV`     | small livestock |
| `GV`     | large livestock |

Both pigs and calves belong to `KV`. Large cattle belongs to `GV`.

### Slaughter Lines

Slaughter-line assignment is independent from animal category. Current operation:

| Animal kind  | Slaughter line         |
| ------------ | ---------------------- |
| Pig          | Pig line               |
| Calf         | Large-cattle/calf line |
| Large cattle | Large-cattle/calf line |

For example, a calf has `AnimalKind = Calf`, `AnimalCategory = KV`, and is processed on the large-cattle/calf line. Animal category must never be derived from slaughter line, and slaughter line must never be derived from animal category.

## Animal Identity

### Large Cattle and Calves

Large cattle and calves are externally identifiable by their ear tag number. The ear tag is considered globally unique and immutable for the lifetime of the animal.

SomnosSuite nevertheless assigns its own internal `AnimalId`. The external ear tag is therefore treated as a business identifier rather than as the database primary key. This allows SomnosSuite to remain independent from external identifier formats, reference animals consistently internally, attach imported metadata and audit information, and evolve without coupling internal persistence to external identifiers.

### Pigs

Pigs currently have no reliable individual external identifier. Individual animals cannot be reconstructed with certainty after processing.

At most, approximate relationships may be inferred from supplier order and delivered quantities, but this is not guaranteed to reflect the physical animal sequence. SomnosSuite must therefore not claim individual traceability for pigs where the source process does not provide it.

Each captured pig record may still receive an internal technical identifier, but that identifier does not represent a physically verifiable external animal identity. For pig operations, the primary correctness requirements are the total number of controlled animals, the total number of negative findings, and correct statistical aggregation of findings and corrective actions.

## Animal Attributes

Depending on the animal kind and available source data, a stunning-control record may reference:

- the internal SomnosSuite `AnimalId`;
- animal kind (see Animal Classification);
- animal category (`KV`/`GV`, see Animal Classification);
- an operational or commercial label (for example a category or breed label used internally);
- the external ear tag number, for large cattle and calves;
- supplier;
- origin;
- slaughter date;
- control timestamp;
- the original stunning method/device (see Original Stunning Device).

Not every field is available for every capture mode. In particular, pigs do not have a reliable external ear tag, supplier, or origin reference in the same sense as large cattle and calves (see Animal Identity above); SomnosSuite must not claim these attributes exist for pigs where the source process does not provide them.

### Timing Precision

The control itself is system-timestamped with high precision.

The slaughter date is currently known only at day-level precision.

For large cattle and calves, more precise slaughter-time information may be available from the ERP integration, but this has not yet been confirmed and must not be assumed until verified (see [Integrations](INTEGRATIONS.md)). The exact set of fields available through the ERP integration is integration-dependent and must be verified per source.

## Stunning Control

Each animal receives exactly one stunning-control record, representing the documented result of the control performed for that animal.

Later findings or corrections do not create a second independent stunning-control record. They modify or correct the existing business record while preserving the required audit history (see [Requirements](REQUIREMENTS.md), SEC-005).

## Stunning Outcome and Failure Indicators

A stunning control has one overall outcome: successful or unsuccessful.

An unsuccessful outcome may contain **multiple simultaneously observed failure indicators**. Known failure indicators currently include:

- eye or other reflexes;
- vocalization;
- breathing activity.

The target model distinguishes between the overall stunning outcome and one or more observed failure indicators. The current implementation supports only a single failure indicator per stunning control; see [DOMAIN_ROADMAP.md](DOMAIN_ROADMAP.md).

## Corrective Stunning

An unsuccessful stunning control may require **multiple corrective stunning actions**.

A corrective stunning action includes at least:

- the stunning device used;
- whether it occurred before or after bleeding.

The exact physical time of a corrective stunning action cannot currently be measured reliably and depends on operator input. The target model distinguishes between accurately system-generated timestamps and operator-reported timing information.

The current implementation supports only a single corrective device/timing combination per stunning control; see [DOMAIN_ROADMAP.md](DOMAIN_ROADMAP.md).

## Original Stunning Device

The device used for the original stunning must be stored explicitly for every animal.

SomnosSuite must not rely solely on the currently configured device of the station, because the active device may change during operation (for example, due to equipment failure). The device associated with a completed stunning-control record must remain historically stable even if the station configuration changes later. See [Capture Workflow](CAPTURE_WORKFLOW.md) for the operator-facing device-selection interaction.

Conceptually:

- the currently selected device is convenience/UI state;
- the device stored on a stunning control is a historical business fact.

## Stunning Device Applicability

Stunning-device applicability is independent from slaughter line.

Stunning devices may have different approvals or permitted uses. A device approved for `KV` may be applicable to both pigs and calves, even though pigs and calves are processed on different slaughter lines.

Current device types:

- captive bolt;
- carbon dioxide (CO₂).

A device may support more than one animal category. The current implementation models only a single animal category per device; see [DOMAIN_ROADMAP.md](DOMAIN_ROADMAP.md). Whether future approval must distinguish individual animal kinds within the same category (rather than only category-level approval) remains open.

## Stunning Device Lifecycle

Stunning devices require their own managed lifecycle. Relevant information may include manufacturer, serial number, model, stunning type, permitted animal categories, inspection history, maintenance history, operational status, and retirement or deactivation.

Historical stunning records must continue to reference the device that was actually used, even if that device is later deactivated or retired. Changes to device master data must not invalidate historical stunning-control records.

### Inspection History

The target model is to preserve a device's full inspection history rather than only the most recent inspection date. An inspection record may eventually contain information such as inspection date, inspection type, performed by, result, notes, next due date, and supporting document; the exact required fields are still open.

Required inspection intervals are currently unknown; the architecture should allow inspection schedules or due dates to be introduced later rather than hard-coding a fixed interval.

SomnosSuite should be able to warn responsible users when a device inspection is overdue. Whether use of an overdue device must be technically blocked remains open.

### Operational Status

An inactive or out-of-service device must remain visible in historical records and reports, but must not be selectable for new stunning controls. This preserves audit correctness without allowing retired equipment to be used operationally.

## Known Domain Model Gaps

See [DOMAIN_ROADMAP.md](DOMAIN_ROADMAP.md) for the authoritative, up-to-date list of differences between this target model and the current implementation.
