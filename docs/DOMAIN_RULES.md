# Domain Rules

> **Document role:** Current implementation
>
> **Status:** Living document
>
> **Source of truth for:** Business rules currently enforced by domain code in `backend/src/SomnosSuite.Domain`.
>
> **Does not describe:** Future or target behavior. See [DOMAIN_MODEL.md](DOMAIN_MODEL.md) for the target model and [DOMAIN_ROADMAP.md](DOMAIN_ROADMAP.md) for known gaps between the two.

Keep this file aligned with domain tests in `backend/tests/SomnosSuite.Domain.Tests`.

## Backend Context

The active backend is the layered solution in `backend/backend.sln`:

- `src/SomnosSuite.Domain`
- `src/SomnosSuite.Application`
- `src/SomnosSuite.Infrastructure`
- `src/SomnosSuite.Persistence`
- `src/SomnosSuite.Presentation`
- `src/SomnosSuite.WebApi`
- `tests/SomnosSuite.Domain.Tests`

## Domain Boundaries

The domain project owns business invariants, lifecycle rules, value-object construction, aggregate behavior, and domain error definitions. It does not own persistence, transport DTOs, API validation, import mapping, authorization, report export, mail delivery, or system-clock access.

Current aggregate roots: `User`, `StunningDevice`, `StunningCheck`, `StunningCheckReport`.

Current value objects: `Animal`, `ReportPeriod`, `StunningCheckAnalysis`, `StunningResult`, `CorrectiveStunningAction`.

Current shared kernel concepts: `Result` and `Result<T>` for expected domain failures, `Error` for stable error identity, and `BaseEntity`, `IEntity`, `IAggregateRoot`, `IValueObject` markers.

## Validation Policy

Exceptions are reserved for programming errors and contract violations — used when the caller has supplied an invalid object graph or broken a method contract (for example, a required domain object or dependency is `null`, or an impossible internal state is reached).

`Result.Failure(...)` is used for invalid domain input, invalid persisted state, and invalid state transitions (for example, required text missing or whitespace, `Guid.Empty`, invalid enum values, invalid dates or chronology, invalid lifecycle transitions, or incomplete persisted audit state).

Domain methods receive current dates and timestamps from callers. Domain objects do not read system time directly.

## Shared Policies

| Area                     | Rule                                                                                                                                                                                                                                                                          |
| ------------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Aggregate construction   | Aggregate roots expose explicit `Create(...)` factories for new instances.                                                                                                                                                                                                    |
| Aggregate rehydration    | Aggregate roots expose explicit `Rehydrate(...)` factories for persisted state validation.                                                                                                                                                                                    |
| Value objects            | Value objects use `Create(...)`; separate rehydration factories are not required.                                                                                                                                                                                             |
| Time                     | Domain methods receive current dates and timestamps from callers. Domain code does not call system time directly.                                                                                                                                                             |
| Expected failures        | Invalid domain input, invalid persisted state, and invalid transitions return `Result.Failure(...)`.                                                                                                                                                                          |
| Contract failures        | Required domain objects passed as `null` throw exceptions.                                                                                                                                                                                                                    |
| Soft delete              | `MarkAsDeleted(...)` returns `Result`, rejects empty modifier ids, and repeated delete is success/no-op.                                                                                                                                                                      |
| Deleted aggregates       | Deleted aggregates reject behavior changes other than idempotent delete.                                                                                                                                                                                                      |
| Audit metadata           | Mutating aggregate behavior requires modifier audit where implemented. Aggregates currently store only the latest modifier metadata (creator/modifier id and timestamp) — no separate before/after value history is retained yet; see [DOMAIN_ROADMAP.md](DOMAIN_ROADMAP.md). |
| Rehydrated deleted state | Deleted persisted aggregates require complete modifier audit.                                                                                                                                                                                                                 |

## Animal

`Animal` is a value object.

The current `AnimalKind` enum is exactly: `Schwein`, `Rind`, `Kuh`, `Muni`, `Ochse`, `Kalb`. `AnimalClassification` maps `Schwein` and `Kalb` to `AnimalCategory.Kleinvieh`, and `Rind`, `Kuh`, `Muni`, `Ochse` to `AnimalCategory.Grossvieh`. This corresponds to the target business terminology in [DOMAIN_MODEL.md](DOMAIN_MODEL.md): `Schwein` is `Pig`, `Kalb` is `Calf`, and `Kleinvieh`/`Grossvieh` are `KV`/`GV`.

| Rule                       | Behavior                                                                                          |
| -------------------------- | ------------------------------------------------------------------------------------------------- |
| Kind                       | `AnimalKind` must be a defined enum value.                                                        |
| Category                   | `AnimalCategory` is derived from `AnimalKind` through `AnimalClassification`.                     |
| Large cattle (Grossvieh)   | `Rind`, `Kuh`, `Muni`, and `Ochse` require ear tag number and supplier name.                      |
| Calf (`Kalb`)              | Requires ear tag number and supplier name, despite being `Kleinvieh`.                             |
| Pig (`AnimalKind.Schwein`) | The only currently implemented kind that can be created without ear tag number and supplier name. |
| Optional text              | Optional text values are trimmed. Blank optional values normalize to `null`.                      |

## User

`User` is an aggregate root.

| Rule                | Behavior                                                                                                   |
| ------------------- | ---------------------------------------------------------------------------------------------------------- |
| Required fields     | Name, email, password hash, role, status, and created timestamp are required.                              |
| Name                | Trimmed and must not be blank.                                                                             |
| Email               | Trimmed, required, and validated as an address string.                                                     |
| Password hash       | Required and stored exactly as supplied; it is not trimmed.                                                |
| Role                | `UserRole` must be a defined enum value.                                                                   |
| Status              | `UserStatus` must be a defined enum value.                                                                 |
| Allowed transitions | `Invited -> Active`, `Invited -> Deactivated`, `Active -> Deactivated`, `Deactivated -> Active`.           |
| Updates             | Name, email, password hash, role, and status updates require modifier audit.                               |
| Rehydration         | Requires non-empty id, valid fields, consistent modifier audit, and `UpdatedAt >= CreatedAt` when updated. |
| Soft delete         | Deleted users reject non-delete changes. Rehydrated deleted users require modifier audit.                  |

## StunningDevice

`StunningDevice` is an aggregate root.

| Rule               | Behavior                                                                                                                                                             |
| ------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Required fields    | Device type, manufacturer, serial number, model, animal category, and caller-supplied `today` are required.                                                          |
| Text               | Manufacturer, serial number, and model are trimmed and must not be blank.                                                                                            |
| Enums              | `StunningDeviceType` and `AnimalCategory` must be defined enum values.                                                                                               |
| Animal category    | The current model stores a single `AnimalCategory` per device. The target model supports multiple applicable categories; see [DOMAIN_ROADMAP.md](DOMAIN_ROADMAP.md). |
| Inspection date    | Last inspection date cannot be in the future relative to caller-supplied `today`.                                                                                    |
| Record inspection  | New inspection date cannot be in the future or older than the current last inspection date.                                                                          |
| Inspection history | Only the most recent inspection date is currently stored; full inspection history is not yet modeled. See [DOMAIN_ROADMAP.md](DOMAIN_ROADMAP.md).                    |
| Audit              | Recording an inspection requires modifier audit.                                                                                                                     |
| Rehydration        | Requires non-empty id, valid fields, and consistent modifier audit.                                                                                                  |
| Soft delete        | Deleted devices reject inspection changes. Rehydrated deleted devices require modifier audit.                                                                        |
| Uniqueness         | Serial-number uniqueness is required, but enforced outside the entity in application or persistence.                                                                 |

## StunningCheck

`StunningCheck` is an aggregate root for one stunning control. It owns one `StunningResult` value object after an outcome is recorded. A `StunningResult` contains one `StunningOutcome`, zero or more `StunningFailureIndicator` values, and zero or more `CorrectiveStunningAction` value objects. Each corrective action contains a non-empty device id and a `CorrectiveStunningTiming`.

| Rule              | Behavior                                                                                                                                                                                                                                                                                                                    |
| ----------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Creation          | Requires a non-null `Animal`, non-empty initial stunning device id, and created timestamp.                                                                                                                                                                                                                                  |
| Initial state     | New checks start in `Created` status.                                                                                                                                                                                                                                                                                       |
| Recording         | `RecordOutcome(...)` receives an already-valid `StunningResult`, records it as the first outcome, and moves the check to `Confirmed`.                                                                                                                                                                                       |
| Recording audit   | Recording requires non-empty recorded-by user id and `RecordedAt >= CreatedAt`.                                                                                                                                                                                                                                             |
| Re-recording      | Confirmed checks cannot be recorded again.                                                                                                                                                                                                                                                                                  |
| Correction        | `CorrectOutcome(...)` receives an already-valid replacement `StunningResult` and is the only correction path. It requires the check to already be confirmed. Correction currently overwrites the previous result; it does not yet retain a separate before/after value history. See [DOMAIN_ROADMAP.md](DOMAIN_ROADMAP.md). |
| Correction audit  | Corrections require modifier audit and `ModifiedAt >= CreatedAt`.                                                                                                                                                                                                                                                           |
| Successful result | `StunningOutcome.Successful` allows no failure indicators and no corrective stunning actions.                                                                                                                                                                                                                               |
| Failed result     | `StunningOutcome.Failed` requires at least one failure indicator and at least one corrective stunning action. Multiple simultaneous failure indicators and multiple corrective stunning actions are supported. Duplicate failure indicators are rejected. Corrective actions may use the same device/timing combination.    |
| Rehydration       | Requires non-empty id, valid lifecycle state, a valid `StunningResult` when confirmed, consistent audit state, and minimum chronology.                                                                                                                                                                                      |
| Soft delete       | Deleted checks reject recording and correction. Rehydrated deleted checks require modifier audit.                                                                                                                                                                                                                           |

## ReportPeriod

`ReportPeriod` is a value object.

| Rule       | Behavior                                                       |
| ---------- | -------------------------------------------------------------- |
| Start      | Start is required.                                             |
| End        | End is required.                                               |
| Chronology | End must be after start.                                       |
| Semantics  | The implemented names are `StartInclusive` and `EndExclusive`. |

## StunningCheckReport

`StunningCheckReport` is an aggregate root.

| Rule            | Behavior                                                                                                                                   |
| --------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| Creation        | Requires non-null report period, non-empty created-by user id, and created timestamp.                                                      |
| Initial state   | Reports start in `Draft`.                                                                                                                  |
| Draft changes   | Draft reports can add/remove stunning check ids and update analysis.                                                                       |
| Add duplicate   | Adding an already included check id is success/no-op.                                                                                      |
| Remove missing  | Removing a missing check id fails.                                                                                                         |
| Finalization    | Requires draft status, not deleted, at least one stunning check id, analysis, and modifier audit.                                          |
| Finalized state | Finalized reports reject check id and analysis changes.                                                                                    |
| Rehydration     | Requires non-empty id, valid status, valid audit, no empty check ids, no duplicate check ids, and `ModifiedAt >= CreatedAt` when modified. |
| Analysis audit  | Rehydrated reports with analysis require modifier audit.                                                                                   |
| Finalized audit | Rehydrated finalized reports require modifier audit.                                                                                       |
| Soft delete     | Deleted reports reject behavior changes except idempotent delete.                                                                          |

## External Mapping Reference

String mapping from external data sources belongs outside the domain, in import, API, or application mapping code. These values are preserved as neutral integration examples.

| External value                 | Domain value                              |
| ------------------------------ | ----------------------------------------- |
| `gut`                          | `StunningOutcome.Successful`              |
| `(Augen-)Reflexe`              | `StunningFailureIndicator.Reflex`         |
| `Reflex des Tieres`            | `StunningFailureIndicator.Reflex`         |
| `Lautaeusserung`               | `StunningFailureIndicator.Vocalization`   |
| `Schnappatmung`                | `StunningFailureIndicator.Gasping`        |
| `Bolzenschuss vor Entblutung`  | `CorrectiveStunningTiming.BeforeBleeding` |
| `Bolzenschuss nach Entblutung` | `CorrectiveStunningTiming.AfterBleeding`  |
| `CO2`                          | `StunningDeviceType.CarbonDioxide`        |
| `Bolzenschuss`                 | `StunningDeviceType.CaptiveBolt`          |

## Tested State

The current domain test suite covers the implemented value-object and aggregate rules for animals, users, stunning devices, stunning checks, report periods, and stunning check reports.

Last verified command:

```powershell
dotnet test tests\SomnosSuite.Domain.Tests\SomnosSuite.Domain.Tests.csproj
```

Result on 2026-05-28: 37 tests passed, 0 failed, 0 skipped. This is a point-in-time snapshot; re-run the command above for current results.
