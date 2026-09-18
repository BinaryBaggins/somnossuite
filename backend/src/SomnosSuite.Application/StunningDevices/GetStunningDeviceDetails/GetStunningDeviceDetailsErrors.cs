using SomnosSuite.Domain.SharedKernel;

namespace SomnosSuite.Application.StunningDevices.GetStunningDeviceDetails;

public static class GetStunningDeviceDetailsErrors
{
    public static readonly Error NotFound = new(
        "StunningDevice.NotFound",
        "The stunning device was not found.");
}