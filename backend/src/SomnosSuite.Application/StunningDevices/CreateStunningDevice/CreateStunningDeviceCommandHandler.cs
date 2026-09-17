using SomnosSuite.Application.Abstractions;
using SomnosSuite.Domain.SharedKernel;
using SomnosSuite.Domain.StunningDevices;

namespace SomnosSuite.Application.StunningDevices.CreateStunningDevice;

public sealed class CreateStunningDeviceCommandHandler(
    IStunningDeviceRepository repository,
    IClock clock)
    : ICommandHandler<CreateStunningDeviceCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateStunningDeviceCommand command,
        CancellationToken cancellationToken)
    {
        var createResult = StunningDevice.Create(
            Guid.NewGuid(),
            command.DeviceType,
            command.Manufacturer,
            command.SerialNumber,
            command.Model,
            command.AnimalCategory,
            command.LastInspectionDate,
            clock.CurrentDate);

        if (createResult.IsFailure)
            return Result<Guid>.Failure(createResult.Error);

        var device = createResult.Value;

        if (await repository.SerialNumberExistsAsync(
                device.SerialNumber,
                cancellationToken))
        {
            return Result<Guid>.Failure(
                CreateStunningDeviceErrors.SerialNumberAlreadyExists);
        }

        await repository.AddAsync(device, cancellationToken);

        return device.Id;
    }
}
