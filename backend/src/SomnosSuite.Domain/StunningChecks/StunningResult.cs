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
            ArgumentNullException.ThrowIfNull(failureIndicators);
            ArgumentNullException.ThrowIfNull(correctiveActions);

            var indicators = failureIndicators.ToArray();
            var actions = correctiveActions.ToArray();

            // Validate that the outcome is defined in the enum.
            if (!Enum.IsDefined(outcome))
                return Result<StunningResult>.Failure(
                    StunningResultErrors.StunningOutcomeIsInvalidError);

            // Validate that all failure indicators are defined in the enum.
            if (indicators.Any(indicator => !Enum.IsDefined(indicator)))
                return Result<StunningResult>.Failure(
                    StunningResultErrors.FailureIndicatorIsInvalidError);

            // Ensure that failure indicators are unique.
            if (indicators.Distinct().Count() != indicators.Length)
                return Result<StunningResult>.Failure(
                    StunningResultErrors.FailureIndicatorsAreDuplicateError);

            // Validate that all corrective actions are not null.
            if (actions.Any(action => action is null))
                return Result<StunningResult>.Failure(
                    StunningResultErrors.CorrectiveActionIsInvalidError);

            // Ensure that a successful outcome does not have any failure indicators and corrective actions.
            if (outcome == StunningOutcome.Successful)
            {
                if (indicators.Length > 0)
                    return Result<StunningResult>.Failure(
                        StunningResultErrors.FailureIndicatorIsNotAllowedError);

                if (actions.Length > 0)
                    return Result<StunningResult>.Failure(
                        StunningResultErrors.CorrectiveActionIsNotAllowedError);
            }

            // Ensure that a failed outcome has at least one failure indicator and corrective action.
            if (outcome == StunningOutcome.Failed)
            {
                if (indicators.Length == 0)
                    return Result<StunningResult>.Failure(
                        StunningResultErrors.FailureIndicatorIsRequiredError);

                if (actions.Length == 0)
                    return Result<StunningResult>.Failure(
                        StunningResultErrors.CorrectiveActionIsRequiredError);
            }

            // Construct with read-only collections for indicators and actions so that they cannot be modified externally.
            return new StunningResult(outcome, Array.AsReadOnly(indicators), Array.AsReadOnly(actions));
        }
    }

    public class StunningResultErrors
    {
        public static readonly Error StunningOutcomeIsInvalidError = new(
            "StunningResultErrors.StunningOutcomeIsInvalid",
            "Stunning outcome is invalid.");
        public static readonly Error FailureIndicatorIsInvalidError = new(
            "StunningResultErrors.FailureIndicatorIsInvalid",
            "Failure indicator is invalid.");
        public static readonly Error FailureIndicatorsAreDuplicateError = new(
            "StunningResultErrors.FailureIndicatorsAreDuplicate",
            "Failure indicators are duplicate.");

        public static readonly Error CorrectiveActionIsInvalidError = new(
            "StunningResultErrors.CorrectiveActionIsInvalid",
            "Corrective action is invalid.");
        public static readonly Error FailureIndicatorIsNotAllowedError = new(
            "StunningResultErrors.FailureIndicatorIsNotAllowed",
            "Failure indicator is not allowed for successful outcome.");
        public static readonly Error CorrectiveActionIsNotAllowedError = new(
            "StunningResultErrors.CorrectiveActionIsNotAllowed",
            "Corrective action is not allowed for successful outcome.");
        public static readonly Error FailureIndicatorIsRequiredError = new(
            "StunningResultErrors.FailureIndicatorIsRequired",
            "Failure indicator is required for failed outcome.");
        public static readonly Error CorrectiveActionIsRequiredError = new(
            "StunningResultErrors.CorrectiveActionIsRequired",
            "Corrective action is required for failed outcome.");

    }
}