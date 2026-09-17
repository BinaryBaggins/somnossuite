using Dapper;
using SomnosSuite.Application.Abstractions;

namespace SomnosSuite.Application.StunningDevices.GetStunningDeviceDetails
{
    public class GetStunningDeviceDetailsQueryHandler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<GetStunningDeviceDetailsQuery, GetStunningDeviceDetailsDto>
    {
        public async Task<GetStunningDeviceDetailsDto> Handle(GetStunningDeviceDetailsQuery request, CancellationToken cancellationToken)
        {
            const string query = @"
            SELECT
                Id,
                DeviceType,
                Model,
                SerialNumber,
                Manufacturer,
                AnimalCategory,
                LastInspectionDate
            FROM StunningDevices
            WHERE IsDeleted = 0 AND Id = @Id";

            using var connection = sqlConnectionFactory.GetOpenConnection();
            return await connection.QuerySingleAsync<GetStunningDeviceDetailsDto>(query, new { Id = request.Id });
        }
    }
}
