using SomnosSuite.Domain.SharedKernel;

namespace SomnosSuite.Domain.StunningChecks
{
    public sealed record CorrectiveStunningAction : IValueObject
    {
        public Guid DeviceId { get; }
        public RestunningTiming Timing { get; }

        private CorrectiveStunningAction(Guid deviceId, RestunningTiming timing)
        {
            DeviceId = deviceId;
            Timing = timing;
        }
        public static Result<CorrectiveStunningAction> Create(Guid deviceId, RestunningTiming timing)
        {
            if (deviceId == Guid.Empty)
                return Result<CorrectiveStunningAction>.Failure(
                    CorrectiveStunningActionErrors.DeviceIdIsRequiredError);

            if (!Enum.IsDefined(timing))
                return Result<CorrectiveStunningAction>.Failure(
                    CorrectiveStunningActionErrors.TimingIsInvalidError);
            return Result<CorrectiveStunningAction>.Success(new CorrectiveStunningAction(deviceId, timing));
        }

    }
    internal class CorrectiveStunningActionErrors
    {
        public static readonly Error DeviceIdIsRequiredError = new("CorrectiveStunningActionErrors.DeviceIdIsRequired", "DeviceId is required.");
        public static readonly Error TimingIsInvalidError = new("CorrectiveStunningActionErrors.TimingIsInvalid", "Timing is invalid.");
    }
}