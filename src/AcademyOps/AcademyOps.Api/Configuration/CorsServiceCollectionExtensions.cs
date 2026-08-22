using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace AcademyOps.Api.Configuration;

public static class CorsServiceCollectionExtensions
{
    public static IServiceCollection AddAcademyOpsCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<AcademyOpsCorsOptions>()
            .Bind(configuration.GetSection(AcademyOpsCorsOptions.SectionName))
            .ValidateOnStart();
        services.AddSingleton<
            IValidateOptions<AcademyOpsCorsOptions>,
            AcademyOpsCorsOptionsValidator>();
        services.AddCors();
        services.AddSingleton<
            IConfigureOptions<CorsOptions>,
            AcademyOpsCorsPolicyConfigurator>();

        return services;
    }
}

internal sealed class AcademyOpsCorsPolicyConfigurator
    : IConfigureOptions<CorsOptions>
{
    private readonly IOptions<AcademyOpsCorsOptions> _configuredOptions;

    public AcademyOpsCorsPolicyConfigurator(
        IOptions<AcademyOpsCorsOptions> configuredOptions)
    {
        ArgumentNullException.ThrowIfNull(configuredOptions);
        _configuredOptions = configuredOptions;
    }

    public void Configure(CorsOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var configuredOptions = _configuredOptions.Value;
        var allowedOrigins = (configuredOptions.AllowedOrigins ?? [])
            .Select(origin => origin.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        options.AddPolicy(CorsPolicyNames.Default, policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();

            if (configuredOptions.AllowAnyOrigin)
            {
                policy.AllowAnyOrigin();
            }
            else
            {
                policy.WithOrigins(allowedOrigins);
            }

            if (configuredOptions.AllowCredentials)
            {
                policy.AllowCredentials();
            }
        });
    }
}
