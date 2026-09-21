# Domain Roadmap

> **Document role:** Implementation gap roadmap
>
> **Status:** Living document
>
> **Source of truth for:** Known differences between the target domain model ([DOMAIN_MODEL.md](DOMAIN_MODEL.md)) and the current implementation ([DOMAIN_RULES.md](DOMAIN_RULES.md)).

## Known Domain Gaps

| #   | Gap                                      | Target                                                                                                               | Current                                                                            |
| --- | ---------------------------------------- | -------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------- |
| 1   | Failure indicators                       | Multiple simultaneous failure indicators per stunning control.                                                       | A single failure indicator is supported.                                           |
| 2   | Corrective stunning actions              | Multiple corrective stunning actions per stunning control.                                                           | A single corrective device/timing combination is supported.                        |
| 3   | Stunning device animal-category approval | A device may be applicable to multiple animal categories.                                                            | `StunningDevice` models exactly one animal category.                               |
| 4   | Stunning device inspection history       | Full inspection history is retained.                                                                                 | Only the most recent inspection date is stored.                                    |
| 5   | Correction audit history                 | Full before/after correction history (value before, value after, correcting user, timestamp) is required.            | Corrections overwrite prior values; only the latest modifier metadata is retained. |
| 6   | Production authentication                | Fast operator identity/authentication at production stations (see [SECURITY_AND_ACCESS.md](SECURITY_AND_ACCESS.md)). | Not yet implemented.                                                               |
| 7   | Offline durable capture/synchronization  | Durable local capture with continuous synchronization (see [OFFLINE_SYNC.md](OFFLINE_SYNC.md)).                      | Not yet implemented.                                                               |

These gaps must be resolved before the corresponding production capture workflow is considered complete. Do not mark any of the above as implemented until the corresponding entry is also updated in [DOMAIN_RULES.md](DOMAIN_RULES.md).

## Next Integration Work

| Area              | Work                                                                                                          |
| ----------------- | ------------------------------------------------------------------------------------------------------------- |
| Application layer | Introduce use cases that call domain factories and behavior methods instead of constructing state directly.   |
| API layer         | Map request DTOs into domain inputs and translate `Result` failures into stable API responses.                |
| Time handling     | Supply current dates and timestamps from application/API services rather than reading time inside the domain. |
| Persistence       | Rehydrate aggregates through explicit `Rehydrate(...)` factories when loading persisted state.                |
| Serial numbers    | Enforce stunning-device serial-number uniqueness in application or persistence.                               |

## Reporting Work

| Area                 | Work                                                                              |
| -------------------- | --------------------------------------------------------------------------------- |
| Analysis calculation | Implement real `StunningCheckAnalysis` calculation from included stunning checks. |
| Export               | Add PDF/export generation outside the domain model.                               |
| Delivery             | Add corporate mail integration outside the domain model.                          |
| Finalization flow    | Wire finalized reports into application/API workflows once persistence exists.    |

## Audit Work

Current aggregates store only the latest modifier metadata. Future audit work should add explicit audit history where the product needs traceability beyond the current modified-by/modified-at fields (see gap #5 above).

Keep the current rule that behavior methods require caller-supplied audit data. Do not let domain objects read the current user or current time directly.

## Integration Notes

Mappings from external strings to domain enums belong in import, API, or application mapping code. They should not be embedded in domain entities or value objects.

## Later Cleanup

| Area           | Work                                                                                                            |
| -------------- | --------------------------------------------------------------------------------------------------------------- |
| WebApi surface | Replace placeholder endpoints with real application endpoints as API work starts.                               |
| Tests          | Add application/API tests when domain integration begins.                                                       |
| Documentation  | Keep this roadmap focused on remaining work; move implemented behavior into [DOMAIN_RULES.md](DOMAIN_RULES.md). |
