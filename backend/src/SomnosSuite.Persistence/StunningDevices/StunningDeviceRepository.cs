using Dapper;
using SomnosSuite.Application;
using SomnosSuite.Application.StunningDevices;
using SomnosSuite.Domain.StunningDevices;

namespace SomnosSuite.Persistence.StunningDevices;

internal sealed class StunningDeviceRepository(
    ISqlConnectionFactory connectionFactory)
    : IStunningDeviceRepository
{
    public async Task<bool> SerialNumberExistsAsync(
    string serialNumber,
    CancellationToken cancellationToken)
    {
        const string sql = """
        SELECT CASE
            WHEN EXISTS (
                SELECT 1
                FROM stunning_devices
                WHERE serial_number = @SerialNumber
            )
            THEN CAST(1 AS bit)
            ELSE CAST(0 AS bit)
        END;
        """;

        await using var connection =
            await connectionFactory.OpenConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            new { SerialNumber = serialNumber },
            cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task InsertAsync(
        StunningDevice stunningDevice,
        CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO stunning_devices
            (
                id,
                device_type,
                manufacturer,
                serial_number,
                model,
                animal_category,
                last_inspection_date,
                modified_by_user_id,
                modified_at,
                is_deleted
            )
            VALUES
            (
                @Id,
                @DeviceType,
                @Manufacturer,
                @SerialNumber,
                @Model,
                @AnimalCategory,
                @LastInspectionDate,
                @ModifiedByUserId,
                @ModifiedAt,
                @IsDeleted
            );
            """;

        await using var connection =
            await connectionFactory.OpenConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            new
            {
                stunningDevice.Id,
                DeviceType = (int)stunningDevice.DeviceType,
                stunningDevice.Manufacturer,
                stunningDevice.SerialNumber,
                stunningDevice.Model,
                AnimalCategory = (int)stunningDevice.AnimalCategory,
                LastInspectionDate =
                    stunningDevice.LastInspectionDate?
                    .ToDateTime(TimeOnly.MinValue),
                stunningDevice.ModifiedByUserId,
                stunningDevice.ModifiedAt,
                stunningDevice.IsDeleted
            },
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
}
