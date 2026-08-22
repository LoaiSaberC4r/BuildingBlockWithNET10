using AcademyOps.Application.Email;
using AcademyOps.Application.Persistence;
using AcademyOps.Infrastructure.EntityFrameworkCore.SqlServer.Email;
using AcademyOps.Infrastructure.EntityFrameworkCore.SqlServer.Options;
using AcademyOps.Infrastructure.EntityFrameworkCore.SqlServer.Persistence;
using BuildingBlock.Infrastructure.Bootstrap;
using BuildingBlock.Infrastructure.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AcademyOps.Infrastructure.EntityFrameworkCore.SqlServer;

public static class DependencyInjection
{
    public static IServiceCollection AddAcademyOpsSqlServerPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException(
                "Connection string 'Database' is not configured.");
        var migrationsAssemblyName = typeof(AcademyOpsDbContext)
            .Assembly
            .GetName()
            .Name
            ?? throw new InvalidOperationException(
                "Unable to resolve the migrations assembly name.");

        services.AddBuildingBlockEntityFrameworkCore<AcademyOpsWritePersistence>();
        services.AddBuildingBlockInterceptors();
        services.AddBuildingBlockSqlServerExceptionMapping();

        services.AddOptions<DatabaseInitializationOptions>()
            .Bind(configuration.GetSection(DatabaseInitializationOptions.SectionName));
        services.AddOptions<EmailOutboxOptions>()
            .Bind(configuration.GetSection(EmailOutboxOptions.SectionName))
            .Validate(
                options => options.PollingIntervalSeconds > 0 &&
                           options.BatchSize is > 0 and <= 100 &&
                           options.MaxAttempts is > 0 and <= 20 &&
                           options.ClaimLeaseSeconds >= 120,
                "Email outbox options are invalid.")
            .ValidateOnStart();

        services.AddScoped<IEmailOutbox, EmailOutbox>();
        services.AddScoped<EmailOutboxProcessor>();
        services.AddSingleton<IDatabaseMigrationService, EfCoreDatabaseMigrationService>();
        services.AddHostedService<DatabaseInitializationHostedService>();
        services.AddHostedService<EmailOutboxBackgroundService>();

        services.AddDbContext<AcademyOpsDbContext>((serviceProvider, options) =>
            options
                .UseSqlServer(
                    connectionString,
                    sql => sql.MigrationsAssembly(migrationsAssemblyName))
                .UseBuildingBlockInterceptors(serviceProvider));

        services.AddBuildingBlockDbContext<AcademyOpsReadPersistence, AcademyOpsDbContext>();
        services.AddBuildingBlockDbContext<AcademyOpsWritePersistence, AcademyOpsDbContext>();

        return services;
    }
}
