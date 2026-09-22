using SomnosSuite.Domain.StunningChecks;
using FluentAssertions;
using Xunit;

namespace SomnosSuite.Domain.Tests.StunningChecks
{
    public class CorrectiveStunningActionTest
    {
        private static readonly Guid ValidDeviceId = Guid.NewGuid();
        private static readonly CorrectiveStunningTiming ValidTiming = CorrectiveStunningTiming.BeforeBleeding;

        [Fact]
        public void Create_Should_Work_With_Valid_Parameters()
        {
            var result = CorrectiveStunningAction.Create(ValidDeviceId, ValidTiming);
            result.IsSuccess.Should().BeTrue();
            result.Value.DeviceId.Should().Be(ValidDeviceId);
            result.Value.Timing.Should().Be(ValidTiming);
        }

        [Fact]
        public void Create_Should_Reject_Empty_Device_Id()
        {
            var result = CorrectiveStunningAction.Create(Guid.Empty, ValidTiming);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(CorrectiveStunningActionErrors.DeviceIdIsRequiredError);
        }

        [Fact]
        public void Create_Should_Reject_Invalid_Timing()
        {
            var result = CorrectiveStunningAction.Create(ValidDeviceId, (CorrectiveStunningTiming)(-1));
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(CorrectiveStunningActionErrors.TimingIsInvalidError);
        }
    }
}