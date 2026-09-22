using FluentAssertions;
using SomnosSuite.Domain.Animals;
using SomnosSuite.Domain.SharedKernel;
using SomnosSuite.Domain.StunningChecks;
using Xunit;

namespace SomnosSuite.Domain.Tests.StunningChecks;

public sealed class StunningCheckTests
{
    private static readonly Guid CheckId =
        Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static readonly Guid InitialDeviceId =
        Guid.Parse("44444444-4444-4444-4444-444444444444");

    private static readonly Guid CorrectiveDeviceId =
        Guid.Parse("55555555-5555-5555-5555-555555555555");

    private static readonly Guid UserId =
        Guid.Parse("66666666-6666-6666-6666-666666666666");

    private static readonly DateTimeOffset CreatedAt =
        new(2026, 5, 1, 8, 0, 0, TimeSpan.Zero);

    private static readonly DateTimeOffset RecordedAt =
        new(2026, 5, 1, 9, 0, 0, TimeSpan.Zero);

    private static readonly DateTimeOffset ModifiedAt =
        new(2026, 5, 1, 10, 0, 0, TimeSpan.Zero);


    [Fact]
    public void Create_Should_Create_Check_In_Created_State()
    {
        var result = StunningCheck.Create(
            ValidAnimal(),
            InitialDeviceId,
            CreatedAt);

        result.IsSuccess.Should().BeTrue();

        var check = result.Value;

        check.Id.Should().NotBe(Guid.Empty);
        check.Animal.Should().Be(ValidAnimal());
        check.InitialStunningDeviceId.Should().Be(InitialDeviceId);
        check.CreatedAt.Should().Be(CreatedAt);

        check.Status.Should().Be(StunningCheckStatus.Created);
        check.StunningResult.Should().BeNull();
        check.RecordedByUserId.Should().BeNull();
        check.RecordedAt.Should().BeNull();
        check.ModifiedByUserId.Should().BeNull();
        check.ModifiedAt.Should().BeNull();
        check.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Create_Should_Reject_Empty_Initial_Device_Id()
    {
        var result = StunningCheck.Create(
            ValidAnimal(),
            Guid.Empty,
            CreatedAt);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.InitialStunningDeviceIdIsRequiredError);
    }

    [Fact]
    public void Create_Should_Throw_When_Animal_Is_Null()
    {
        Action act = () => StunningCheck.Create(
            null!,
            InitialDeviceId,
            CreatedAt);

        act.Should().Throw<ArgumentNullException>();
    }


    [Fact]
    public void RecordOutcome_Should_Confirm_Check()
    {
        var check = ValidCreatedCheck();
        var stunningResult = ValidFailedStunningResult();

        var result = check.RecordOutcome(
            stunningResult,
            UserId,
            RecordedAt);

        result.IsSuccess.Should().BeTrue();

        check.StunningResult.Should().BeSameAs(stunningResult);
        check.RecordedByUserId.Should().Be(UserId);
        check.RecordedAt.Should().Be(RecordedAt);
        check.Status.Should().Be(StunningCheckStatus.Confirmed);
    }

    [Fact]
    public void RecordOutcome_Should_Reject_Already_Confirmed_Check()
    {
        var check = RehydrateConfirmed(
            UserId,
            RecordedAt,
            ValidFailedStunningResult()).Value;

        var result = check.RecordOutcome(
            ValidSuccessfulStunningResult(),
            UserId,
            ModifiedAt);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.RecordedChecksCannotBeRecordedAgainError);
    }

    [Fact]
    public void RecordOutcome_Should_Reject_Deleted_Check()
    {
        var check = RehydrateConfirmed(
            UserId,
            RecordedAt,
            ValidFailedStunningResult(),
            modifiedByUserId: UserId,
            modifiedAt: ModifiedAt,
            isDeleted: true).Value;

        var result = check.RecordOutcome(
            ValidSuccessfulStunningResult(),
            UserId,
            ModifiedAt);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.StunningCheckIsDeletedError);
    }

    [Fact]
    public void RecordOutcome_Should_Reject_Empty_RecordedByUserId()
    {
        var check = ValidCreatedCheck();

        var result = check.RecordOutcome(
            ValidFailedStunningResult(),
            Guid.Empty,
            RecordedAt);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.RecordedByUserIdIsRequiredError);
    }

    [Fact]
    public void RecordOutcome_Should_Reject_RecordedAt_Before_CreatedAt()
    {
        var check = ValidCreatedCheck();

        var result = check.RecordOutcome(
            ValidFailedStunningResult(),
            UserId,
            CreatedAt.AddTicks(-1));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.RecordedAtCannotBeBeforeCreatedAtError);
    }

    [Fact]
    public void RecordOutcome_Should_Throw_When_StunningResult_Is_Null()
    {
        var check = ValidCreatedCheck();

        Action act = () => check.RecordOutcome(
            null!,
            UserId,
            RecordedAt);

        act.Should().Throw<ArgumentNullException>();
    }


    [Fact]
    public void CorrectOutcome_Should_Replace_Result_And_Set_Modification_Audit()
    {
        var originalResult = ValidFailedStunningResult();

        var check = RehydrateConfirmed(
            UserId,
            RecordedAt,
            originalResult).Value;

        var correctedResult = ValidSuccessfulStunningResult();

        var result = check.CorrectOutcome(
            correctedResult,
            UserId,
            ModifiedAt);

        result.IsSuccess.Should().BeTrue();

        check.StunningResult.Should().BeSameAs(correctedResult);
        check.StunningResult.Should().NotBeSameAs(originalResult);
        check.ModifiedByUserId.Should().Be(UserId);
        check.ModifiedAt.Should().Be(ModifiedAt);

        check.RecordedByUserId.Should().Be(UserId);
        check.RecordedAt.Should().Be(RecordedAt);
        check.Status.Should().Be(StunningCheckStatus.Confirmed);
    }

    [Fact]
    public void CorrectOutcome_Should_Reject_Unconfirmed_Check()
    {
        var check = ValidCreatedCheck();

        var result = check.CorrectOutcome(
            ValidSuccessfulStunningResult(),
            UserId,
            ModifiedAt);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.ConfirmedCheckIsRequiredForCorrectionError);
    }

    [Fact]
    public void CorrectOutcome_Should_Reject_Deleted_Check()
    {
        var check = RehydrateConfirmed(
            UserId,
            RecordedAt,
            ValidFailedStunningResult(),
            modifiedByUserId: UserId,
            modifiedAt: ModifiedAt,
            isDeleted: true).Value;

        var result = check.CorrectOutcome(
            ValidSuccessfulStunningResult(),
            UserId,
            ModifiedAt.AddHours(1));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.StunningCheckIsDeletedError);
    }

    [Fact]
    public void CorrectOutcome_Should_Reject_Empty_ModifiedByUserId()
    {
        var check = RehydrateConfirmed(
            UserId,
            RecordedAt,
            ValidFailedStunningResult()).Value;

        var result = check.CorrectOutcome(
            ValidSuccessfulStunningResult(),
            Guid.Empty,
            ModifiedAt);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.ModifiedByUserIdIsRequiredError);
    }

    [Fact]
    public void CorrectOutcome_Should_Reject_ModifiedAt_Before_CreatedAt()
    {
        var check = RehydrateConfirmed(
            UserId,
            RecordedAt,
            ValidFailedStunningResult()).Value;

        var result = check.CorrectOutcome(
            ValidSuccessfulStunningResult(),
            UserId,
            CreatedAt.AddTicks(-1));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.ModifiedAtCannotBeBeforeCreatedAtError);
    }

    [Fact]
    public void CorrectOutcome_Should_Throw_When_StunningResult_Is_Null()
    {
        var check = RehydrateConfirmed(
            UserId,
            RecordedAt,
            ValidFailedStunningResult()).Value;

        Action act = () => check.CorrectOutcome(
            null!,
            UserId,
            ModifiedAt);

        act.Should().Throw<ArgumentNullException>();
    }


    [Fact]
    public void Rehydrate_Should_Accept_Valid_Created_State()
    {
        var result = RehydrateCreated(
            recordedByUserId: null,
            recordedAt: null,
            stunningResult: null);

        result.IsSuccess.Should().BeTrue();

        result.Value.Status.Should().Be(StunningCheckStatus.Created);
        result.Value.StunningResult.Should().BeNull();
        result.Value.RecordedByUserId.Should().BeNull();
        result.Value.RecordedAt.Should().BeNull();
    }

    [Fact]
    public void Rehydrate_Should_Reject_Created_State_With_StunningResult()
    {
        var result = RehydrateCreated(
            recordedByUserId: null,
            recordedAt: null,
            stunningResult: ValidFailedStunningResult());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.StunningResultIsNotAllowedError);
    }

    [Fact]
    public void Rehydrate_Should_Reject_Created_State_With_RecordedByUserId()
    {
        var result = RehydrateCreated(
            recordedByUserId: UserId,
            recordedAt: null,
            stunningResult: null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.RecordedByUserIdIsNotAllowedError);
    }

    [Fact]
    public void Rehydrate_Should_Reject_Created_State_With_RecordedAt()
    {
        var result = RehydrateCreated(
            recordedByUserId: null,
            recordedAt: RecordedAt,
            stunningResult: null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.RecordedAtIsNotAllowedError);
    }

    [Fact]
    public void Rehydrate_Should_Accept_Valid_Confirmed_State()
    {
        var stunningResult = ValidFailedStunningResult();

        var result = RehydrateConfirmed(
            UserId,
            RecordedAt,
            stunningResult);

        result.IsSuccess.Should().BeTrue();

        result.Value.Status.Should().Be(StunningCheckStatus.Confirmed);
        result.Value.StunningResult.Should().BeSameAs(stunningResult);
        result.Value.RecordedByUserId.Should().Be(UserId);
        result.Value.RecordedAt.Should().Be(RecordedAt);
    }

    [Fact]
    public void Rehydrate_Should_Reject_Confirmed_State_Without_StunningResult()
    {
        var result = RehydrateConfirmed(
            UserId,
            RecordedAt,
            null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.StunningResultIsRequiredError);
    }

    [Fact]
    public void Rehydrate_Should_Reject_Confirmed_State_Without_RecordedByUserId()
    {
        var result = RehydrateConfirmed(
            null,
            RecordedAt,
            ValidFailedStunningResult());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.RecordedByUserIdIsRequiredError);
    }

    [Fact]
    public void Rehydrate_Should_Reject_Confirmed_State_With_Empty_RecordedByUserId()
    {
        var result = RehydrateConfirmed(
            Guid.Empty,
            RecordedAt,
            ValidFailedStunningResult());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.RecordedByUserIdIsRequiredError);
    }

    [Fact]
    public void Rehydrate_Should_Reject_Confirmed_State_Without_RecordedAt()
    {
        var result = RehydrateConfirmed(
            UserId,
            null,
            ValidFailedStunningResult());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.RecordedAtIsRequiredError);
    }

    [Fact]
    public void Rehydrate_Should_Reject_Invalid_Status()
    {
        var result = StunningCheck.Rehydrate(
            CheckId,
            ValidAnimal(),
            InitialDeviceId,
            CreatedAt,
            null,
            null,
            null,
            null,
            null,
            (StunningCheckStatus)(-1),
            false);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.StunningCheckStatusIsInvalidError);
    }

    [Fact]
    public void Rehydrate_Should_Reject_Empty_Id()
    {
        var result = StunningCheck.Rehydrate(
            Guid.Empty,
            ValidAnimal(),
            InitialDeviceId,
            CreatedAt,
            null,
            null,
            null,
            null,
            null,
            StunningCheckStatus.Created,
            false);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.InvalidIdError);
    }

    [Fact]
    public void Rehydrate_Should_Reject_Incomplete_Modification_Audit()
    {
        var missingModifiedAt = RehydrateConfirmed(
            UserId,
            RecordedAt,
            ValidFailedStunningResult(),
            modifiedByUserId: UserId,
            modifiedAt: null);

        missingModifiedAt.IsFailure.Should().BeTrue();
        missingModifiedAt.Error.Should().Be(
            StunningCheckErrors.ModifiedInfoIsIncompleteError);

        var missingModifiedBy = RehydrateConfirmed(
            UserId,
            RecordedAt,
            ValidFailedStunningResult(),
            modifiedByUserId: null,
            modifiedAt: ModifiedAt);

        missingModifiedBy.IsFailure.Should().BeTrue();
        missingModifiedBy.Error.Should().Be(
            StunningCheckErrors.ModifiedInfoIsIncompleteError);
    }

    [Fact]
    public void Rehydrate_Should_Reject_Empty_ModifiedByUserId()
    {
        var result = RehydrateConfirmed(
            UserId,
            RecordedAt,
            ValidFailedStunningResult(),
            modifiedByUserId: Guid.Empty,
            modifiedAt: ModifiedAt);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.ModifiedByUserIdIsRequiredError);
    }

    [Fact]
    public void Rehydrate_Should_Reject_RecordedAt_Before_CreatedAt()
    {
        var result = RehydrateConfirmed(
            UserId,
            CreatedAt.AddTicks(-1),
            ValidFailedStunningResult());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.RecordedAtCannotBeBeforeCreatedAtError);
    }

    [Fact]
    public void Rehydrate_Should_Reject_ModifiedAt_Before_CreatedAt()
    {
        var result = RehydrateConfirmed(
            UserId,
            RecordedAt,
            ValidFailedStunningResult(),
            modifiedByUserId: UserId,
            modifiedAt: CreatedAt.AddTicks(-1));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.ModifiedAtCannotBeBeforeCreatedAtError);
    }

    [Fact]
    public void Rehydrate_Should_Require_Modification_Audit_For_Deleted_Check()
    {
        var result = RehydrateConfirmed(
            UserId,
            RecordedAt,
            ValidFailedStunningResult(),
            modifiedByUserId: null,
            modifiedAt: null,
            isDeleted: true);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(
            StunningCheckErrors.ModifiedInfoIsRequiredForDeletedCheckError);
    }


    [Fact]
    public void MarkAsDeleted_Should_Set_Delete_State_And_Audit()
    {
        var check = ValidCreatedCheck();

        var result = check.MarkAsDeleted(
            UserId,
            ModifiedAt);

        result.IsSuccess.Should().BeTrue();

        check.IsDeleted.Should().BeTrue();
        check.ModifiedByUserId.Should().Be(UserId);
        check.ModifiedAt.Should().Be(ModifiedAt);
    }

    [Fact]
    public void MarkAsDeleted_Should_Be_Idempotent()
    {
        var check = ValidCreatedCheck();

        check.MarkAsDeleted(
            UserId,
            ModifiedAt).IsSuccess.Should().BeTrue();

        check.MarkAsDeleted(
            Guid.Empty,
            ModifiedAt.AddDays(1)).IsSuccess.Should().BeTrue();

        check.IsDeleted.Should().BeTrue();

        // The original deletion audit must remain unchanged.
        check.ModifiedByUserId.Should().Be(UserId);
        check.ModifiedAt.Should().Be(ModifiedAt);
    }


    private static StunningCheck ValidCreatedCheck()
    {
        return StunningCheck.Create(
            ValidAnimal(),
            InitialDeviceId,
            CreatedAt).Value;
    }

    private static Animal ValidAnimal()
    {
        return Animal.Create(
            AnimalKind.Schwein,
            null,
            null).Value;
    }

    private static StunningResult ValidSuccessfulStunningResult()
    {
        return StunningResult.Create(
            StunningOutcome.Successful,
            [],
            []).Value;
    }

    private static StunningResult ValidFailedStunningResult()
    {
        var correctiveAction = CorrectiveStunningAction.Create(
            CorrectiveDeviceId,
            CorrectiveStunningTiming.BeforeBleeding).Value;

        return StunningResult.Create(
            StunningOutcome.Failed,
            [StunningFailureIndicator.Gasping],
            [correctiveAction]).Value;
    }

    private static Result<StunningCheck> RehydrateCreated(
        Guid? recordedByUserId,
        DateTimeOffset? recordedAt,
        StunningResult? stunningResult,
        Guid? modifiedByUserId = null,
        DateTimeOffset? modifiedAt = null,
        bool isDeleted = false)
    {
        return StunningCheck.Rehydrate(
            CheckId,
            ValidAnimal(),
            InitialDeviceId,
            CreatedAt,
            recordedByUserId,
            recordedAt,
            modifiedByUserId,
            modifiedAt,
            stunningResult,
            StunningCheckStatus.Created,
            isDeleted);
    }

    private static Result<StunningCheck> RehydrateConfirmed(
        Guid? recordedByUserId,
        DateTimeOffset? recordedAt,
        StunningResult? stunningResult,
        Guid? modifiedByUserId = null,
        DateTimeOffset? modifiedAt = null,
        bool isDeleted = false)
    {
        return StunningCheck.Rehydrate(
            CheckId,
            ValidAnimal(),
            InitialDeviceId,
            CreatedAt,
            recordedByUserId,
            recordedAt,
            modifiedByUserId,
            modifiedAt,
            stunningResult,
            StunningCheckStatus.Confirmed,
            isDeleted);
    }
}