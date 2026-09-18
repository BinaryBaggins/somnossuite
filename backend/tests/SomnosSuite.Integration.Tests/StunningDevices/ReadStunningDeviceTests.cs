using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace SomnosSuite.Integration.Tests.StunningDevices;

public sealed class ReadStunningDeviceTests(
    IntegrationTestFixture fixture)
    : IClassFixture<IntegrationTestFixture>
{
    [Fact]
    public async Task GetAll_ReturnsCreatedDevices()
    {
        var first = await CreateDevice(
            model: "Read Test A");

        var second = await CreateDevice(
            model: "Read Test B");

        var response = await fixture.Client.GetAsync(
            "/api/stunning-devices");

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var devices = await response.Content
            .ReadFromJsonAsync<List<DeviceListItemResponse>>();

        devices.Should().NotBeNull();

        devices!.Should().Contain(
            device => device.Id == first.Id);

        devices.Should().Contain(
            device => device.Id == second.Id);
    }

    [Fact]
    public async Task GetById_WithExistingDevice_ReturnsDevice()
    {
        var created = await CreateDevice(
            model: "Details Test");

        var response = await fixture.Client.GetAsync(
            $"/api/stunning-devices/{created.Id}");

        response.StatusCode.Should()
            .Be(HttpStatusCode.OK);

        var device = await response.Content
            .ReadFromJsonAsync<DeviceDetailsResponse>();

        device.Should().NotBeNull();

        device!.Id.Should().Be(created.Id);
        device.DeviceType.Should().Be(0);
        device.Manufacturer.Should()
            .Be("Somnos Integration Test");
        device.SerialNumber.Should()
            .Be(created.SerialNumber);
        device.Model.Should().Be("Details Test");
        device.AnimalCategory.Should().Be(0);
        device.LastInspectionDate.Should()
            .Be(new DateOnly(2026, 9, 1));
    }

    [Fact]
    public async Task GetById_WithUnknownDevice_ReturnsNotFound()
    {
        var id = Guid.NewGuid();

        var response = await fixture.Client.GetAsync(
            $"/api/stunning-devices/{id}");

        response.StatusCode.Should()
            .Be(HttpStatusCode.NotFound);

        var error = await response.Content
            .ReadFromJsonAsync<ErrorResponse>();

        error.Should().NotBeNull();

        error!.Code.Should()
            .Be("StunningDevice.NotFound");
    }

    private async Task<CreatedDevice> CreateDevice(
        string model)
    {
        var serialNumber =
            $"READ-{Guid.NewGuid():N}";

        var request = new
        {
            deviceType = 0,
            manufacturer = "Somnos Integration Test",
            serialNumber,
            model,
            animalCategory = 0,
            lastInspectionDate = "2026-09-01"
        };

        var response = await fixture.Client.PostAsJsonAsync(
            "/api/stunning-devices",
            request);

        response.StatusCode.Should()
            .Be(HttpStatusCode.Created);

        var created = await response.Content
            .ReadFromJsonAsync<CreateResponse>();

        created.Should().NotBeNull();

        return new CreatedDevice(
            created!.Id,
            serialNumber);
    }

    private sealed record CreateResponse(Guid Id);

    private sealed record CreatedDevice(
        Guid Id,
        string SerialNumber);

    private sealed record DeviceListItemResponse(
        Guid Id,
        int DeviceType,
        string Model,
        string SerialNumber,
        int AnimalCategory);

    private sealed record DeviceDetailsResponse(
        Guid Id,
        int DeviceType,
        string Model,
        string SerialNumber,
        string Manufacturer,
        int AnimalCategory,
        DateOnly? LastInspectionDate);

    private sealed record ErrorResponse(
        string Code,
        string? Message);
}