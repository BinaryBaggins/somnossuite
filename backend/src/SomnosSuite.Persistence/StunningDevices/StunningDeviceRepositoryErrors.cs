using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SomnosSuite.Domain.SharedKernel;

namespace SomnosSuite.Persistence.StunningDevices
{
    public static class StunningDeviceRepositoryErrors
    {
        public static readonly Error NotFound = new(
            "StunningDevice.NotFound",
            "The stunning device was not found.");
    }
}