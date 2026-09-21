# Reporting

> **Document role:** Target reporting model
>
> **Status:** Living document
>
> **Source of truth for:** Report lifecycle, weekly/monthly reports, templates, scopes, recipients, report snapshots, generated artifacts, resend behavior, in-app availability, branding, export formats, and delivery failures.
>
> **Does not describe:** Recovery objectives (see [OPERATIONS_AND_RECOVERY.md](OPERATIONS_AND_RECOVERY.md)) or authorization/role definitions (see [SECURITY_AND_ACCESS.md](SECURITY_AND_ACCESS.md)).

## Purpose

SomnosSuite provides recurring operational and management reports based on stunning-control data. Reports support quality oversight, management review, and audit-related documentation.

Reporting must remain configurable without requiring IT intervention for ordinary business changes.

## Report Availability

Reports must be available through the SomnosSuite application and through automated delivery via the corporate mail service. Historical reports must remain accessible from within SomnosSuite.

## Report Scheduling

SomnosSuite must support at least weekly and monthly reports. Additional schedules may be introduced later. Reports are generated automatically and do not require manual approval before distribution.

## Report Recipients

Authorized users must be able to configure report recipients, which may include individual users or email addresses and organizational mailing groups.

Current report recipients include slaughterhouse management, safety management, and executive management. The set of recipients may evolve over time.

The exact technical integration mechanism with the corporate mail service remains to be defined.

## Report Content

Required report information includes at least:

- number of controlled animals;
- number of unsuccessful stunning controls;
- failure rate;
- number of corrective stunning actions;
- percentage distribution of successful and unsuccessful controls;
- observed failure indicators / causes;
- a detailed list of unsuccessful stunning controls, including failure indicator, corrective action, control timestamp, and operator.

Future reports may include additional analytical dimensions such as device, animal category, label, supplier, origin, or slaughter line.

## Report Scope

A report may include data from one or multiple slaughter lines, and one or multiple animal categories.

Authorized users must be able to configure the applicable scope through predefined report templates.

## Report Templates

Users must not create arbitrary report definitions. SomnosSuite provides predefined report templates whose supported parameters may be configured by authorized users.

Configurable template properties may include reporting period, data scope, included sections, recipients, delivery schedule, and export format. This provides flexibility while keeping report structure controlled and maintainable.

## Report Snapshot Semantics

A generated report represents the state of the relevant data at the time of report generation. Subsequent corrections to underlying stunning-control records must not modify an already generated report.

Previously issued reports are historical, immutable snapshots — not permanently live queries. A previously generated report must be downloadable exactly as it was originally generated. SomnosSuite must therefore retain the generated report artifact itself, or another representation that guarantees byte-equivalent or visually equivalent reproduction, rather than relying solely on re-running the original database query at a later date.

## Branding

Generated reports must support organizational branding, which may include a company logo, standardized headers and footers, report title and period, and consistent visual formatting. The final branding specification remains to be defined.

## Export Formats

PDF is required. Support for additional export formats such as Excel or CSV remains open.

## Email Delivery

Scheduled reports are delivered automatically using the organization's corporate mail service (see [Integrations](INTEGRATIONS.md)).

A user-facing audit trail of every successful delivery is not currently required. SomnosSuite must nevertheless know whether delivery succeeded or failed, sufficiently to react to delivery failures. Technical logs may contain delivery information required for troubleshooting, but a detailed business-facing delivery history is not currently required.

## Delivery Failure and Manual Resend

If automatic report delivery fails, the generated report must remain available in SomnosSuite, responsible users must receive an in-application notification, and the failure must not cause the report artifact to be lost.

Authorized users must be able to retry or manually resend an existing historical report. Resending must use the already generated historical report artifact rather than silently generating a new report from potentially changed source data.

## Dashboards

Threshold-based immediate report generation is not currently required. Operational and quality indicators may instead be presented through dashboards.

Potential dashboard metrics include current failure rate, number of controlled animals, number of corrective stunning actions, trends over time, and device-related statistics. Dashboards are a desired future capability; detailed dashboard requirements are defined separately.

## Open Decisions

| Topic                                     | Status                          |
| ----------------------------------------- | ------------------------------- |
| Weekly reports                            | Required                        |
| Monthly reports                           | Required                        |
| In-app report access                      | Required                        |
| Email delivery                            | Required                        |
| Corporate mail service integration        | Required, technical design open |
| Individual recipients                     | Required                        |
| Mailing groups                            | Required                        |
| Historical report snapshots               | Required                        |
| Exact historical re-download              | Required                        |
| Manual resend                             | Required                        |
| Failed-delivery notification              | Required                        |
| PDF                                       | Required                        |
| Excel / CSV                               | Open                            |
| Branding                                  | Required                        |
| User-created arbitrary report definitions | Not required                    |
| Configurable predefined templates         | Required                        |
| Multi-line / multi-category reports       | Required                        |
| Threshold-triggered reports               | Not currently required          |
| Operational dashboard                     | Desired                         |
