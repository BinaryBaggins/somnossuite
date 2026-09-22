using FluentAssertions;
using SomnosSuite.Domain.Animals;
using SomnosSuite.Domain.StunningChecks;
using Xunit;

namespace SomnosSuite.Domain.Tests.StunningChecks;

public sealed class StunningCheckTests
{
    private static readonly Animal ValidAnimal = Animal.Create(AnimalKind.Schwein, null, "Lieferant").Value;
    private static readonly CorrectiveStunningAction ValidCorrectiveStunningAction = CorrectiveStunningAction.Create(Guid.NewGuid(), CorrectiveStunningTiming.BeforeBleeding).Value;
    private static readonly StunningResult ValidStunningResult = StunningResult.Create(StunningOutcome.Failed, [StunningFailureIndicator.Gasping], [ValidCorrectiveStunningAction]).Value;


}
