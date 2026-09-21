# SomnosSuite Requirements

> **Document role:** Confirmed requirements
>
> **Status:** Living document
>
> **Source of truth for:** Functional, non-functional, security, and compliance requirements.
>
> **Does not describe:** Implementation details, target domain concepts (see [DOMAIN_MODEL.md](DOMAIN_MODEL.md)), or currently implemented rules (see [DOMAIN_RULES.md](DOMAIN_RULES.md)).

## Requirement Classification

Requirements in this document are classified as:

- **Required** — necessary for the system to fulfil its operational purpose.
- **Desired** — expected to provide significant value but not required for the initial release.
- **Open** — requirement exists, but details are not yet sufficiently understood.

## Functional Requirements

### FR-001 Unified Stunning-Control System

classification: **Required**

SomnosSuite shall support stunning-control documentation for all currently supported livestock workflows within one system.

### FR-002 Stunning-Control Capture

classification: **Required**

Operators shall be able to record the result of a stunning control.

The captured information shall include whether stunning was satisfactory and, when applicable, observed failure indicators and the corrective measure taken.

See [Capture Workflow](CAPTURE_WORKFLOW.md) and [Domain Model](DOMAIN_MODEL.md) for details.

### FR-003 Multi-Station Operation

classification: **Required**

Multiple capture and administrative stations shall be able to use SomnosSuite concurrently.

### FR-004 User and Responsibility Management

classification: **Required**

Users with appropriate permissions shall be able to manage tasks and responsibilities relevant to their operational role.

Detailed authorization rules are defined in [Security and Access](SECURITY_AND_ACCESS.md).

### FR-005 Weekly Reporting

classification: **Required**

The system shall support generation of weekly reports.

### FR-006 Monthly Reporting

classification: **Required**

The system shall support generation of monthly reports.

### FR-007 Flexible Analysis

classification: **Desired**

The system should support additional analyses without requiring redesign of the core capture workflow.

Potential analyses include failure rates, device statistics, animal-category statistics, and trends.

### FR-008 Audit Access

classification: **Required**

Historical stunning-control records shall be available for audit and review.

## Reliability Requirements

### NFR-001 No Silent Data Loss

classification: **Required**

A completed capture operation must not be silently lost.

### NFR-002 Offline Capture

classification: **Required**

Capture stations shall remain capable of recording stunning controls during temporary loss of connectivity to the central server.

See [Offline Capture and Synchronization](OFFLINE_SYNC.md).

### NFR-003 Synchronization

classification: **Required**

Data recorded while offline shall be synchronized with the central system after connectivity is restored.

The synchronization mechanism shall prevent duplicate processing of the same capture operation.

### NFR-004 Concurrent Operation

classification: **Required**

The system shall support simultaneous use by multiple capture and administrative stations.

### NFR-005 Maintainability

classification: **Required**

The system shall consolidate previously separate applications into a maintainable platform with shared infrastructure and common business logic.

### NFR-006 Recovery Objectives

classification: **Required**

The system shall meet the recovery point and recovery time objectives defined in [Operations and Recovery](OPERATIONS_AND_RECOVERY.md).

This document does not define its own numeric recovery objectives.

## Security and Audit Requirements

### SEC-001 Authentication

classification: **Required**

Operational and administrative users shall be authenticated before accessing protected system functionality.

See [Security and Access](SECURITY_AND_ACCESS.md).

### SEC-002 Authorization

classification: **Required**

System functionality shall be restricted according to the user's responsibilities and permissions.

### SEC-003 Change Attribution

classification: **Required**

Where business-relevant data is created or modified, the system shall retain sufficient information to determine who performed the action.

### SEC-004 Station Attribution

classification: **Required**

Every operational capture must be attributable to the SomnosSuite capture station from which it originated.

Station identity is separate from user identity.

Each physical capture workstation must have a stable SomnosSuite `StationId`.

The detailed station enrollment/session mechanism is defined outside this document and remains an architectural implementation concern; see [Offline Capture and Synchronization](OFFLINE_SYNC.md).

### SEC-005 Correction History

classification: **Required**

Where a stunning-control record is corrected, the system shall retain:

- the value before the correction;
- the value after the correction;
- the identity of the user who performed the correction;
- the timestamp of the correction.

Immediate operational corrections performed during the active capture process do not require a stated reason.

Whether later, historical corrections require an authorization step or a mandatory reason remains open; see [Security and Access](SECURITY_AND_ACCESS.md).

## Compliance Requirements

### COMP-001 Audit Availability

classification: **Required**

Records required for operational or external audits shall remain retrievable.

### COMP-002 Historical Data Retention

classification: **Required**

Historical stunning-control records must remain available indefinitely by default.

SomnosSuite must not automatically delete historical business records unless an explicitly configured retention policy permits it.

### COMP-003 Regulatory Retention Policy

classification: **Open**

The applicable legal or regulatory minimum retention periods are currently unknown and must be investigated.

Future retention policies may allow authorized responsible personnel to configure retention behavior where legally and operationally permitted.

### COMP-004 Historical Correction Authorization

classification: **Open**

Correction attribution and before/after history are required (see SEC-005). What remains open is whether historical corrections (performed outside the immediate operational context) require additional authorization or a mandatory reason.
