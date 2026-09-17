using SomnosSuite.Domain.SharedKernel;

namespace SomnosSuite.Application.StunningDevices.CreateStunningDevice;

public static class CreateStunningDeviceErrors
{
    public static readonly Error SerialNumberAlreadyExists =
        new(
            "StunningDevice.SerialNumberAlreadyExists",
            "A stunning device with this serial number already exists.");
}
