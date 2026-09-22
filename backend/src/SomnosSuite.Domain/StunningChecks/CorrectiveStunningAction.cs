using SomnosSuite.Domain.SharedKernel;

namespace SomnosSuite.Domain.StunningChecks
{
    public sealed record CorrectiveStunningAction : IValueObject
    {
        public Guid DeviceId { get; }
        public CorrectiveStunningTiming Timing { get; }

        private CorrectiveStunningAction(Guid deviceId, CorrectiveStunningTiming timing)
        {
            DeviceId = deviceId;
            Timing = timing;
        }

        public static Result<CorrectiveStunningAction> Create(Guid deviceId, CorrectiveStunningTiming timing)
        {
            // Validate that the deviceId is not empty.
            if (deviceId == Guid.Empty)
                return Result<CorrectiveStunningAction>.Failure(
                    CorrectiveStunningActionErrors.DeviceIdIsRequiredError);

            // Validate that the timing is defined in the enum.
            if (!Enum.IsDefined(timing))
                return Result<CorrectiveStunningAction>.Failure(
                    CorrectiveStunningActionErrors.TimingIsInvalidError);
            return Result<CorrectiveStunningAction>.Success(new CorrectiveStunningAction(deviceId, timing));
        }

    }
    public class CorrectiveStunningActionErrors
    {
        public static readonly Error DeviceIdIsRequiredError = new(
            "CorrectiveStunningActionErrors.DeviceIdIsRequired",
            "DeviceId is required.");
        public static readonly Error TimingIsInvalidError = new(
            "CorrectiveStunningActionErrors.TimingIsInvalid",
            "Timing is invalid.");
    }
}