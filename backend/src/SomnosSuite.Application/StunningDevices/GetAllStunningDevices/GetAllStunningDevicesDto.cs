using SomnosSuite.Domain.Animals;
using SomnosSuite.Domain.StunningDevices;

namespace SomnosSuite.Application.StunningDevices.GetAllStunningDevices
{
    public sealed record GetAllStunningDevicesDto(
        Guid Id,
        StunningDeviceType DeviceType,
        string Model,
        string SerialNumber,
        AnimalCategory AnimalCategory);
}