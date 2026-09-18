using Dapper;
using SomnosSuite.Application;
using SomnosSuite.Application.StunningDevices;
using SomnosSuite.Domain.Animals;
using SomnosSuite.Domain.SharedKernel;
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

    public async Task<Result<StunningDevice>> GetByIdAsync(Guid id, DateOnly today, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                id AS Id,
                device_type AS DeviceType,
                manufacturer AS Manufacturer,
                serial_number AS SerialNumber,
                model AS Model,
                animal_category AS AnimalCategory,
                last_inspection_date AS LastInspectionDate,
                modified_by_user_id AS ModifiedByUserId,
                modified_at AS ModifiedAt,
                is_deleted AS IsDeleted
            FROM stunning_devices
            WHERE id = @Id;
            """;

        await using var connection =
            await connectionFactory.OpenConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            new { Id = id },
            cancellationToken: cancellationToken);

        var row = await connection.QuerySingleOrDefaultAsync<StunningDeviceRow>(command);

        if (row is null)
        {
            return Result.Failure<StunningDevice>(StunningDeviceRepositoryErrors.NotFound);
        }

        return StunningDevice.Rehydrate(
            row.Id,
            (StunningDeviceType)row.DeviceType,
            row.Manufacturer,
            row.SerialNumber,
            row.Model,
            (AnimalCategory)row.AnimalCategory,
            row.LastInspectionDate.HasValue
                ? DateOnly.FromDateTime(
                    row.LastInspectionDate.Value)
                : null,
            row.ModifiedByUserId,
            row.ModifiedAt,
            today,
            row.IsDeleted);
    }

    public async Task UpdateAsync(
    StunningDevice stunningDevice,
    CancellationToken cancellationToken)
    {
        const string sql = """
        UPDATE stunning_devices
        SET
            last_inspection_date = @LastInspectionDate,
            modified_by_user_id = @ModifiedByUserId,
            modified_at = @ModifiedAt,
            is_deleted = @IsDeleted
        WHERE id = @Id;
        """;

        await using var connection =
            await connectionFactory.OpenConnectionAsync(
                cancellationToken);

        var command = new CommandDefinition(
            sql,
            new
            {
                stunningDevice.Id,
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

    private sealed record StunningDeviceRow(
        Guid Id,
        int DeviceType,
        string Manufacturer,
        string SerialNumber,
        string Model,
        int AnimalCategory,
        DateTime? LastInspectionDate,
        Guid? ModifiedByUserId,
        DateTimeOffset? ModifiedAt,
        bool IsDeleted);
}
