namespace SomnosSuite.Application.StunningDevices.GetStunningDeviceDetails
{
    public record GetStunningDeviceDetailsDto(
        Guid Id,
        string DeviceType,
        string Model,
        string SerialNumber,
        string Manufacturer,
        string AnimalCategory,
        DateOnly? LastInspectionDate
    )
    {

    }
}
