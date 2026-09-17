using SomnosSuite.Domain.Animals;
using SomnosSuite.Domain.StunningDevices;

namespace SomnosSuite.Presentation.StunningDevices
{
    public sealed record CreateStunningDeviceRequestDto(
    StunningDeviceType DeviceType,
    string? Manufacturer,
    string? SerialNumber,
    string? Model,
    AnimalCategory AnimalCategory,
    DateOnly? LastInspectionDate);
}