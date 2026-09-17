namespace SomnosSuite.Application.Abstractions
{
    public interface IClock
    {
        DateOnly CurrentDate { get; }
    }
}
