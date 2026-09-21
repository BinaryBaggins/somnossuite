# Capture Workflow

> **Document role:** Target operational workflow
>
> **Status:** Living document
>
> **Source of truth for:** What operators actually do — pig capture, large-cattle/calf capture, production-day lifecycle, station interaction, device-selection workflow, and operational corrections.
>
> **Does not describe:** Domain entities and target business rules (see [DOMAIN_MODEL.md](DOMAIN_MODEL.md)), currently implemented rules (see [DOMAIN_RULES.md](DOMAIN_RULES.md)), or synchronization mechanics (see [OFFLINE_SYNC.md](OFFLINE_SYNC.md)).

## General Principle

Every slaughtered animal receives an individual stunning-control record.

The system therefore operates on individual animal-level records rather than aggregate counts or samples. A stunning-control record represents the observed stunning condition of one animal together with the relevant animal, device, timing, corrective-action, and audit information. See [Domain Model](DOMAIN_MODEL.md) for the underlying concepts.

## Operational Day

The operational day is started manually. Production usually begins in the early morning, approximately between 03:00 and 05:00.

There is currently no multi-shift model that must be represented in SomnosSuite. A stunning-control record therefore does not require a separate shift identifier. Date and timestamp, operator, and station are sufficient for operational attribution.

## Slaughter-Line Sequence

There are two production lines: the pig line, operated independently, and the large-cattle/calf line.

Large cattle and calves may be processed in either order during the same operating day on the large-cattle/calf line. SomnosSuite must not assume that one animal category permanently occupies a line for the whole day.

## Capture Modes

SomnosSuite must support two different capture modes.

### Pig Capture

Pig stunning controls are entered manually at the slaughter line.

The operator observes the animal and records the stunning result directly at the capture station. The capture process does not depend on an externally imported animal record. Pig capture is high-volume and requires a fast, repetitive UX (see [Architecture](ARCHITECTURE.md) for volume and interaction-pattern expectations).

### Large Cattle and Calf Capture

Large cattle and calf data is imported automatically from the ERP integration (see [Integrations](INTEGRATIONS.md)).

The animal is identified primarily by its ear tag number. The operator then confirms the stunning-control result for the imported animal.

## Stunning Result

For each animal, the operator records whether stunning was successful.

If stunning was not satisfactory, the operator records the observed failure indicator(s) — such as reflexes, vocalization, or breathing activity — as defined in [Domain Model](DOMAIN_MODEL.md).

## Corrective Action

When stunning is insufficient, a corrective stunning action is performed using a captive-bolt device.

The operator records whether the corrective stunning occurred before or after bleeding, and which device was used. The exact physical time of corrective stunning cannot currently be measured automatically and depends on correct operator input.

## Device-Selection Workflow

SomnosSuite may assume that the previously selected stunning device remains correct for the next animal; the operator is not required to reselect or reconfirm the device for every animal.

However, the device stored on each completed stunning-control record must represent the device actually used. The currently selected device must be clearly visible in the interface, and changing it must be fast and must not unnecessarily interrupt production. The interaction design must consider the risk that an operator forgets to change the selected device after a physical device switch (for example, after equipment failure).

Replacement devices must already exist as known devices in SomnosSuite before they can be selected.

A separate audit record for changing the currently selected device is not currently required. Historical correctness is achieved by storing the device actually used on each individual stunning-control record.

## Operational Corrections

The operator may correct an erroneous entry during the immediate operational capture process.

The capture workflow must remain fast and must not introduce unnecessary interaction during time-critical line operation. A correction reason is not required for this immediate operational correction.

Corrections performed later, outside the immediate operational context, are considered historical corrections and are more sensitive because the original circumstances are harder to reconstruct. See [Security and Access](SECURITY_AND_ACCESS.md) and [Requirements](REQUIREMENTS.md) (SEC-005) for the required correction-attribution and history rules, and for the open question of whether historical corrections require an additional reason or authorization step.

## Station Assignment

Each capture station is assigned to exactly one slaughter line. Capture operations from the two slaughter lines do not normally overlap, since pig slaughter and large-cattle/calf slaughter are not normally performed simultaneously.

## Daily Closing

The daily closing process in the legacy applications is a technical mechanism rather than a business requirement: it exists primarily to transfer locally collected data to the central network location, and captured data can otherwise remain local for an entire day or longer.

SomnosSuite should not require a manual daily closing process. Synchronization should occur continuously whenever connectivity is available; see [Offline Capture and Synchronization](OFFLINE_SYNC.md) for the target synchronization model.

A station must clearly indicate whether unsynchronized records remain locally stored, and must not silently allow shutdown or completion of the operating day while unsynchronized data remains without warning the user. The exact shutdown behavior remains to be defined.
