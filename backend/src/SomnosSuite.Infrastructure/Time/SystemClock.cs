using SomnosSuite.Application.Abstractions;

namespace SomnosSuite.Infrastructure.Time
{
    internal sealed class SystemClock : IClock
    {
        public DateOnly CurrentDate =>
            DateOnly.FromDateTime(
                TimeProvider.System.GetLocalNow().DateTime);
    }
}