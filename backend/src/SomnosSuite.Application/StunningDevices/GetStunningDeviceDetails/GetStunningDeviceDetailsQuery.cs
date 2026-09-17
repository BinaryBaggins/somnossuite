using SomnosSuite.Application.Abstractions;

namespace SomnosSuite.Application.StunningDevices.GetStunningDeviceDetails
{
    public record GetStunningDeviceDetailsQuery(Guid Id) : IQuery<GetStunningDeviceDetailsDto>;
}
