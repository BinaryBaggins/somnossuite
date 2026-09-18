using SomnosSuite.Application.Abstractions;
using SomnosSuite.Domain.SharedKernel;

namespace SomnosSuite.Application.StunningDevices.GetStunningDeviceDetails
{
    public sealed record GetStunningDeviceDetailsQuery(Guid Id) : IQuery<Result<GetStunningDeviceDetailsDto>>;


}
