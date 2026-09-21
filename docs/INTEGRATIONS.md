# Integrations

> **Document role:** Target integration model
>
> **Status:** Living document
>
> **Source of truth for:** ERP integration, corporate mail integration, future external integrations, and integration assumptions/boundaries.
>
> **Does not describe:** Offline behavior when an integration is unavailable (see [OFFLINE_SYNC.md](OFFLINE_SYNC.md)), or deployment/component boundaries (see [ARCHITECTURE.md](ARCHITECTURE.md)).

## ERP Integration

### Purpose

The large-cattle and calf workflow depends on animal records produced by an ERP system. The ERP provides the animal record associated with each slaughtered large-cattle or calf animal, which is used to identify the animal before the stunning-control result is confirmed by the operator.

### Current Integration Method

The current legacy application receives ERP data through a semicolon-separated text file on a network share, where each line represents one slaughtered animal. The application watches the file for changes and processes newly appended lines. In normal operation, one new record is produced per slaughtered animal. The file is reset as part of the legacy daily-closing workflow.

### Available Data

The currently known data includes at least ear tag number, supplier information, and animal category. Additional fields may be configurable in the ERP. The exact set of fields available through the current integration must be verified.

### Alternative Formats

The ERP may also support structured formats such as XML or JSON. The extent to which these formats and additional fields are configurable has not yet been established, and should be investigated before the final integration contract is designed.

### Upstream Data Assumptions

The current operational process assumes that each slaughtered large-cattle or calf animal has a corresponding upstream record, a valid ear tag is available, an imported animal does not appear more than once, animals without an upstream record do not enter the normal slaughter workflow, and imported animals are expected to correspond to animals that are actually slaughtered.

Violations of these assumptions are currently considered upstream process errors. SomnosSuite should nevertheless handle malformed or duplicate input defensively and must not silently create inconsistent data.

### Architectural Consequence

A normal web browser cannot directly monitor a network-share file the way the legacy desktop application does — browsers do not have unrestricted access to network file shares or operating-system file-change notifications. ERP ingestion must therefore occur outside ordinary browser JavaScript, for example server-side or through a dedicated integration component.

The preferred architecture is a component that accesses the ERP data source, detects newly available animal records, validates and imports them, persists imported records, and makes them available to the appropriate capture station. This component may initially be deployed as part of the SomnosSuite backend, provided that the implementation remains logically separated from the capture domain (see [Architecture](ARCHITECTURE.md)). Whether it must become an independently deployable service is not decided here.

### Reliability

The ERP ingestion mechanism must not rely solely on transient file-system notifications. The integration should be capable of reconciling the source against already imported records after restart or temporary errors. Duplicate input must not create duplicate animal records, and malformed input must not silently enter the operational capture workflow.

See [Offline Capture and Synchronization](OFFLINE_SYNC.md) for how SomnosSuite behaves when the ERP feed itself is temporarily unavailable.

## Pig Workflow

The pig capture workflow currently has no external animal-identification source. Individual pigs are not identified through an upstream animal record; stunning-control records are captured sequentially by the operator at the pig line.

## Corporate Mail Integration

Scheduled reports are delivered automatically using the organization's corporate mail service. See [Reporting](REPORTING.md) for report delivery, failure handling, and resend requirements.

## Legacy Data Export

The existing legacy applications perform a daily closing operation, during which captured data is exported to a network drive as semicolon-separated text, with one animal per row and one file per day and station. Central availability of legacy data currently depends on completion of the daily closing process. This behavior does not apply to SomnosSuite; see [Offline Capture and Synchronization](OFFLINE_SYNC.md) for the target synchronization model.
