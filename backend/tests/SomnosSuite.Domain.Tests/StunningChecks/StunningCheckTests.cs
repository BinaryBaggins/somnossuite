using FluentAssertions;
using SomnosSuite.Domain.Animals;
using SomnosSuite.Domain.StunningChecks;
using Xunit;

namespace SomnosSuite.Domain.Tests.StunningChecks;

public sealed class StunningCheckTests
{
    private static readonly Guid CheckId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid InitialDeviceId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private static readonly Guid RestunningDeviceId = Guid.Parse("55555555-5555-5555-5555-555555555555");
    private static readonly Guid UserId = Guid.Parse("66666666-6666-6666-6666-666666666666");
    private static readonly DateTimeOffset CreatedAt = new(2026, 5, 1, 8, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset RecordedAt = new(2026, 5, 1, 9, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset ModifiedAt = new(2026, 5, 1, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_Should_Reject_Empty_Initial_Device_Id()
    {
        StunningCheck.Create(Animal(), Guid.Empty, CreatedAt).Error
            .Should().Be(StunningCheckErrors.InitialStunningDeviceIdIsRequiredError);
    }

    [Fact]
    public void RecordOutcome_Should_Enforce_Successful_And_Failed_Outcome_Rules()
    {
        var successful = ValidCreatedCheck();

        successful.RecordOutcome(
            StunningOutcome.Successful,
            StunningFailureIndicator.Reflex,
            null,
            UserId,
            null,
            RecordedAt).Error.Should().Be(StunningCheckErrors.FailureIndicatorIsNotAllowedError);

        var failed = ValidCreatedCheck();

        failed.RecordOutcome(
            StunningOutcome.Failed,
            null,
            CorrectiveStunningTiming.BeforeBleeding,
            UserId,
            RestunningDeviceId,
            RecordedAt).Error.Should().Be(StunningCheckErrors.FailureIndicatorIsRequiredError);

        failed.RecordOutcome(
            StunningOutcome.Failed,
            StunningFailureIndicator.Reflex,
            CorrectiveStunningTiming.BeforeBleeding,
            UserId,
            RestunningDeviceId,
            RecordedAt).IsSuccess.Should().BeTrue();

        failed.Status.Should().Be(StunningCheckStatus.Confirmed);
        failed.Outcome.Should().Be(StunningOutcome.Failed);
    }

    [Fact]
    public void RecordOutcome_Should_Reject_Deleted_Confirmed_And_Invalid_Chronology()
    {
        var deleted = RehydrateCreated(isDeleted: true, modifiedByUserId: UserId, modifiedAt: ModifiedAt).Value;
        deleted.RecordOutcome(StunningOutcome.Successful, null, null, UserId, null, RecordedAt).Error
            .Should().Be(StunningCheckErrors.StunningCheckIsDeletedError);

        var confirmed = RehydrateConfirmed().Value;
        confirmed.RecordOutcome(StunningOutcome.Successful, null, null, UserId, null, RecordedAt).Error
            .Should().Be(StunningCheckErrors.RecordedChecksCannotBeRecordedAgainError);

        var created = ValidCreatedCheck();
        created.RecordOutcome(StunningOutcome.Successful, null, null, UserId, null, CreatedAt.AddTicks(-1)).Error
            .Should().Be(StunningCheckErrors.RecordedAtCannotBeBeforeCreatedAtError);
    }

    [Fact]
    public void CorrectOutcome_Should_Work_Only_For_Confirmed_Checks_And_Require_Audit()
    {
        var created = ValidCreatedCheck();
        created.CorrectOutcome(StunningOutcome.Successful, null, null, null, UserId, ModifiedAt).Error
            .Should().Be(StunningCheckErrors.ConfirmedCheckIsRequiredForCorrectionError);

        var confirmed = RehydrateConfirmed().Value;
        confirmed.CorrectOutcome(StunningOutcome.Successful, null, null, null, Guid.Empty, ModifiedAt).Error
            .Should().Be(StunningCheckErrors.ModifiedByUserIdIsRequiredError);

        confirmed.CorrectOutcome(StunningOutcome.Successful, null, null, null, UserId, ModifiedAt).IsSuccess.Should().BeTrue();
        confirmed.Outcome.Should().Be(StunningOutcome.Successful);
        confirmed.ModifiedByUserId.Should().Be(UserId);
        confirmed.ModifiedAt.Should().Be(ModifiedAt);
    }

    [Fact]
    public void Rehydrate_Should_Reject_Invalid_Lifecycle_Audit_Deleted_State_And_Chronology()
    {
        RehydrateCreated(outcome: StunningOutcome.Successful).Error
            .Should().Be(StunningCheckErrors.OutcomeIsNotAllowedError);

        RehydrateConfirmed(modifiedByUserId: UserId, modifiedAt: null).Error
            .Should().Be(StunningCheckErrors.ModifiedInfoIsIncompleteError);

        RehydrateCreated(isDeleted: true, modifiedByUserId: null, modifiedAt: null).Error
            .Should().Be(StunningCheckErrors.ModifiedInfoIsRequiredForDeletedCheckError);

        RehydrateConfirmed(recordedAt: CreatedAt.AddTicks(-1)).Error
            .Should().Be(StunningCheckErrors.RecordedAtCannotBeBeforeCreatedAtError);
    }

    [Fact]
    public void MarkAsDeleted_Should_Be_Idempotent()
    {
        var check = ValidCreatedCheck();

        check.MarkAsDeleted(UserId, ModifiedAt).IsSuccess.Should().BeTrue();
        check.MarkAsDeleted(Guid.Empty, ModifiedAt.AddDays(1)).IsSuccess.Should().BeTrue();

        check.IsDeleted.Should().BeTrue();
        check.ModifiedByUserId.Should().Be(UserId);
    }

    private static StunningCheck ValidCreatedCheck()
    {
        return RehydrateCreated().Value;
    }

    private static Animal Animal()
    {
        return SomnosSuite.Domain.Animals.Animal.Create(AnimalKind.Schwein, null, null).Value;
    }

    private static SomnosSuite.Domain.SharedKernel.Result<StunningCheck> RehydrateCreated(
        Guid? modifiedByUserId = null,
        DateTimeOffset? modifiedAt = null,
        StunningOutcome? outcome = null,
        bool isDeleted = false)
    {
        return StunningCheck.Rehydrate(
            CheckId,
            Animal(),
            InitialDeviceId,
            CreatedAt,
            null,
            null,
            modifiedByUserId,
            modifiedAt,
            outcome,
            null,
            null,
            null,
            StunningCheckStatus.Created,
            isDeleted);
    }

    private static SomnosSuite.Domain.SharedKernel.Result<StunningCheck> RehydrateConfirmed(
        Guid? modifiedByUserId = null,
        DateTimeOffset? modifiedAt = null,
        DateTimeOffset? recordedAt = null)
    {
        return StunningCheck.Rehydrate(
            CheckId,
            Animal(),
            InitialDeviceId,
            CreatedAt,
            UserId,
            recordedAt ?? RecordedAt,
            modifiedByUserId,
            modifiedAt,
            StunningOutcome.Failed,
            StunningFailureIndicator.Reflex,
            CorrectiveStunningTiming.BeforeBleeding,
            RestunningDeviceId,
            StunningCheckStatus.Confirmed,
            false);
    }
}
