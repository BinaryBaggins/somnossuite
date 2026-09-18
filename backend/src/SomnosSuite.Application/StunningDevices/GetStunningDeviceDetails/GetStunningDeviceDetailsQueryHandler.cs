using Dapper;
using SomnosSuite.Application.Abstractions;
using SomnosSuite.Domain.Animals;
using SomnosSuite.Domain.SharedKernel;
using SomnosSuite.Domain.StunningDevices;

namespace SomnosSuite.Application.StunningDevices.GetStunningDeviceDetails;

public sealed class GetStunningDeviceDetailsQueryHandler(
    ISqlConnectionFactory connectionFactory)
    : IQueryHandler<
        GetStunningDeviceDetailsQuery,
        Result<GetStunningDeviceDetailsDto>>
{
    public async Task<Result<GetStunningDeviceDetailsDto>> Handle(
        GetStunningDeviceDetailsQuery request,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                id AS Id,
                device_type AS DeviceType,
                manufacturer AS Manufacturer,
                serial_number AS SerialNumber,
                model AS Model,
                animal_category AS AnimalCategory,
                last_inspection_date AS LastInspectionDate
            FROM stunning_devices
            WHERE id = @Id
              AND is_deleted = 0;
            """;

        await using var connection =
            await connectionFactory.OpenConnectionAsync(
                cancellationToken);

        var command = new CommandDefinition(
            sql,
            new { request.Id },
            cancellationToken: cancellationToken);

        var row = await connection
            .QuerySingleOrDefaultAsync<StunningDeviceDetailsRow>(
                command);

        if (row is null)
        {
            return Result<GetStunningDeviceDetailsDto>.Failure(
                GetStunningDeviceDetailsErrors.NotFound);
        }

        return new GetStunningDeviceDetailsDto(
            row.Id,
            (StunningDeviceType)row.DeviceType,
            row.Model,
            row.SerialNumber,
            row.Manufacturer,
            (AnimalCategory)row.AnimalCategory,
            row.LastInspectionDate.HasValue
                ? DateOnly.FromDateTime(
                    row.LastInspectionDate.Value)
                : null);
    }

    private sealed record StunningDeviceDetailsRow(
       Guid Id,
       int DeviceType,
       string Manufacturer,
       string SerialNumber,
       string Model,
       int AnimalCategory,
       DateTime? LastInspectionDate);
}