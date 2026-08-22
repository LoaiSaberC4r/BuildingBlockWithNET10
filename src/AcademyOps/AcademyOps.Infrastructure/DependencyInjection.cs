using BuildingBlock.Infrastructure.Bootstrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AcademyOps.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAcademyOpsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddBuildingBlockCaching();
        services.AddBuildingBlockMailKitEmail(configuration);

        return services;
    }
}
