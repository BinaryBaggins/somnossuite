using SomnosSuite.Domain.StunningDevices;

namespace SomnosSuite.Application.StunningDevices
{
    public interface IStunningDeviceRepository
    {
        Task<bool> SerialNumberExistsAsync(
        string serialNumber,
        CancellationToken cancellationToken);

        Task AddAsync(
            StunningDevice stunningDevice,
            CancellationToken cancellationToken);
    }
}
