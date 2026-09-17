using FluentAssertions;
using SomnosSuite.Application.Abstractions;
using SomnosSuite.Application.StunningDevices;
using SomnosSuite.Application.StunningDevices.CreateStunningDevice;
using SomnosSuite.Domain.Animals;
using SomnosSuite.Domain.StunningDevices;

namespace SomnosSuite.Application.Tests.StunningDevices.CreateStunningDevice;

public sealed class CreateStunningDeviceCommandHandlerTests
{
    private static readonly DateOnly Today = new(2026, 9, 17);

    [Fact]
    public async Task Handle_WithValidCommand_ShouldInsertDeviceAndReturnId()
    {
        // Arrange
        var repository = new FakeStunningDeviceRepository();
        var clock = new FakeClock(Today);

        var handler = new CreateStunningDeviceCommandHandler(
            repository,
            clock);

        var command = CreateValidCommand();

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);

        repository.InsertedDevice.Should().NotBeNull();
        repository.InsertedDevice!.Id.Should().Be(result.Value);

        repository.InsertedDevice.DeviceType
            .Should().Be(command.DeviceType);

        repository.InsertedDevice.Manufacturer
            .Should().Be(command.Manufacturer);

        repository.InsertedDevice.SerialNumber
            .Should().Be(command.SerialNumber);

        repository.InsertedDevice.Model
            .Should().Be(command.Model);

        repository.InsertedDevice.AnimalCategory
            .Should().Be(command.AnimalCategory);

        repository.InsertedDevice.LastInspectionDate
            .Should().Be(command.LastInspectionDate);
    }

    [Fact]
    public async Task Handle_WithInvalidDomainData_ShouldReturnFailureAndNotUseRepository()
    {
        // Arrange
        var repository = new FakeStunningDeviceRepository();
        var clock = new FakeClock(Today);

        var handler = new CreateStunningDeviceCommandHandler(
            repository,
            clock);

        var command = CreateValidCommand() with
        {
            Manufacturer = " "
        };

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should()
            .Be(StunningDeviceErrors.ManufacturerIsRequiredError);

        repository.SerialNumberCheckCount.Should().Be(0);
        repository.InsertedDevice.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithExistingSerialNumber_ShouldReturnFailureAndNotInsert()
    {
        // Arrange
        var repository = new FakeStunningDeviceRepository
        {
            SerialNumberExists = true
        };

        var clock = new FakeClock(Today);

        var handler = new CreateStunningDeviceCommandHandler(
            repository,
            clock);

        var command = CreateValidCommand();

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();

        result.Error.Should()
            .Be(CreateStunningDeviceErrors.SerialNumberAlreadyExists);

        repository.SerialNumberCheckCount.Should().Be(1);
        repository.InsertedDevice.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldNormalizeSerialNumberBeforeUniquenessCheck()
    {
        // Arrange
        var repository = new FakeStunningDeviceRepository();
        var clock = new FakeClock(Today);

        var handler = new CreateStunningDeviceCommandHandler(
            repository,
            clock);

        var command = CreateValidCommand() with
        {
            SerialNumber = "  SN-123  "
        };

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        repository.CheckedSerialNumber
            .Should().Be("SN-123");

        repository.InsertedDevice.Should().NotBeNull();

        repository.InsertedDevice!.SerialNumber
            .Should().Be("SN-123");
    }

    private static CreateStunningDeviceCommand CreateValidCommand()
    {
        return new CreateStunningDeviceCommand(
            StunningDeviceType.CaptiveBolt,
            "Acme",
            "SN-123",
            "Model 1",
            AnimalCategory.Grossvieh,
            Today.AddDays(-1));
    }

    private sealed class FakeClock(DateOnly currentDate) : IClock
    {
        public DateOnly CurrentDate { get; } = currentDate;
    }

    private sealed class FakeStunningDeviceRepository
        : IStunningDeviceRepository
    {
        public bool SerialNumberExists { get; init; }

        public int SerialNumberCheckCount { get; private set; }

        public string? CheckedSerialNumber { get; private set; }

        public StunningDevice? InsertedDevice { get; private set; }

        public Task<bool> SerialNumberExistsAsync(
            string serialNumber,
            CancellationToken cancellationToken)
        {
            SerialNumberCheckCount++;
            CheckedSerialNumber = serialNumber;

            return Task.FromResult(SerialNumberExists);
        }

        public Task InsertAsync(
            StunningDevice stunningDevice,
            CancellationToken cancellationToken)
        {
            InsertedDevice = stunningDevice;

            return Task.CompletedTask;
        }
    }
}