using SomnosSuite.Domain.Animals;
using SomnosSuite.Domain.SharedKernel;

namespace SomnosSuite.Domain.StunningDevices
{
    public sealed class StunningDevice : BaseEntity, IAggregateRoot
    {
        public StunningDeviceType DeviceType { get; private set; }
        public string Manufacturer { get; private set; } = null!;
        public string SerialNumber { get; private set; } = null!;
        public string Model { get; private set; } = null!;
        public AnimalCategory AnimalCategory { get; private set; }
        public DateOnly? LastInspectionDate { get; private set; }
        public Guid? ModifiedByUserId { get; private set; }
        public DateTimeOffset? ModifiedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private StunningDevice() { } //EF Core

        private StunningDevice(
            Guid id,
            StunningDeviceType deviceType,
            string manufacturer,
            string serialNumber,
            string model,
            AnimalCategory animalCategory,
            DateOnly? lastInspectionDate)
            : base(id)
        {
            DeviceType = deviceType;
            Manufacturer = manufacturer;
            SerialNumber = serialNumber;
            Model = model;
            AnimalCategory = animalCategory;
            LastInspectionDate = lastInspectionDate;
            IsDeleted = false;
        }

        private StunningDevice(
            Guid id,
            StunningDeviceType deviceType,
            string manufacturer,
            string serialNumber,
            string model,
            AnimalCategory animalCategory,
            DateOnly? lastInspectionDate,
            Guid? modifiedByUserId,
            DateTimeOffset? modifiedAt,
            bool isDeleted)
        : base(id)
        {
            DeviceType = deviceType;
            Manufacturer = manufacturer;
            SerialNumber = serialNumber;
            Model = model;
            AnimalCategory = animalCategory;
            LastInspectionDate = lastInspectionDate;
            ModifiedByUserId = modifiedByUserId;
            ModifiedAt = modifiedAt;
            IsDeleted = isDeleted;
        }


        public static Result<StunningDevice> Create(
            Guid id,
            StunningDeviceType deviceType,
            string? manufacturer,
            string? serialNumber,
            string? model,
            AnimalCategory animalCategory,
            DateOnly? lastInspectionDate,
            DateOnly today)
        {
            if (id == Guid.Empty)
                return Result<StunningDevice>.Failure(
                    StunningDeviceErrors.InvalidIdError);

            var validation = Validate(
                deviceType,
                manufacturer,
                serialNumber,
                model,
                animalCategory,
                lastInspectionDate,
                today,
                out var trimmedManufacturer,
                out var trimmedSerialNumber,
                out var trimmedModel);

            if (validation.IsFailure)
                return Result<StunningDevice>.Failure(validation.Error);

            return new StunningDevice(
                id,
                deviceType,
                trimmedManufacturer,
                trimmedSerialNumber,
                trimmedModel,
                animalCategory,
                lastInspectionDate);
        }

        public static Result<StunningDevice> Rehydrate(
            Guid id,
            StunningDeviceType deviceType,
            string? manufacturer,
            string? serialNumber,
            string? model,
            AnimalCategory animalCategory,
            DateOnly? lastInspectionDate,
            Guid? modifiedByUserId,
            DateTimeOffset? modifiedAt,
            DateOnly today,
            bool isDeleted)
        {
            if (id == Guid.Empty)
                return Result<StunningDevice>.Failure(StunningDeviceErrors.InvalidIdError);

            var modifiedInfoValidation = ValidateModifiedInfo(modifiedByUserId, modifiedAt, isDeleted);
            if (modifiedInfoValidation.IsFailure)
                return Result<StunningDevice>.Failure(modifiedInfoValidation.Error);

            var validation = Validate(
                deviceType,
                manufacturer,
                serialNumber,
                model,
                animalCategory,
                lastInspectionDate,
                today,
                out var trimmedManufacturer,
                out var trimmedSerialNumber,
                out var trimmedModel);

            if (validation.IsFailure)
                return Result<StunningDevice>.Failure(validation.Error);

            return new StunningDevice(
                id,
                deviceType,
                trimmedManufacturer,
                trimmedSerialNumber,
                trimmedModel,
                animalCategory,
                lastInspectionDate,
                modifiedByUserId,
                modifiedAt,
                isDeleted);
        }

        private static Result Validate(
            StunningDeviceType deviceType,
            string? manufacturer,
            string? serialNumber,
            string? model,
            AnimalCategory animalCategory,
            DateOnly? lastInspectionDate,
            DateOnly today,
            out string trimmedManufacturer,
            out string trimmedSerialNumber,
            out string trimmedModel)
        {
            trimmedManufacturer = manufacturer?.Trim() ?? string.Empty;
            trimmedSerialNumber = serialNumber?.Trim() ?? string.Empty;
            trimmedModel = model?.Trim() ?? string.Empty;

            if (!Enum.IsDefined(deviceType))
                return Result.Failure(StunningDeviceErrors.StunningDeviceTypeIsInvalidError);

            if (!Enum.IsDefined(animalCategory))
                return Result.Failure(StunningDeviceErrors.AnimalCategoryIsInvalidError);

            if (string.IsNullOrWhiteSpace(trimmedManufacturer))
                return Result.Failure(StunningDeviceErrors.ManufacturerIsRequiredError);

            if (string.IsNullOrWhiteSpace(trimmedSerialNumber))
                return Result.Failure(StunningDeviceErrors.SerialNumberIsRequiredError);

            if (string.IsNullOrWhiteSpace(trimmedModel))
                return Result.Failure(StunningDeviceErrors.ModelIsRequiredError);

            if (lastInspectionDate > today)
                return Result.Failure(StunningDeviceErrors.LastInspectionDateCannotBeInFutureError);

            return Result.Success();
        }

        private static Result ValidateModifiedInfo(Guid? modifiedByUserId, DateTimeOffset? modifiedAt, bool isDeleted)
        {
            if (modifiedByUserId.HasValue != modifiedAt.HasValue)
                return Result.Failure(StunningDeviceErrors.ModifiedInfoIsIncompleteError);

            if (modifiedByUserId == Guid.Empty)
                return Result.Failure(StunningDeviceErrors.ModifiedByUserIdIsRequiredError);

            if (isDeleted && (!modifiedByUserId.HasValue || !modifiedAt.HasValue))
                return Result.Failure(StunningDeviceErrors.ModifiedInfoIsRequiredForDeletedDeviceError);

            return Result.Success();
        }

        public Result RecordInspection(
            DateOnly inspectionDate,
            DateOnly today,
            Guid modifiedByUserId,
            DateTimeOffset modifiedAt)
        {
            if (IsDeleted)
                return Result.Failure(StunningDeviceErrors.StunningDeviceIsDeletedError);

            if (inspectionDate > today)
                return Result.Failure(StunningDeviceErrors.LastInspectionDateCannotBeInFutureError);

            if (LastInspectionDate.HasValue && inspectionDate < LastInspectionDate.Value)
                return Result.Failure(StunningDeviceErrors.NewLastInspectionDateCannotBeOlderThanCurrentLastInspectionDateError);

            var modifiedInfoResult = UpdateModifiedInfo(modifiedByUserId, modifiedAt);
            if (modifiedInfoResult.IsFailure)
                return modifiedInfoResult;

            LastInspectionDate = inspectionDate;

            return Result.Success();
        }

        private Result UpdateModifiedInfo(Guid modifiedByUserId, DateTimeOffset modifiedAt)
        {
            if (modifiedByUserId == Guid.Empty)
                return Result.Failure(StunningDeviceErrors.ModifiedByUserIdIsRequiredError);

            ModifiedByUserId = modifiedByUserId;
            ModifiedAt = modifiedAt;
            return Result.Success();
        }

        public Result MarkAsDeleted(Guid modifiedByUserId, DateTimeOffset modifiedAt)
        {
            if (IsDeleted)
                return Result.Success();

            var modifiedInfoResult = UpdateModifiedInfo(modifiedByUserId, modifiedAt);
            if (modifiedInfoResult.IsFailure)
                return modifiedInfoResult;

            IsDeleted = true;

            return Result.Success();
        }
    }
}
