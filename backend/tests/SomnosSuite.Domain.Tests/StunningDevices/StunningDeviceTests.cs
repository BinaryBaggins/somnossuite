using FluentAssertions;
using SomnosSuite.Domain.Animals;
using SomnosSuite.Domain.SharedKernel;
using SomnosSuite.Domain.StunningDevices;
using Xunit;

namespace SomnosSuite.Domain.Tests.StunningDevices;

public sealed class StunningDeviceTests
{
    private static readonly Guid DeviceId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid ModifierId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly DateTimeOffset ModifiedAt = new(2026, 5, 2, 8, 0, 0, TimeSpan.Zero);
    private static readonly DateOnly Today = new(2026, 5, 28);

    [Fact]
    public void Create_Should_Validate_Required_Strings_Enums_And_Future_Inspection_Date()
    {
        Create(manufacturer: " ").Error.Should().Be(StunningDeviceErrors.ManufacturerIsRequiredError);
        Create(deviceType: (StunningDeviceType)999).Error.Should().Be(StunningDeviceErrors.StunningDeviceTypeIsInvalidError);
        Create(animalCategory: (AnimalCategory)999).Error.Should().Be(StunningDeviceErrors.AnimalCategoryIsInvalidError);
        Create(lastInspectionDate: Today.AddDays(1)).Error.Should().Be(StunningDeviceErrors.LastInspectionDateCannotBeInFutureError);
    }

    [Fact]
    public void Create_WithValidInput_ShouldUseProvidedId()
    {
        var id = Guid.NewGuid();

        var result = Create(id: id);

        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
    }

    [Fact]
    public void Create_WithEmptyId_ShouldFail()
    {
        var result = Create(id: Guid.Empty);

        result.Error.Should().Be(StunningDeviceErrors.InvalidIdError);
    }

    [Fact]
    public void Create_Should_Trim_Strings()
    {
        var result = Create(manufacturer: "  Acme  ", serialNumber: "  SN-1  ", model: "  M1  ");

        result.IsSuccess.Should().BeTrue();
        result.Value.Manufacturer.Should().Be("Acme");
        result.Value.SerialNumber.Should().Be("SN-1");
        result.Value.Model.Should().Be("M1");
    }

    [Fact]
    public void Rehydrate_Should_Preserve_Deleted_State_And_Require_Audit_For_Deleted_Device()
    {
        Rehydrate(isDeleted: true, modifiedByUserId: null, modifiedAt: null).Error
            .Should().Be(StunningDeviceErrors.ModifiedInfoIsRequiredForDeletedDeviceError);

        var result = Rehydrate(isDeleted: true, modifiedByUserId: ModifierId, modifiedAt: ModifiedAt);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void RecordInspection_Should_Reject_Deleted_Future_And_Older_Dates_And_Set_Audit()
    {
        var deleted = Rehydrate(isDeleted: true, modifiedByUserId: ModifierId, modifiedAt: ModifiedAt).Value;
        deleted.RecordInspection(Today, Today, ModifierId, ModifiedAt).Error
            .Should().Be(StunningDeviceErrors.StunningDeviceIsDeletedError);

        var device = Rehydrate(lastInspectionDate: Today.AddDays(-1)).Value;
        device.RecordInspection(Today.AddDays(1), Today, ModifierId, ModifiedAt).Error
            .Should().Be(StunningDeviceErrors.LastInspectionDateCannotBeInFutureError);

        device.RecordInspection(Today.AddDays(-2), Today, ModifierId, ModifiedAt).Error
            .Should().Be(StunningDeviceErrors.NewLastInspectionDateCannotBeOlderThanCurrentLastInspectionDateError);

        device.RecordInspection(Today, Today, ModifierId, ModifiedAt).IsSuccess.Should().BeTrue();
        device.LastInspectionDate.Should().Be(Today);
        device.ModifiedByUserId.Should().Be(ModifierId);
        device.ModifiedAt.Should().Be(ModifiedAt);
    }

    [Fact]
    public void MarkAsDeleted_Should_Be_Idempotent()
    {
        var device = Rehydrate().Value;

        device.MarkAsDeleted(ModifierId, ModifiedAt).IsSuccess.Should().BeTrue();
        device.MarkAsDeleted(Guid.Empty, ModifiedAt.AddDays(1)).IsSuccess.Should().BeTrue();

        device.IsDeleted.Should().BeTrue();
        device.ModifiedByUserId.Should().Be(ModifierId);
    }

    private static Result<StunningDevice> Create(
    Guid? id = null,
    StunningDeviceType deviceType = StunningDeviceType.CaptiveBolt,
    string? manufacturer = "Acme",
    string? serialNumber = "SN-1",
    string? model = "M1",
    AnimalCategory animalCategory = AnimalCategory.Grossvieh,
    DateOnly? lastInspectionDate = null)
    {
        return StunningDevice.Create(
            id ?? DeviceId,
            deviceType,
            manufacturer,
            serialNumber,
            model,
            animalCategory,
            lastInspectionDate,
            Today);
    }

    private static Result<StunningDevice> Rehydrate(
        DateOnly? lastInspectionDate = null,
        Guid? modifiedByUserId = null,
        DateTimeOffset? modifiedAt = null,
        bool isDeleted = false)
    {
        return StunningDevice.Rehydrate(
            DeviceId,
            StunningDeviceType.CaptiveBolt,
            "Acme",
            "SN-1",
            "M1",
            AnimalCategory.Grossvieh,
            lastInspectionDate,
            modifiedByUserId,
            modifiedAt,
            Today,
            isDeleted);
    }
}
