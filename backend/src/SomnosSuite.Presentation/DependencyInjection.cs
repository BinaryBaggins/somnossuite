using Microsoft.Extensions.DependencyInjection;

namespace SomnosSuite.Presentation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services
            .AddControllers()
            .AddApplicationPart(
                typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
