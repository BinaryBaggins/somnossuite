using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SomnosSuite.Domain.SharedKernel;
namespace SomnosSuite.Domain.StunningChecks
{
    public sealed record StunningResult : IValueObject
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

        public static StunningResult Create(StunningOutcome outcome, IReadOnlyCollection<StunningFailureIndicator> failureIndicators, IReadOnlyCollection<CorrectiveStunningAction> correctiveActions)
        {
            // Add any necessary validation or business rules here before creating the instance
            return new StunningResult(outcome, failureIndicators, correctiveActions);
        }

        public static StunningResult Create(StunningOutcome outcome, StunningFailureIndicator failureIndicator, CorrectiveStunningAction correctiveAction)
        {
            // Add any necessary validation or business rules here before creating the instance
            return new StunningResult(outcome, [failureIndicator], [correctiveAction]);
        }
    }
}