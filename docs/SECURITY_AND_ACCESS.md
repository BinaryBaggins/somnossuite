# Security and Access

> **Document role:** Target security and access model
>
> **Status:** Living document
>
> **Source of truth for:** User identity, production-station operator identity, office authentication requirements, authorization, multiple roles/permissions, user switching, offline authenticated sessions, correction attribution, and audit identity.
>
> **Does not describe:** Recovery objectives (see [OPERATIONS_AND_RECOVERY.md](OPERATIONS_AND_RECOVERY.md)) or reporting content (see [REPORTING.md](REPORTING.md)).

## Purpose

SomnosSuite must identify the person performing business-relevant actions while keeping authentication practical for time-critical production workflows.

Authentication requirements differ between production capture stations and normal office workstations.

## Identity Environment

The organization currently operates an on-premises corporate directory service, with synchronization to a cloud corporate identity provider planned.

SomnosSuite should avoid introducing an independent user identity system where an existing corporate identity can be reused. The exact integration with the corporate directory and cloud identity provider remains to be designed. This document does not decide a specific identity-provider integration, or JWT vs. cookie-based session handling.

## User Population

SomnosSuite is currently intended for internal employees. Support for external users may be required in the future.

The authentication and authorization architecture should therefore avoid assumptions that make future external identities unnecessarily difficult to introduce.

## Production Station Identity

Production touch PCs currently run under a shared operating-system station account, which does not identify the individual employee performing a stunning control.

SomnosSuite must therefore maintain a separate application-level user identity for production capture. A business-relevant capture must be attributable to the actual operator and not merely to the shared workstation account. Station identity itself (as opposed to user identity) is defined in [Offline Capture and Synchronization](OFFLINE_SYNC.md).

## Office Workstations

Office and administrative workstations may use corporate identity mechanisms. Planned identity infrastructure includes the on-premises corporate directory, cloud identity provider synchronization, and biometric authentication for eligible non-production workstations.

The final authentication mechanism for office users remains an architectural decision. Single sign-on should be evaluated before introducing a separate SomnosSuite password.

## Production Authentication

Authentication at production stations must be significantly faster than a conventional username/password workflow, since production stations use touch screens with an on-screen keyboard, making frequent password entry undesirable.

Existing employee badges (RFID/NFC) may provide a suitable mechanism for rapid user identification or authentication. The security properties of the existing badge technology must be verified before it is treated as a trusted authentication factor. SomnosSuite must not assume that possession of a readable badge identifier alone provides strong authentication.

## User Switching

Production stations are shared by multiple employees. SomnosSuite should provide a fast mechanism for switching the active operator, minimizing interruption of the capture process while ensuring that subsequent captures are attributed to the correct employee. The exact user-switching interaction remains to be defined.

## Offline Authentication

SomnosSuite is not required to support a new login while the authentication infrastructure is unavailable.

If a user has already authenticated successfully before connectivity is lost, the active session may continue to be used for offline capture. If that user explicitly logs out while the station is offline, a new login is not guaranteed until connectivity is restored.

## Audit Identity and Attribution

Every stunning-control record must identify the user responsible for the capture operation, together with the relevant capture timestamp.

If a record is corrected, SomnosSuite must retain the identity of the user who performed the correction. The correcting user may be the same person who created the original record, but the system must not assume this — creation and modification attribution are separate audit facts.

A capture may therefore contain audit information such as operator identity, station identity, capture timestamp, and correction identity and timestamp where applicable. User identity and station identity are separate concepts.

### Correction History

Where records are corrected after capture, SomnosSuite must retain enough history to determine the original value, the corrected value, who performed the correction, and when the correction occurred (see [Requirements](REQUIREMENTS.md), SEC-005).

During the immediate operational capture process, the station operator may correct an erroneous entry without providing a reason, since the capture workflow must remain fast (see [Capture Workflow](CAPTURE_WORKFLOW.md)).

Corrections performed after the immediate operational context are historical corrections and are more sensitive, because the original circumstances are harder to reconstruct. Historical corrections may require additional authorization and an explanatory reason; the exact boundary and workflow for this remain open (see [Requirements](REQUIREMENTS.md), COMP-004).

Detailed regulatory requirements for audit history retention are still unknown and must be investigated.

## Authorization

Authorization must be based on business responsibilities rather than workstation identity. A user may hold multiple responsibilities simultaneously; the authorization model must support multiple permissions or responsibilities per user rather than assuming exactly one mutually exclusive role. Access is not currently required to be restricted to a specific slaughter line.

Current responsibility areas, which are not mutually exclusive:

- **Operators** — capture, immediate operational correction.
- **Shift/department managers** — operational overview, employees, failure rates, device management, stunning-method management, audit-related views.
- **Quality assurance / responsible personnel** — reports: period, content, scope, recipients, schedule, export format, quality metrics.
- **Management** — higher-level reports and overview.
- **IT administrators** — technical administration, deployment, infrastructure, station recovery.

The exact permissions for each responsibility area remain to be defined.

## User Administration

User administration may be performed by IT or by authorized slaughterhouse management. The exact division of responsibilities between identity administration and SomnosSuite authorization administration remains to be defined.

## Data Retention and Auditor Access

Retention requirements are defined in [Requirements](REQUIREMENTS.md) (COMP-002, COMP-003); this document does not redefine them. Automatic deletion must not be introduced until the applicable regulatory requirements are known.

A dedicated interactive auditor role is not currently required; audit information is currently provided through generated reports (see [Reporting](REPORTING.md)). Future direct auditor access may be introduced if business or regulatory requirements change.

## Future Authentication Capabilities

Potential future capabilities include cloud identity provider integration, corporate single sign-on, biometric authentication on supported workstations, and external identities. These are future capabilities and are not requirements for the initial production-station authentication workflow.

## Open Security Decisions

| Decision                                     | Status          | Notes                                                                      |
| -------------------------------------------- | --------------- | -------------------------------------------------------------------------- |
| Corporate identity source                    | Partially known | On-prem directory exists; cloud identity provider sync planned             |
| Office authentication                        | Open            | Evaluate SSO / cloud identity provider / integrated Windows authentication |
| Production authentication                    | Open            | Fast operator authentication required; RFID/NFC badge candidate            |
| Production user switching                    | Open            | Must be fast enough for line operation                                     |
| Offline re-login                             | Decided         | Not required                                                               |
| Existing authenticated session during outage | Decided         | May continue                                                               |
| Multiple roles per user                      | Decided         | Required                                                                   |
| Line-scoped permissions                      | Decided         | Not currently required                                                     |
| External identities                          | Future          | May be required later                                                      |
| Historical correction reason/authorization   | Open            | See Requirements COMP-004                                                  |
