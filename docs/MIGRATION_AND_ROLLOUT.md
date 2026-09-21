# Migration and Rollout

> **Document role:** Target migration and rollout plan
>
> **Status:** Living document
>
> **Source of truth for:** One-time legacy JSON migration, reconciliation, validation, parallel operation, cutover, rollback, and decommissioning legacy applications.

## Purpose

SomnosSuite will replace the existing legacy applications used for stunning-control capture and reporting.

Historical stunning-control records must remain available after the migration.

The migration is intended to be a one-time transition from the legacy applications to SomnosSuite.

---

## Historical Data

Historical stunning-control records currently exist as JSON files.

The available history dates from approximately April 2023 to the present.

More than 1,000 JSON files currently exist.

The exact number of records contained in these files must be determined during migration preparation.

## Migration Scope

Historical business records must be imported into SomnosSuite.

Imported records must be available through the same search and reporting capabilities as newly captured SomnosSuite records where the historical source data supports those capabilities.

Previously generated PDF reports do not need to be migrated.

## Legacy Source Quality

The structure and meaning of the existing historical data are considered known.

The migration process must nevertheless validate source records before importing them.

Malformed, incomplete, duplicate, or inconsistent records must not be silently accepted.

Migration anomalies must be reported so that they can be reviewed before go-live.

---

## Migration Strategy

### One-Time Import

Legacy data migration is intended to be performed once as part of the SomnosSuite rollout.

SomnosSuite does not require a permanent legacy-import feature.

The migration tooling may remain in the repository for traceability and recovery purposes, but it is not part of the normal production workflow.

### Repeatability

The migration process should be repeatable during testing and rollout preparation.

Repeated execution must not silently create duplicate historical records.

The migration tooling should therefore provide deterministic duplicate detection or another idempotent import mechanism.

### Source Preservation

Original legacy JSON files must remain unchanged during migration.

Migration tooling must treat the legacy files as read-only source data.

This allows the migration to be repeated and independently verified.

---

## Validation

### Primary Acceptance Criterion

The primary migration and go-live acceptance criterion is data correctness.

SomnosSuite must reproduce the relevant historical business data accurately.

### Reconciliation

Migration validation should include at least:

- total source files processed;
- total source records discovered;
- total records successfully imported;
- total records rejected;
- total duplicate records detected;
- counts grouped by relevant animal category;
- counts of successful and unsuccessful stunning controls;
- counts of corrective stunning actions where available.

Where possible, the migrated data should be compared with known legacy totals and reports.

Any discrepancy must be understood before production cutover.

### Sampling

In addition to automated reconciliation, representative historical records should be manually compared between the legacy data and SomnosSuite.

Samples should include:

- successful controls;
- unsuccessful controls;
- different failure indicators;
- corrective stunning actions;
- different animal categories;
- different stunning devices;
- records from different dates.

---

## Rollout

### Target State

SomnosSuite is intended to fully replace the existing legacy applications.

The old applications are not intended to remain permanently available after successful rollout.

### Parallel Operation

Temporary parallel operation of SomnosSuite and the legacy applications is acceptable during rollout.

Parallel operation should be limited to the period required to verify that:

- capture behaves correctly;
- persisted data is correct;
- synchronization works reliably;
- reports contain the expected information;
- users can complete their operational workflows.

### Production Cutover

Production cutover should occur outside active slaughter operations.

Before cutover:

1. the required historical migration must be completed or validated;
2. SomnosSuite must be deployed and health-checked;
3. capture stations must be provisioned;
4. required device and user master data must be available;
5. external integrations must be verified;
6. reporting configuration must be validated;
7. rollback procedures must be prepared.

### Rollback

A short-term rollback to the legacy applications must remain possible during the initial production rollout.

The rollback procedure must be defined before go-live.

The procedure must account for records that may have been created in SomnosSuite after cutover so that business data is not silently lost or duplicated.

The exact reconciliation procedure for a rollback remains to be designed.

### Legacy Decommissioning

Once SomnosSuite has been validated in production and rollback is no longer required:

- legacy capture applications may be retired;
- legacy reporting applications may be retired;
- historical records remain accessible through SomnosSuite;
- original migration source files should be retained according to the agreed archival policy.

The legacy applications themselves do not need to remain available for historical lookup after successful migration.

---

## Migration Architecture

The preferred migration flow is:

```text
Legacy JSON files
        |
        v
Migration reader
        |
        v
Parsing / mapping
        |
        v
Validation
        |
        +---- invalid ---> Migration error report
        |
        v
SomnosSuite application/domain mapping
        |
        v
SQL Server
        |
        v
Reconciliation report
```

Migration code should reuse SomnosSuite business validation where practical instead of bypassing domain constraints.

Where historical data cannot satisfy newer domain requirements, explicit migration rules must be defined rather than inventing missing values.

---

## Open Decisions

| Topic                                   | Status                                        |
| --------------------------------------- | --------------------------------------------- |
| Historical migration required           | Yes                                           |
| Historical source format                | JSON                                          |
| Historical period                       | Approximately April 2023 to present           |
| Historical files                        | More than 1,000                               |
| Historical records searchable           | Required                                      |
| Old PDF reports migrated                | No                                            |
| Migration type                          | One-time                                      |
| Parallel operation                      | Acceptable                                    |
| Legacy replacement                      | Required                                      |
| Short-term rollback                     | Required                                      |
| Legacy applications after stabilization | Not required                                  |
| Primary acceptance criterion            | Data correctness                              |
| Exact migration mapping                 | To be documented after inspecting source JSON |
| Rollback reconciliation                 | To be designed                                |
