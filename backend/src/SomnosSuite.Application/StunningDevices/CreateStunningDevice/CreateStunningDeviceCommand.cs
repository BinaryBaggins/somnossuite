using SomnosSuite.Application.Abstractions;
using SomnosSuite.Domain.Animals;
using SomnosSuite.Domain.SharedKernel;
using SomnosSuite.Domain.StunningDevices;

namespace SomnosSuite.Application.StunningDevices.CreateStunningDevice;

public sealed record CreateStunningDeviceCommand(
    StunningDeviceType DeviceType,
    string? Manufacturer,
    string? SerialNumber,
    string? Model,
    AnimalCategory AnimalCategory,
    DateOnly? LastInspectionDate)
    : ICommand<Result<Guid>>;
