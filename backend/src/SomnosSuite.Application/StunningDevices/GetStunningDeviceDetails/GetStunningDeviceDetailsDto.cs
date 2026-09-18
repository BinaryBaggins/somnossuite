using SomnosSuite.Domain.Animals;
using SomnosSuite.Domain.StunningDevices;

namespace SomnosSuite.Application.StunningDevices.GetStunningDeviceDetails
{
    public sealed record GetStunningDeviceDetailsDto(
        Guid Id,
        StunningDeviceType DeviceType,
        string Model,
        string SerialNumber,
        string Manufacturer,
        AnimalCategory AnimalCategory,
        DateOnly? LastInspectionDate);
}
