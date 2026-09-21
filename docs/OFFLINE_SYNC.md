# Offline Capture and Synchronization

> **Document role:** Target offline/synchronization model
>
> **Status:** Living document
>
> **Source of truth for:** Offline capture requirements, durable local storage, synchronization, idempotency, operation identity, station identity, sync status, conflict handling, and recovery of pending operations.
>
> **Does not describe:** Recovery point/time objectives (see [OPERATIONS_AND_RECOVERY.md](OPERATIONS_AND_RECOVERY.md) — this document does not define its own RPO/RTO values).

## Purpose

Capture availability at the slaughter lines must not depend on continuous availability of the central SomnosSuite server. Temporary loss of connectivity to the central server must not cause already captured data to be lost.

## Failure Domains

SomnosSuite distinguishes between different connectivity failures.

### Central SomnosSuite Server Unavailable

A capture station should continue operating locally when the central SomnosSuite server is temporarily unavailable. Captured records must be stored durably on the station and synchronized after connectivity is restored.

### ERP Integration Unavailable

The large-cattle and calf workflow depends on incoming animal records from the ERP integration (see [Integrations](INTEGRATIONS.md)). If the ERP data feed becomes unavailable, new animals cannot reliably be identified, so SomnosSuite cannot guarantee continued large-cattle/calf capture when the ERP upstream source itself is unavailable. This is different from loss of connectivity to the central SomnosSuite server.

### Pig Capture

Pig capture does not depend on an external per-animal data source, so the pig line can remain operational independently of ERP availability.

## Startup Requirements

SomnosSuite is not required to support starting a new operational session while the required central or upstream infrastructure is unavailable. For large-cattle and calf capture, starting a new session requires access to the ERP animal-data feed.

Once normal operation has started, a temporary loss of the central SomnosSuite server must not stop local capture. The primary offline requirement concerns this continuity after normal operation has already started, not unattended startup without any infrastructure.

## Local Durability

Captured records must survive temporary loss of connectivity, application restart, browser restart where technically possible, and temporary central-server outage. Captured data must not exist only in volatile browser memory.

## Synchronization

Locally captured data must be synchronized with the central system once connectivity is restored. The system must be able to distinguish at least: locally captured, pending synchronization, successfully synchronized, synchronization failed, and conflict requiring attention.

A station must clearly indicate whether unsynchronized records remain locally stored, and must not silently allow shutdown or completion of the operating day while unsynchronized data remains without warning the user. If unsynchronized records still exist when a station is restarted, SomnosSuite must automatically resume synchronization.

## Idempotency

A synchronization retry must not create duplicate central records. Every locally created operation should have a stable unique operation identifier, so that the same operation may be transmitted repeatedly without being processed more than once.

## Station Identity

Every capture station requires a stable SomnosSuite `StationId`, separate from user identity. The server must be able to determine from which station a capture or synchronized operation originated.

A browser application cannot reliably use the operating-system hostname as its station identity. A station should be provisioned once, through a station-enrollment mechanism that is still to be designed, and retain its identity locally across restarts.

If another physical workstation attempts to use an already active station identity, the system must reject the conflicting session. Multiple browser windows on the same physical station are not inherently prohibited, but multiple physical computers must not operate simultaneously under the same station identity. Authorized IT personnel must have a recovery mechanism to release or reassign a station identity if the previous session is no longer valid. The exact lease/session algorithm is not decided here.

## Existing Behavior

The legacy applications can retain local data until a daily closing operation, and data may remain local for an entire day or longer if daily closing is not performed. SomnosSuite should improve on this behavior by synchronizing continuously when connectivity is available, while still retaining durable local data during outages.
