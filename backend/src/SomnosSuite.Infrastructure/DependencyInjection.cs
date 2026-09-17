using Microsoft.Extensions.DependencyInjection;
using SomnosSuite.Application.Abstractions;
using SomnosSuite.Infrastructure.Time;

namespace SomnosSuite.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
        {
            services.AddSingleton<IClock, SystemClock>();

            return services;
        }
    }
}
