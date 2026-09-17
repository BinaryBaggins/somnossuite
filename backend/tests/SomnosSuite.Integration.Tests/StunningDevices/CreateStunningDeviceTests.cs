using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Data.SqlClient;

namespace SomnosSuite.Integration.Tests.StunningDevices;

public sealed class CreateStunningDeviceTests(
    IntegrationTestFixture fixture)
    : IClassFixture<IntegrationTestFixture>
{
    [Fact]
    public async Task Create_WithValidRequest_PersistsDevice()
    {
        var serialNumber = $"INT-{Guid.NewGuid():N}";

        var request = new
        {
            deviceType = 0,
            manufacturer = "Somnos Integration Test",
            serialNumber,
            model = "Test Device",
            animalCategory = 0,
            lastInspectionDate = "2026-09-01"
        };

        var response = await fixture.Client.PostAsJsonAsync(
            "/api/stunning-devices",
            request);

        response.StatusCode.Should()
            .Be(HttpStatusCode.Created);

        await using var connection =
            new SqlConnection(fixture.ConnectionString);

        await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT COUNT(*)
            FROM stunning_devices
            WHERE serial_number = @SerialNumber;
            """;

        command.Parameters.AddWithValue(
            "@SerialNumber",
            serialNumber);

        var count = await CountDevicesBySerialNumber(serialNumber);

        count.Should().Be(1);
    }

    [Fact]
    public async Task Create_WithDuplicateSerialNumber_ReturnsBadRequest()
    {
        var serialNumber = $"INT-{Guid.NewGuid():N}";

        var request = new
        {
            deviceType = 0,
            manufacturer = "Somnos Integration Test",
            serialNumber,
            model = "Test Device",
            animalCategory = 0,
            lastInspectionDate = "2026-09-01"
        };

        var firstResponse = await fixture.Client.PostAsJsonAsync(
            "/api/stunning-devices",
            request);

        firstResponse.StatusCode.Should()
            .Be(HttpStatusCode.Created);

        var secondResponse = await fixture.Client.PostAsJsonAsync(
            "/api/stunning-devices",
            request);

        secondResponse.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);

        var error =
            await secondResponse.Content
                .ReadFromJsonAsync<ErrorResponse>();

        error.Should().NotBeNull();

        error!.Code.Should()
            .Be("StunningDevice.SerialNumberAlreadyExists");

        var count = await CountDevicesBySerialNumber(serialNumber);

        count.Should().Be(1);
    }

    [Fact]
    public async Task Create_WithMissingManufacturer_ReturnsBadRequest()
    {
        var serialNumber = $"INT-{Guid.NewGuid():N}";

        var request = new
        {
            deviceType = 0,
            manufacturer = "",
            serialNumber,
            model = "Test Device",
            animalCategory = 0,
            lastInspectionDate = "2026-09-01"
        };

        var response = await fixture.Client.PostAsJsonAsync(
            "/api/stunning-devices",
            request);

        response.StatusCode.Should()
            .Be(HttpStatusCode.BadRequest);

        var error =
            await response.Content
                .ReadFromJsonAsync<ErrorResponse>();

        error.Should().NotBeNull();

        error!.Code.Should()
            .Be("StunningDevice.ManufacturerIsRequired");

        var count = await CountDevicesBySerialNumber(serialNumber);

        count.Should().Be(0);
    }

    private async Task<int> CountDevicesBySerialNumber(
    string serialNumber)
    {
        await using var connection =
            new SqlConnection(fixture.ConnectionString);

        await connection.OpenAsync();

        await using var command =
            connection.CreateCommand();

        command.CommandText = """
        SELECT COUNT(*)
        FROM stunning_devices
        WHERE serial_number = @SerialNumber;
        """;

        command.Parameters.AddWithValue(
            "@SerialNumber",
            serialNumber);

        return Convert.ToInt32(
            await command.ExecuteScalarAsync());
    }

    private sealed record ErrorResponse(
        string Code,
        string? Message);
}