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
            StunningResult? stunningResult,
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
            StunningResult = stunningResult;
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
            StunningResult? stunningResult,
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
                stunningResult);

            if (rehydratedStateValidation.IsFailure)
                return Result<StunningCheck>.Failure(rehydratedStateValidation.Error);

            return new StunningCheck(
                id,
                animal,
                initialStunningDeviceId,
                createdAt,
                recordedByUserId,
                recordedAt,
                modifiedByUserId,
                modifiedAt,
                stunningResult,
                status,
                isDeleted);
        }

        public Result RecordOutcome(
            StunningResult stunningResult,
            Guid recordedByUserId,
            DateTimeOffset recordedAt)
        {
            ArgumentNullException.ThrowIfNull(stunningResult, nameof(stunningResult));

            if (IsDeleted)
                return Result.Failure(StunningCheckErrors.StunningCheckIsDeletedError);

            if (Status == StunningCheckStatus.Confirmed)
                return Result.Failure(StunningCheckErrors.RecordedChecksCannotBeRecordedAgainError);

            if (recordedByUserId == Guid.Empty)
                return Result.Failure(StunningCheckErrors.RecordedByUserIdIsRequiredError);

            if (recordedAt < CreatedAt)
                return Result.Failure(StunningCheckErrors.RecordedAtCannotBeBeforeCreatedAtError);

            StunningResult = stunningResult;
            RecordedByUserId = recordedByUserId;
            RecordedAt = recordedAt;
            Status = StunningCheckStatus.Confirmed;

            return Result.Success();
        }

        public Result CorrectOutcome(
            StunningResult stunningResult,
            Guid modifiedByUserId,
            DateTimeOffset modifiedAt)
        {
            ArgumentNullException.ThrowIfNull(stunningResult, nameof(stunningResult));

            if (IsDeleted)
                return Result.Failure(StunningCheckErrors.StunningCheckIsDeletedError);

            if (Status != StunningCheckStatus.Confirmed)
                return Result.Failure(StunningCheckErrors.ConfirmedCheckIsRequiredForCorrectionError);

            var modifiedInfoResult = UpdateModifiedInfo(modifiedByUserId, modifiedAt);


            if (modifiedInfoResult.IsFailure)
                return modifiedInfoResult;

            StunningResult = stunningResult;

            return Result.Success();
        }

        private static Result ValidateLifecycleState(
            StunningCheckStatus status,
            Guid? recordedByUserId,
            DateTimeOffset? recordedAt,
            StunningResult? stunningResult)
        {
            if (!Enum.IsDefined(status))
                return Result.Failure(StunningCheckErrors.StunningCheckStatusIsInvalidError);

            return status switch
            {
                StunningCheckStatus.Created => ValidateRehydratedCreatedState(
                    recordedByUserId,
                    recordedAt,
                    stunningResult),

                StunningCheckStatus.Confirmed => ValidateRehydratedConfirmedState(
                    recordedByUserId,
                    recordedAt,
                    stunningResult),

                _ => Result.Failure(StunningCheckErrors.StunningCheckStatusIsInvalidError)
            };
        }

        private static Result ValidateRehydratedCreatedState(
            Guid? recordedByUserId,
            DateTimeOffset? recordedAt,
            StunningResult? stunningResult)
        {
            if (recordedByUserId.HasValue)
                return Result.Failure(StunningCheckErrors.RecordedByUserIdIsNotAllowedError);
            if (recordedAt.HasValue)
                return Result.Failure(StunningCheckErrors.RecordedAtIsNotAllowedError);
            if (stunningResult is not null)
                return Result.Failure(StunningCheckErrors.StunningResultIsNotAllowedError);

            return Result.Success();
        }

        private static Result ValidateRehydratedConfirmedState(
            Guid? recordedByUserId,
            DateTimeOffset? recordedAt,
            StunningResult? stunningResult)
        {
            if (!recordedByUserId.HasValue || recordedByUserId.Value == Guid.Empty)
                return Result.Failure(StunningCheckErrors.RecordedByUserIdIsRequiredError);
            if (!recordedAt.HasValue)
                return Result.Failure(StunningCheckErrors.RecordedAtIsRequiredError);
            if (stunningResult is null)
                return Result.Failure(StunningCheckErrors.StunningResultIsRequiredError);
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
