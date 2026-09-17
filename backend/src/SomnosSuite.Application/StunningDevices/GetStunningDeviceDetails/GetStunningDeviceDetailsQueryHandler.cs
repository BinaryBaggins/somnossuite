using Dapper;
using SomnosSuite.Application.Abstractions;

namespace SomnosSuite.Application.StunningDevices.GetStunningDeviceDetails;

public sealed class GetStunningDeviceDetailsQueryHandler(
    ISqlConnectionFactory connectionFactory)
    : IQueryHandler<
        GetStunningDeviceDetailsQuery,
        GetStunningDeviceDetailsDto>
{
    public async Task<GetStunningDeviceDetailsDto> Handle(
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
              AND is_deleted = FALSE;
            """;

        await using var connection =
            await connectionFactory.OpenConnectionAsync(
                cancellationToken);

        var command = new CommandDefinition(
            sql,
            new { request.Id },
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleAsync<GetStunningDeviceDetailsDto>(
                command);
    }
}