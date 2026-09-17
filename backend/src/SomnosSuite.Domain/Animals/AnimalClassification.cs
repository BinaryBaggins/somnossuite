namespace SomnosSuite.Domain.Animals
{
    public static class AnimalClassification
    {
        public static AnimalCategory GetCategory(AnimalKind kind) =>
            kind switch
            {
                AnimalKind.Schwein => AnimalCategory.Kleinvieh,
                AnimalKind.Kalb => AnimalCategory.Kleinvieh,

                AnimalKind.Rind => AnimalCategory.Grossvieh,
                AnimalKind.Kuh => AnimalCategory.Grossvieh,
                AnimalKind.Muni => AnimalCategory.Grossvieh,
                AnimalKind.Ochse => AnimalCategory.Grossvieh,

                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
            };
    }
}