using SomnosSuite.Domain.SharedKernel;

namespace SomnosSuite.Domain.Animals
{
    public class AnimalErrors
    {
        public static readonly Error AnimalKindIsInvalidError = new(
            "Animal.AnimalKindIsInvalid",
            "Animal category must be a defined value.");
        public static readonly Error EarTagNumberIsRequiredForGrossviehError = new(
            "Animal.EarTagIsRequiredForGrossvieh",
            "Ear tag number is required for Grossvieh.");

        public static readonly Error SupplierNameIsRequiredForGrossviehError = new(
            "Animal.SupplierNameIsRequiredForGrossvieh",
            "Supplier name is required for Grossvieh."
        );

        public static readonly Error EarTagNumberIsRequiredForKalbError = new(
            "Animal.EarTagIsRequiredForKalb",
            "Ear tag number is required for Kalb.");

        public static readonly Error SupplierNameIsRequiredForKalbError = new(
            "Animal.SupplierNameIsRequiredForKalb",
            "Supplier name is required for Kalb."
        );
    }
}