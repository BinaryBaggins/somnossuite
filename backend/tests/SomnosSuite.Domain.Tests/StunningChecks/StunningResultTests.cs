using FluentAssertions;
using SomnosSuite.Domain.StunningChecks;
using Xunit;
namespace SomnosSuite.Domain.Tests.StunningChecks
{
    public class StunningResultTests
    {
        private static readonly CorrectiveStunningAction[] CorrectiveActions =
        [
            CorrectiveStunningAction.Create(Guid.NewGuid(), CorrectiveStunningTiming.BeforeBleeding).Value,
            CorrectiveStunningAction.Create(Guid.NewGuid(), CorrectiveStunningTiming.AfterBleeding).Value
        ];

        [Fact]
        public void Create_Should_Work_For_Successful_Outcome_With_Valid_Inputs()
        {
            var result = StunningResult.Create(StunningOutcome.Successful, [], []);
            result.IsSuccess.Should().BeTrue();
            result.Value.Outcome.Should().Be(StunningOutcome.Successful);
            result.Value.FailureIndicators.Should().BeEmpty();
            result.Value.CorrectiveActions.Should().BeEmpty();
        }

        [Fact]
        public void Create_Should_Fail_For_Successful_Outcome_With_Provided_FailureIndicators()
        {
            var result = StunningResult.Create(StunningOutcome.Successful, [StunningFailureIndicator.Gasping], []);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(StunningResultErrors.FailureIndicatorIsNotAllowedError);
        }

        [Fact]
        public void Create_Should_Fail_For_Successful_Outcome_With_Provided_CorrectiveActions()
        {
            var result = StunningResult.Create(StunningOutcome.Successful, [], CorrectiveActions);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(StunningResultErrors.CorrectiveActionIsNotAllowedError);
        }

        [Fact]
        public void Create_Should_Work_For_Failed_Outcome_With_Valid_Single_Indicator_And_Single_CorrectiveAction()
        {
            var result = StunningResult.Create(StunningOutcome.Failed, [StunningFailureIndicator.Gasping], [CorrectiveActions[0]]);
            result.IsSuccess.Should().BeTrue();
            result.Value.Outcome.Should().Be(StunningOutcome.Failed);
            result.Value.FailureIndicators.Should().BeEquivalentTo([StunningFailureIndicator.Gasping]);
            result.Value.CorrectiveActions.Should().BeEquivalentTo([CorrectiveActions[0]]);
        }

        [Fact]
        public void Create_Should_Work_For_Failed_Outcome_With_Multiple_Valid_Indicators_And_Multiple_Valid_CorrectiveActions()
        {
            var result = StunningResult.Create(StunningOutcome.Failed, [StunningFailureIndicator.Gasping, StunningFailureIndicator.Reflex], CorrectiveActions);
            result.IsSuccess.Should().BeTrue();
            result.Value.Outcome.Should().Be(StunningOutcome.Failed);
            result.Value.FailureIndicators.Should().BeEquivalentTo([StunningFailureIndicator.Gasping, StunningFailureIndicator.Reflex]);
            result.Value.CorrectiveActions.Should().BeEquivalentTo([CorrectiveActions[0], CorrectiveActions[1]]);
        }

        [Fact]
        public void Create_Should_Fail_For_Failed_Outcome_Without_FailureIndicators()
        {
            var result = StunningResult.Create(StunningOutcome.Failed, [], CorrectiveActions);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(StunningResultErrors.FailureIndicatorIsRequiredError);
        }

        [Fact]
        public void Create_Should_Fail_For_Failed_Outcome_Without_CorrectiveActions()
        {
            var result = StunningResult.Create(StunningOutcome.Failed, [StunningFailureIndicator.Gasping], []);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(StunningResultErrors.CorrectiveActionIsRequiredError);
        }

        [Fact]
        public void Create_Should_Fail_For_Failed_Outcome_With_Invalid_FailureIndicator()
        {
            var result = StunningResult.Create(StunningOutcome.Failed, [(StunningFailureIndicator)(-1)], CorrectiveActions);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(StunningResultErrors.FailureIndicatorIsInvalidError);
        }

        [Fact]
        public void Create_Should_Fail_For_Failed_Outcome_With_Duplicate_FailureIndicators()
        {
            var result = StunningResult.Create(StunningOutcome.Failed, [StunningFailureIndicator.Gasping, StunningFailureIndicator.Gasping], CorrectiveActions);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(StunningResultErrors.FailureIndicatorsAreDuplicateError);
        }

        [Fact]
        public void Create_Should_Fail_With_Invalid_Outcome()
        {
            var result = StunningResult.Create((StunningOutcome)(-1), [], []);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(StunningResultErrors.StunningOutcomeIsInvalidError);
        }
    }
}