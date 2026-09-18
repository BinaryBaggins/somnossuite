using SomnosSuite.Domain.StunningDevices;
using SomnosSuite.Domain.SharedKernel;
namespace SomnosSuite.Application.StunningDevices
{
    public interface IStunningDeviceRepository
    {
        Task<bool> SerialNumberExistsAsync(
        string serialNumber,
        CancellationToken cancellationToken);

        Task InsertAsync(
            StunningDevice stunningDevice,
            CancellationToken cancellationToken);

        Task<Result<StunningDevice?>> LoadByIdAsync(
            Guid id,
            DateOnly today,
            CancellationToken cancellationToken);

        Task UpdateAsync(
            StunningDevice stunningDevice,
            CancellationToken cancellationToken);

    }
}
