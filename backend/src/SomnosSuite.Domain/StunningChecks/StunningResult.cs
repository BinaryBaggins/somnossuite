using SomnosSuite.Domain.SharedKernel;
namespace SomnosSuite.Domain.StunningChecks
{
    public sealed class StunningResult : IValueObject
    {
        public StunningOutcome Outcome { get; }
        public IReadOnlyCollection<StunningFailureIndicator> FailureIndicators { get; }
        public IReadOnlyCollection<CorrectiveStunningAction> CorrectiveActions { get; }

        private StunningResult(StunningOutcome outcome, IReadOnlyCollection<StunningFailureIndicator> failureIndicators, IReadOnlyCollection<CorrectiveStunningAction> correctiveActions)
        {
            Outcome = outcome;
            FailureIndicators = failureIndicators;
            CorrectiveActions = correctiveActions;
        }

        public static Result<StunningResult> Create(StunningOutcome outcome, IEnumerable<StunningFailureIndicator> failureIndicators, IEnumerable<CorrectiveStunningAction> correctiveActions)
        {

            var indicators = failureIndicators.ToArray();
            var actions = correctiveActions.ToArray();

            var validationResult = ValidateOutcomeRules(
                outcome,
                indicators.FirstOrDefault(),
                actions.FirstOrDefault()?.Timing,
                actions.FirstOrDefault()?.DeviceId);

            if (validationResult.IsFailure)
                return Result<StunningResult>.Failure(validationResult.Error);

            return new StunningResult(outcome, indicators, actions);
        }

        private static Result ValidateOutcomeRules(
            StunningOutcome outcome,
            StunningFailureIndicator? failureIndicator,
            RestunningTiming? restunningTiming,
            Guid? restunningDeviceId)
        {
            if (!Enum.IsDefined(outcome))
                return Result.Failure(StunningResultErrors.StunningOutcomeIsInvalidError);

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

                _ => Result.Failure(StunningResultErrors.StunningOutcomeIsInvalidError)
            };
        }

        private static Result ValidateSuccessfulOutcome(
            StunningFailureIndicator? failureIndicator,
            RestunningTiming? restunningTiming,
            Guid? restunningDeviceId)
        {
            if (failureIndicator.HasValue)
                return Result.Failure(StunningResultErrors.FailureIndicatorIsNotAllowedError);

            if (restunningTiming.HasValue)
                return Result.Failure(StunningResultErrors.RestunningTimingIsNotAllowedError);

            if (restunningDeviceId.HasValue)
                return Result.Failure(StunningResultErrors.RestunningDeviceIdIsNotAllowedError);

            return Result.Success();
        }

        private static Result ValidateFailedOutcome(
            StunningFailureIndicator? failureIndicator,
            RestunningTiming? restunningTiming,
            Guid? restunningDeviceId)
        {
            if (!failureIndicator.HasValue)
                return Result.Failure(StunningResultErrors.FailureIndicatorIsRequiredError);

            if (!restunningTiming.HasValue)
                return Result.Failure(StunningResultErrors.RestunningTimingIsRequiredError);

            if (!restunningDeviceId.HasValue || restunningDeviceId.Value == Guid.Empty)
                return Result.Failure(StunningResultErrors.RestunningDeviceIdIsRequiredError);

            return Result.Success();
        }
    }

    internal class StunningResultErrors
    {
        public static readonly Error StunningOutcomeIsInvalidError = new(
            "StunningResultErrors.StunningOutcomeIsInvalid",
            "Stunning outcome is invalid.");
        public static readonly Error FailureIndicatorIsNotAllowedError = new(
            "StunningResultErrors.FailureIndicatorIsNotAllowed",
            "Failure indicator is not allowed for successful outcome.");
        public static readonly Error RestunningTimingIsNotAllowedError = new(
            "StunningResultErrors.RestunningTimingIsNotAllowed",
            "Restunning timing is not allowed for successful outcome.");
        public static readonly Error RestunningDeviceIdIsNotAllowedError = new(
            "StunningResultErrors.RestunningDeviceIdIsNotAllowed",
            "Restunning device ID is not allowed for successful outcome.");
        public static readonly Error FailureIndicatorIsRequiredError = new(
            "StunningResultErrors.FailureIndicatorIsRequired",
            "Failure indicator is required for failed outcome.");
        public static readonly Error RestunningTimingIsRequiredError = new(
            "StunningResultErrors.RestunningTimingIsRequired",
            "Restunning timing is required for failed outcome.");
        public static readonly Error RestunningDeviceIdIsRequiredError = new(
            "StunningResultErrors.RestunningDeviceIdIsRequired",
            "Restunning device ID is required for failed outcome.");
    }
}