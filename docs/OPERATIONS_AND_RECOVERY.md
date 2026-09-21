# Operations and Recovery

> **Document role:** Target operations and recovery model
>
> **Status:** Living document
>
> **Source of truth for:** RPO, RTO, backup, restore, monitoring, alerting, maintenance, deployment windows, disaster recovery, and operational ownership.
>
> No other document may define its own numeric recovery objective. Other documents must link here instead.

## Purpose

SomnosSuite is part of an operational production environment. The system must therefore be designed not only for correct functionality, but also for predictable failure handling, recovery, monitoring, and maintenance.

## Operational Ownership

The underlying infrastructure is operated jointly by internal IT and external IT service providers. SomnosSuite application operation should provide clear operational interfaces and documentation usable by both parties, keeping responsibilities between application operation and infrastructure operation distinguishable.

## Hosting Environment

SomnosSuite is intended to run on-premises, on a virtualization platform. The server infrastructure and relevant network equipment are protected by UPS infrastructure. Capture stations themselves are not currently protected by UPS.

## Availability Objective

Central SomnosSuite outages should be kept as short as reasonably possible. The organization is developing a VM-based disaster-recovery strategy, and SomnosSuite should integrate with this infrastructure rather than implementing an independent infrastructure-level disaster-recovery system. Application design must nevertheless support reliable restart and recovery after infrastructure failure.

The exact Recovery Time Objective (RTO) has not yet been defined / quantified. Current business expectation:

> Restore central SomnosSuite functionality as quickly as operationally possible.

A measurable RTO should be agreed with internal and external IT as part of disaster-recovery planning.

## Recovery Point Objective

For business data that has already been successfully synchronized to the central SomnosSuite environment:

```text
Target RPO: <= 24 hours
```

In the event of a major central server or storage failure, the organization accepts that up to approximately one day of centrally stored SomnosSuite data may need to be reconstructed or may be lost. The current VM-level backup strategy is intended to provide this level of protection.

The RPO is considered fulfilled only if backups are created successfully at the required interval, backups are stored outside the affected virtual machine, backups can be restored successfully, and recovery procedures are periodically tested.

Locally captured but not yet synchronized station data is outside the central RPO and is covered separately by [Offline Capture and Synchronization](OFFLINE_SYNC.md).

### Current Backup State

The current environment uses regular VM-level backups. There is currently no separate database-level backup strategy. VM-level backups alone do not establish an application-level zero-data-loss guarantee or guaranteed point-in-time recovery.

VM-level backups are not currently required to be supplemented by database-specific backups to meet the defined RPO; a dedicated database backup and recovery strategy (for example full, differential, and transaction-log backups, point-in-time restore, or replication/high-availability mechanisms) may still be introduced if a lower RPO is adopted in the future. The final design must be coordinated with the organization's disaster-recovery strategy.

Restore testing is required regardless of the specific backup mechanism used.

## Central and Local Data Are Different Failure Domains

SomnosSuite must distinguish between data already persisted on the central server, and data captured locally but not yet synchronized.

Server backup and disaster recovery protect only the first category; unsynchronized station data requires separate protection (see [Offline Capture and Synchronization](OFFLINE_SYNC.md)).

Example:

```text
Capture completed on station
        ↓
stored locally
        ↓
not yet synchronized
        ↓
station hardware fails
```

A central database backup cannot recover this record because the server never received it. The required handling of this exceptional case is still an open business and architecture decision.

## Local Capture Durability

Completed local captures must be persisted durably before the user interface reports them as safely recorded. A completed capture must survive ordinary application or browser restart. SomnosSuite must not rely on volatile process memory for unsynchronized business data. After restart, locally pending data must be discovered automatically and synchronization must resume; see [Offline Capture and Synchronization](OFFLINE_SYNC.md).

## Capture Station Replacement

A failed capture workstation must be replaceable without requiring reconstruction of the entire system. An authorized administrator should be able to provision a replacement workstation, register it as the appropriate SomnosSuite station, release or replace the previous station registration, and resume operational use. Replacement of physical hardware must not require changes to historical capture records.

## Unsynchronized Data on Failed Stations

Loss of a physical station while unsynchronized records remain locally stored is considered an exceptional failure scenario. The desired handling has not yet been defined.

Possible future mitigations may include redundant local storage, more frequent synchronization, secondary network storage, station-side backup, a dedicated local capture service, or recovery tooling for local storage. No specific solution is currently mandated. The system architecture must nevertheless keep this failure mode visible rather than assuming it cannot occur.

## Monitoring

SomnosSuite must provide sufficient observability for technical and operational failures. Monitoring may be performed directly by SomnosSuite, by a dedicated monitoring service, or by integration with existing enterprise monitoring. The preferred model is to expose application health and operational metrics that can be consumed by an established monitoring platform, rather than building a separate proprietary monitoring platform.

Potential integration points include HTTP health endpoints, structured logs, host or container metrics, database-health checks, synchronization metrics, and application events.

### Required Operational Signals

Monitoring should eventually cover at least: central API availability; database availability; ERP integration health and last successful ERP import; station connectivity; synchronization backlog and errors; database migration state; storage capacity; failed report generation; and failed report delivery. Additional metrics may be introduced as the system evolves.

### Alert Routing

Alerts must be routed according to operational impact.

Failures that may affect slaughter operations (for example, ERP feed unavailable, a capture station unable to synchronize for an extended period, the central capture service unavailable, or a station configuration conflict) should be visible to IT and slaughterhouse management.

Purely technical issues that do not currently affect slaughter operations (for example, background maintenance warnings, non-critical storage thresholds, or delayed non-operational reports) may initially be routed only to IT. The exact severity and escalation model remains to be defined.

## Maintenance and Deployment

### Production Change Window

SomnosSuite must not be updated during active slaughter operations. Application deployments, schema migrations, infrastructure changes, and other disruptive maintenance must be performed outside active production periods.

### Deployment Safety

Production deployment procedures should eventually include pre-deployment backup verification, application compatibility checks, database migration, application deployment, health verification, and a rollback or recovery procedure. Database migrations must be designed so that a failed deployment does not silently corrupt or lose operational data.

### Capture Continuity During Maintenance

Planned maintenance should occur outside slaughter operations. Offline capture (see [Offline Capture and Synchronization](OFFLINE_SYNC.md)) is intended primarily to protect against unplanned temporary outages and must not be used as a substitute for safe deployment planning.

## Open Decisions

| Topic                                            | Current Status                                              |
| ------------------------------------------------ | ----------------------------------------------------------- |
| Central data-loss target                         | RPO ≤ 24 hours                                              |
| Current backup strategy                          | VM-level backups                                            |
| Database-specific backups                        | Not currently required by the defined RPO                   |
| Restore testing                                  | Required                                                    |
| Disaster recovery                                | VM-based DR work in progress                                |
| Local unsynchronized data after workstation loss | Open                                                        |
| Monitoring platform                              | Existing platform available; replacement planned            |
| Production alert recipients                      | IT + slaughterhouse management where operationally relevant |
| Station replacement                              | Must be supported                                           |
| Production updates                               | Not allowed during active slaughter                         |
| Exact RTO                                        | Open / not yet quantified                                   |
