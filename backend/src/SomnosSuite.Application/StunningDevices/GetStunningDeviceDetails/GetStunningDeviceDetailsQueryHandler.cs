using Dapper;
using SomnosSuite.Application.Abstractions;
using SomnosSuite.Domain.SharedKernel;

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

        var device = await connection
            .QuerySingleOrDefaultAsync<GetStunningDeviceDetailsDto>(
                command);

        if (device is null)
        {
            return Result<GetStunningDeviceDetailsDto>.Failure(
                GetStunningDeviceDetailsErrors.NotFound);
        }

        return device;
    }
}