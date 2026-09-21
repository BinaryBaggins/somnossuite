# Project Overview

> **Document role:** Project overview
>
> **Status:** Living document
>
> **Source of truth for:** Why SomnosSuite exists, business background, product goals, user groups, and high-level operational context.
>
> **Does not describe:** Technical implementation details. See [ARCHITECTURE.md](ARCHITECTURE.md) for architecture and [REQUIREMENTS.md](REQUIREMENTS.md) for detailed requirements.

## Background

SomnosSuite originates from several independently developed applications used to document livestock stunning controls.

Approximately two years before the start of SomnosSuite, the first application was developed to support the documentation of stunning controls for large cattle and calves.

An existing application for pig stunning documentation was subsequently replaced by a newly developed application.

A third application was later introduced to generate recurring evaluations and reports for responsible personnel.

This resulted in three separate applications that covered related business processes but required independent maintenance, administration, deployment, and support.

SomnosSuite was initiated to consolidate these workflows into one maintainable system.

## Business Problem

The previous solution consists of multiple independent applications for:

- stunning-control data capture for large cattle and calves;
- stunning-control data capture for pigs;
- recurring evaluations and reporting.

Operating and maintaining several independent applications creates unnecessary technical and administrative overhead.

The applications also make it harder to provide a consistent user experience, shared user management, common reporting, and centralized data access.

## Product Goal

SomnosSuite shall provide a single application platform for livestock stunning-control documentation and evaluation.

The system shall support:

- operational data capture;
- documentation and traceability;
- automated reporting;
- analysis and evaluation;
- multiple operational locations and workstations;
- role-based access to responsibilities and functionality.

The long-term goal is that users, operational managers, quality personnel, and other responsible roles can independently perform and manage the tasks relevant to them.

## Primary Users

SomnosSuite is expected to support the following user groups:

- Data capture operators
- Shift and department managers
- Quality assurance personnel
- Management
- Auditors
- IT administrators

The exact permissions and responsibilities of these roles are defined in [Security and Access](SECURITY_AND_ACCESS.md).

## Operational Context

The current operational environment contains two slaughter lines: one for large cattle and calves, and a dedicated pig line.

Each slaughter line has a capture station used for data entry, in addition to desktop workstations used by managers and control personnel for administration, review, and evaluation.

Detailed environment, scale, and deployment information is defined in [Architecture](ARCHITECTURE.md).

## Core Capture Workflow

The primary operational workflow is the documentation of a stunning control: for each inspected animal, the operator records whether stunning was successful, and, if not, the observed failure indicators and the corrective measure taken.

The detailed operational workflow is defined in [Capture Workflow](CAPTURE_WORKFLOW.md).

## Reporting

SomnosSuite shall provide reporting and evaluation capabilities, including at least weekly and monthly reports, with a reporting architecture that remains flexible for additional future analyses.

Detailed reporting requirements are defined in [Reporting](REPORTING.md).

## Reliability Expectations

Two operational failures are considered unacceptable: silent loss of captured data, and loss of capture capability during a temporary network failure.

Detailed reliability requirements are defined in [Requirements](REQUIREMENTS.md), [Offline Capture and Synchronization](OFFLINE_SYNC.md), and [Operations and Recovery](OPERATIONS_AND_RECOVERY.md).

## Auditability

Stunning-control records must remain available for audits, with sufficient traceability to reconstruct relevant historical records.

The detailed legal, regulatory, and retention requirements are currently not known and must be investigated separately. Until those requirements are known, SomnosSuite should avoid architectural decisions that would make later audit or retention requirements difficult to implement.

Detailed audit and access requirements are defined in [Security and Access](SECURITY_AND_ACCESS.md).

## Existing Systems

The current business processes are implemented using internally developed legacy applications based on Java Swing and Java console applications.

These systems represent an important source for understanding the existing workflows, terminology, reports, and operational expectations. They should be treated as reference implementations, but not automatically as the specification for the new system.

Migration from these legacy applications is defined in [Migration and Rollout](MIGRATION_AND_ROLLOUT.md).
