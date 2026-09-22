using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public static CorrectiveStunningAction Create(Guid deviceId, RestunningTiming timing)
        {
            // Add any necessary validation or business rules here before creating the instance
            return new CorrectiveStunningAction(deviceId, timing);
        }

    }
}