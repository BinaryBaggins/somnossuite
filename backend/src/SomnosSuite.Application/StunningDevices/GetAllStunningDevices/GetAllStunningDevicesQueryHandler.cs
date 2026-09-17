using Dapper;
using SomnosSuite.Application.Abstractions;

namespace SomnosSuite.Application.StunningDevices.GetAllStunningDevices;

public sealed class GetAllStunningDevicesQueryHandler(
    ISqlConnectionFactory connectionFactory)
    : IQueryHandler<
        GetAllStunningDevicesQuery,
        List<GetAllStunningDevicesDto>>
{
    public async Task<List<GetAllStunningDevicesDto>> Handle(
        GetAllStunningDevicesQuery request,
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                id AS Id,
                device_type AS DeviceType,
                model AS Model,
                serial_number AS SerialNumber,
                animal_category AS AnimalCategory
            FROM stunning_devices
            WHERE is_deleted = FALSE
            ORDER BY model, serial_number;
            """;

        await using var connection =
            await connectionFactory.OpenConnectionAsync(
                cancellationToken);

        var command = new CommandDefinition(
            sql,
            cancellationToken: cancellationToken);

        var devices =
            await connection.QueryAsync<GetAllStunningDevicesDto>(
                command);

        return devices.AsList();
    }
}