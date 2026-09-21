# Architecture

> **Document role:** Target architecture
>
> **Status:** Living document
>
> **Source of truth for:** System boundaries, logical components, deployment model, architectural principles, modular-monolith direction, capture vs. reporting separation, and browser/client vs. server responsibilities.
>
> **Does not describe:** Detailed synchronization rules (see [OFFLINE_SYNC.md](OFFLINE_SYNC.md)), reporting rules (see [REPORTING.md](REPORTING.md)), domain rules (see [DOMAIN_MODEL.md](DOMAIN_MODEL.md) and [DOMAIN_RULES.md](DOMAIN_RULES.md)), or recovery objectives (see [OPERATIONS_AND_RECOVERY.md](OPERATIONS_AND_RECOVERY.md)).

## Hosting Model

SomnosSuite is intended to operate entirely on-premises.

The organization's server infrastructure is hosted on a virtualization platform within the company's own building.

Cloud infrastructure is not currently required for the core system.

## Network Environment

Capture stations, office workstations, server infrastructure, and other operational systems are separated through network segments.

SomnosSuite must therefore not assume that all clients and services reside within the same local network segment.

Required communication paths between network segments must be explicitly defined and restricted.

## Logical Components

The active backend is a layered, modular-monolith solution:

- `SomnosSuite.Domain` — business invariants, lifecycle rules, value objects, aggregate behavior, and domain error definitions.
- `SomnosSuite.Application` — use cases orchestrating domain behavior.
- `SomnosSuite.Infrastructure` — cross-cutting technical concerns such as system-clock access.
- `SomnosSuite.Persistence` — data access and rehydration of persisted state.
- `SomnosSuite.Presentation` — request/response mapping.
- `SomnosSuite.WebApi` — the hosted API surface.

The domain layer does not own persistence, transport DTOs, API validation, import mapping, authorization, report export, mail delivery, or system-clock access. Those concerns are handled by the surrounding layers.

The direction is a modular monolith: capture, reporting, and integration concerns are logically separated within the same deployable backend, rather than assuming a distributed microservice architecture from the start. A component may be split into a separately deployable service later if independent availability or scaling requirements justify it.

## Capture vs. Reporting Separation

Operational capture (recording a stunning control at the line) and reporting/evaluation (recurring reports, dashboards) are distinct concerns:

- capture must remain fast, available, and resilient to temporary connectivity loss (see [Capture Workflow](CAPTURE_WORKFLOW.md) and [Offline Capture and Synchronization](OFFLINE_SYNC.md));
- reporting operates on already-persisted, centrally available data and does not need to meet the same latency or offline constraints (see [Reporting](REPORTING.md)).

The architecture must not couple the availability of reporting to the availability of capture, or vice versa.

## Browser Client vs. Server Responsibilities

The preferred SomnosSuite capture-station client architecture is browser-based, so that capture stations do not require installation and maintenance of a dedicated desktop application.

A standard web browser cannot directly access network file shares or receive operating-system file-change notifications the way a legacy desktop application can. Consequently, external upstream integrations (see [Integrations](INTEGRATIONS.md)) must be handled server-side, not from browser JavaScript.

Durable local capture storage and synchronization logic run in the browser client; see [Offline Capture and Synchronization](OFFLINE_SYNC.md) for the detailed model.

## Capture Stations

The slaughter lines currently use industrial fanless touch PCs running a fixed desktop operating system.

Capture stations are statically associated with a slaughter line. There is normally exactly one capture PC per slaughter line. Station assignment is configured once and is not normally selected by the operator; the configuration must survive browser and application restarts.

Multiple browser windows on the same physical station are not inherently prohibited, but multiple physical computers must not operate simultaneously under the same SomnosSuite station identity. See [Offline Capture and Synchronization](OFFLINE_SYNC.md) for the station-identity model.

## Workstation Scale

SomnosSuite is expected to support approximately 10–20 operational and administrative workstations.

The expected scale does not currently require distributed or high-volume cloud infrastructure.

## Capture Volume

The pig line processes approximately 1,200–1,600 animals per operating day.

Large-cattle and calf volumes are substantially lower. The exact expected daily volume for large cattle and calves must still be confirmed.

## Capture Interaction Pattern

Capture activity is burst-oriented rather than evenly distributed.

Operators may inspect multiple animals and subsequently record the results for a group of approximately ten animals in rapid succession.

The capture interface must therefore support very fast repetitive interaction. A normal successful capture should require as few interactions as reasonably possible. Negative results may require additional input for failure indicators and corrective measures.

## Local Reliability

Capture stations are not protected by an uninterruptible power supply (UPS). Servers and network switches are protected by UPS infrastructure.

SomnosSuite must therefore persist a completed local capture immediately to durable storage before presenting it as safely recorded. Captured data must not depend solely on application memory. Detailed durability and synchronization behavior is defined in [Offline Capture and Synchronization](OFFLINE_SYNC.md).

## Synchronization Visibility

Capture stations must expose synchronization state to the operator. Detailed synchronization states and behavior are defined in [Offline Capture and Synchronization](OFFLINE_SYNC.md).

Synchronization failures must be visible to operational users, responsible managers, and IT. The exact notification and escalation mechanism is defined in [Operations and Recovery](OPERATIONS_AND_RECOVERY.md).
