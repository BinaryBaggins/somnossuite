using SomnosSuite.Domain.Animals;
using SomnosSuite.Domain.SharedKernel;

namespace SomnosSuite.Domain.StunningChecks
{
    public sealed class StunningCheck : BaseEntity, IAggregateRoot
    {
        public Animal? Animal { get; private set; }
        public Guid InitialStunningDeviceId { get; private set; }

        //if needed replace with audit history
        public DateTimeOffset CreatedAt { get; private set; }
        public Guid? RecordedByUserId { get; private set; }
        public DateTimeOffset? RecordedAt { get; private set; }
        public Guid? ModifiedByUserId { get; private set; }
        public DateTimeOffset? ModifiedAt { get; private set; }
        public StunningResult? StunningResult { get; private set; }

        public StunningOutcome? Outcome { get; private set; }
        public StunningFailureIndicator? FailureIndicator { get; private set; }
        public RestunningTiming? RestunningTiming { get; private set; }
        public Guid? RestunningDeviceId { get; private set; }

        public StunningCheckStatus Status { get; private set; }
        public bool IsDeleted { get; private set; }

        private StunningCheck() { }

        private StunningCheck(Animal animal, Guid initialStunningDeviceId, DateTimeOffset createdAt)
        : base(Guid.NewGuid())
        {
            Animal = animal;
            InitialStunningDeviceId = initialStunningDeviceId;
            Status = StunningCheckStatus.Created;
            CreatedAt = createdAt;
            IsDeleted = false;
        }

        private StunningCheck(
            Guid id,
            Animal animal,
            Guid initialStunningDeviceId,
            DateTimeOffset createdAt,
            Guid? recordedByUserId,
            DateTimeOffset? recordedAt,
            Guid? modifiedByUserId,
            DateTimeOffset? modifiedAt,
            StunningOutcome? outcome,
            StunningFailureIndicator? failureIndicator,
            RestunningTiming? restunningTiming,
            Guid? restunningDeviceId,
            StunningCheckStatus status,
            bool isDeleted)
        : base(id)
        {
            Animal = animal;
            InitialStunningDeviceId = initialStunningDeviceId;
            CreatedAt = createdAt;
            RecordedByUserId = recordedByUserId;
            RecordedAt = recordedAt;
            ModifiedByUserId = modifiedByUserId;
            ModifiedAt = modifiedAt;
            Outcome = outcome;
            FailureIndicator = failureIndicator;
            RestunningTiming = restunningTiming;
            RestunningDeviceId = restunningDeviceId;
            Status = status;
            IsDeleted = isDeleted;
        }

        public static Result<StunningCheck> Create(Animal animal, Guid initialStunningDeviceId, DateTimeOffset createdAt)
        {
            ArgumentNullException.ThrowIfNull(animal, nameof(animal)); // Required aggregate input; null indicates caller misuse

            if (initialStunningDeviceId == Guid.Empty)
                return Result<StunningCheck>.Failure(StunningCheckErrors.InitialStunningDeviceIdIsRequiredError);

            return new StunningCheck(animal, initialStunningDeviceId, createdAt);
        }

        public static Result<StunningCheck> Rehydrate(
            Guid id,
            Animal animal,
            Guid initialStunningDeviceId,
            DateTimeOffset createdAt,
            Guid? recordedByUserId,
            DateTimeOffset? recordedAt,
            Guid? modifiedByUserId,
            DateTimeOffset? modifiedAt,
            StunningOutcome? outcome,
            StunningFailureIndicator? failureIndicator,
            RestunningTiming? restunningTiming,
            Guid? restunningDeviceId,
            StunningCheckStatus status,
            bool isDeleted)
        {
            if (id == Guid.Empty)
                return Result<StunningCheck>.Failure(StunningCheckErrors.InvalidIdError);
            ArgumentNullException.ThrowIfNull(animal, nameof(animal));

            if (initialStunningDeviceId == Guid.Empty)
                return Result<StunningCheck>.Failure(StunningCheckErrors.InitialStunningDeviceIdIsRequiredError);

            var auditValidation = ValidateRehydratedAuditState(
                createdAt,
                recordedAt,
                modifiedByUserId,
                modifiedAt,
                isDeleted);

            if (auditValidation.IsFailure)
                return Result<StunningCheck>.Failure(auditValidation.Error);

            var rehydratedStateValidation = ValidateLifecycleState(
                status,
                recordedByUserId,
                recordedAt,
                outcome,
                failureIndicator,
                restunningTiming,
                restunningDeviceId);

            if (rehydratedStateValidation.IsFailure)
                return Result<StunningCheck>.Failure(rehydratedStateValidation.Error);

            if (outcome.HasValue)
            {
                var outcomeValidation = ValidateOutcomeRules(
                    outcome.Value,
                    failureIndicator,
                    restunningTiming,
                    restunningDeviceId);

                if (outcomeValidation.IsFailure)
                    return Result<StunningCheck>.Failure(outcomeValidation.Error);
            }

            return new StunningCheck(
                id,
                animal,
                initialStunningDeviceId,
                createdAt,
                recordedByUserId,
                recordedAt,
                modifiedByUserId,
                modifiedAt,
                outcome,
                failureIndicator,
                restunningTiming,
                restunningDeviceId,
                status,
                isDeleted);
        }

        public Result RecordOutcome(
            StunningOutcome outcome,
            StunningFailureIndicator? failureIndicator,
            RestunningTiming? restunningTiming,
            Guid recordedByUserId,
            Guid? restunningDeviceId,
            DateTimeOffset recordedAt)
        {
            if (IsDeleted)
                return Result.Failure(StunningCheckErrors.StunningCheckIsDeletedError);

            if (Status == StunningCheckStatus.Confirmed)
                return Result.Failure(StunningCheckErrors.RecordedChecksCannotBeRecordedAgainError);

            if (recordedByUserId == Guid.Empty)
                return Result.Failure(StunningCheckErrors.RecordedByUserIdIsRequiredError);

            if (recordedAt < CreatedAt)
                return Result.Failure(StunningCheckErrors.RecordedAtCannotBeBeforeCreatedAtError);

            var outcomeValidation = ValidateOutcomeRules(
                outcome,
                failureIndicator,
                restunningTiming,
                restunningDeviceId);

            if (outcomeValidation.IsFailure)
                return Result.Failure(outcomeValidation.Error);

            Outcome = outcome;
            FailureIndicator = failureIndicator;
            RestunningTiming = restunningTiming;
            RestunningDeviceId = restunningDeviceId;

            RecordedByUserId = recordedByUserId;
            RecordedAt = recordedAt;
            Status = StunningCheckStatus.Confirmed;

            return Result.Success();
        }

        public Result CorrectOutcome(
            StunningOutcome outcome,
            StunningFailureIndicator? failureIndicator,
            RestunningTiming? restunningTiming,
            Guid? restunningDeviceId,
            Guid modifiedByUserId,
            DateTimeOffset modifiedAt)
        {
            if (IsDeleted)
                return Result.Failure(StunningCheckErrors.StunningCheckIsDeletedError);

            if (Status != StunningCheckStatus.Confirmed)
                return Result.Failure(StunningCheckErrors.ConfirmedCheckIsRequiredForCorrectionError);

            var outcomeValidation = ValidateOutcomeRules(
                outcome,
                failureIndicator,
                restunningTiming,
                restunningDeviceId);

            if (outcomeValidation.IsFailure)
                return Result.Failure(outcomeValidation.Error);

            var modifiedInfoResult = UpdateModifiedInfo(modifiedByUserId, modifiedAt);
            if (modifiedInfoResult.IsFailure)
                return modifiedInfoResult;

            Outcome = outcome;
            FailureIndicator = failureIndicator;
            RestunningTiming = restunningTiming;
            RestunningDeviceId = restunningDeviceId;

            return Result.Success();
        }

        private static Result ValidateLifecycleState(
            StunningCheckStatus status,
            Guid? recordedByUserId,
            DateTimeOffset? recordedAt,
            StunningOutcome? outcome,
            StunningFailureIndicator? failureIndicator,
            RestunningTiming? restunningTiming,
            Guid? restunningDeviceId)
        {
            if (!Enum.IsDefined(status))
                return Result.Failure(StunningCheckErrors.StunningCheckStatusIsInvalidError);

            return status switch
            {
                StunningCheckStatus.Created => ValidateRehydratedCreatedState(
                    recordedByUserId,
                    recordedAt,
                    outcome,
                    failureIndicator,
                    restunningTiming,
                    restunningDeviceId),

                StunningCheckStatus.Confirmed => ValidateRehydratedConfirmedState(
                    recordedByUserId,
                    recordedAt,
                    outcome),

                _ => Result.Failure(StunningCheckErrors.StunningCheckStatusIsInvalidError)
            };
        }

        private static Result ValidateRehydratedCreatedState(
            Guid? recordedByUserId,
            DateTimeOffset? recordedAt,
            StunningOutcome? outcome,
            StunningFailureIndicator? failureIndicator,
            RestunningTiming? restunningTiming,
            Guid? restunningDeviceId)
        {
            if (recordedByUserId.HasValue)
                return Result.Failure(StunningCheckErrors.RecordedByUserIdIsNotAllowedError);
            if (recordedAt.HasValue)
                return Result.Failure(StunningCheckErrors.RecordedAtIsNotAllowedError);
            if (outcome.HasValue)
                return Result.Failure(StunningCheckErrors.OutcomeIsNotAllowedError);
            if (failureIndicator.HasValue)
                return Result.Failure(StunningCheckErrors.FailureIndicatorIsNotAllowedError);
            if (restunningTiming.HasValue)
                return Result.Failure(StunningCheckErrors.RestunningTimingIsNotAllowedError);
            if (restunningDeviceId.HasValue)
                return Result.Failure(StunningCheckErrors.RestunningDeviceIdIsNotAllowedError);
            return Result.Success();
        }

        private static Result ValidateRehydratedConfirmedState(
            Guid? recordedByUserId,
            DateTimeOffset? recordedAt,
            StunningOutcome? outcome)
        {
            if (!recordedByUserId.HasValue || recordedByUserId.Value == Guid.Empty)
                return Result.Failure(StunningCheckErrors.RecordedByUserIdIsRequiredError);
            if (!recordedAt.HasValue)
                return Result.Failure(StunningCheckErrors.RecordedAtIsRequiredError);
            if (!outcome.HasValue)
                return Result.Failure(StunningCheckErrors.StunningOutcomeIsRequiredError);
            return Result.Success();
        }

        private static Result ValidateOutcomeRules(
            StunningOutcome outcome,
            StunningFailureIndicator? failureIndicator,
            RestunningTiming? restunningTiming,
            Guid? restunningDeviceId)
        {
            if (!Enum.IsDefined(outcome))
                return Result.Failure(StunningCheckErrors.StunningOutcomeIsInvalidError);

            return outcome switch
            {
                StunningOutcome.Successful => ValidateSuccessfulOutcome(
                    failureIndicator,
                    restunningTiming,
                    restunningDeviceId),

                StunningOutcome.Failed => ValidateFailedOutcome(
                    failureIndicator,
                    restunningTiming,
                    restunningDeviceId),

                _ => Result.Failure(StunningCheckErrors.StunningOutcomeIsInvalidError)
            };
        }

        private static Result ValidateSuccessfulOutcome(
            StunningFailureIndicator? failureIndicator,
            RestunningTiming? restunningTiming,
            Guid? restunningDeviceId)
        {
            if (failureIndicator.HasValue)
                return Result.Failure(StunningCheckErrors.FailureIndicatorIsNotAllowedError);

            if (restunningTiming.HasValue)
                return Result.Failure(StunningCheckErrors.RestunningTimingIsNotAllowedError);

            if (restunningDeviceId.HasValue)
                return Result.Failure(StunningCheckErrors.RestunningDeviceIdIsNotAllowedError);

            return Result.Success();
        }

        private static Result ValidateFailedOutcome(
            StunningFailureIndicator? failureIndicator,
            RestunningTiming? restunningTiming,
            Guid? restunningDeviceId)
        {
            if (!failureIndicator.HasValue)
                return Result.Failure(StunningCheckErrors.FailureIndicatorIsRequiredError);

            if (!restunningTiming.HasValue)
                return Result.Failure(StunningCheckErrors.RestunningTimingIsRequiredError);

            if (!restunningDeviceId.HasValue || restunningDeviceId.Value == Guid.Empty)
                return Result.Failure(StunningCheckErrors.RestunningDeviceIdIsRequiredError);

            return Result.Success();
        }

        private Result UpdateModifiedInfo(Guid modifiedByUserId, DateTimeOffset modifiedAt)
        {
            if (modifiedByUserId == Guid.Empty)
                return Result.Failure(StunningCheckErrors.ModifiedByUserIdIsRequiredError);

            if (modifiedAt < CreatedAt)
                return Result.Failure(StunningCheckErrors.ModifiedAtCannotBeBeforeCreatedAtError);

            ModifiedByUserId = modifiedByUserId;
            ModifiedAt = modifiedAt;
            return Result.Success();
        }

        public Result MarkAsDeleted(Guid modifiedByUserId, DateTimeOffset modifiedAt)
        {
            if (IsDeleted)
                return Result.Success();

            var modifiedInfoResult = UpdateModifiedInfo(modifiedByUserId, modifiedAt);
            if (modifiedInfoResult.IsFailure)
                return modifiedInfoResult;

            IsDeleted = true;

            return Result.Success();
        }

        private static Result ValidateRehydratedAuditState(
            DateTimeOffset createdAt,
            DateTimeOffset? recordedAt,
            Guid? modifiedByUserId,
            DateTimeOffset? modifiedAt,
            bool isDeleted)
        {
            if (modifiedByUserId.HasValue != modifiedAt.HasValue)
                return Result.Failure(StunningCheckErrors.ModifiedInfoIsIncompleteError);

            if (modifiedByUserId == Guid.Empty)
                return Result.Failure(StunningCheckErrors.ModifiedByUserIdIsRequiredError);

            if (recordedAt.HasValue && recordedAt.Value < createdAt)
                return Result.Failure(StunningCheckErrors.RecordedAtCannotBeBeforeCreatedAtError);

            if (modifiedAt.HasValue && modifiedAt.Value < createdAt)
                return Result.Failure(StunningCheckErrors.ModifiedAtCannotBeBeforeCreatedAtError);

            if (isDeleted && (!modifiedByUserId.HasValue || !modifiedAt.HasValue))
                return Result.Failure(StunningCheckErrors.ModifiedInfoIsRequiredForDeletedCheckError);

            return Result.Success();
        }
    }
}
