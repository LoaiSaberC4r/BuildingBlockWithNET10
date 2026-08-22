using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AcademyOps.Infrastructure.EntityFrameworkCore.SqlServer.Persistence;

public interface IDatabaseMigrationService
{
    IReadOnlyList<string> GetCompiledMigrations(AcademyOpsDbContext dbContext);

    Task<IReadOnlyList<string>> GetPendingMigrationsAsync(
        AcademyOpsDbContext dbContext,
        CancellationToken cancellationToken = default);

    Task MigrateAsync(
        AcademyOpsDbContext dbContext,
        CancellationToken cancellationToken = default);
}

internal sealed class EfCoreDatabaseMigrationService : IDatabaseMigrationService
{
    public IReadOnlyList<string> GetCompiledMigrations(AcademyOpsDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        return dbContext.Database.GetMigrations().ToArray();
    }

    public async Task<IReadOnlyList<string>> GetPendingMigrationsAsync(
        AcademyOpsDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        var migrations = await dbContext.Database
            .GetPendingMigrationsAsync(cancellationToken);
        return migrations.ToArray();
    }

    public Task MigrateAsync(
        AcademyOpsDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        return dbContext.Database.MigrateAsync(cancellationToken);
    }
}

internal readonly record struct DatabaseConnectionDetails(
    string DataSource,
    string Database,
    bool IntegratedSecurity)
{
    public static DatabaseConnectionDetails From(AcademyOpsDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        var connection = dbContext.Database.GetDbConnection();
        if (connection is not SqlConnection sqlConnection)
        {
            return new DatabaseConnectionDetails(
                connection.DataSource,
                connection.Database,
                IntegratedSecurity: false);
        }

        var builder = new SqlConnectionStringBuilder(sqlConnection.ConnectionString);
        return new DatabaseConnectionDetails(
            builder.DataSource,
            builder.InitialCatalog,
            builder.IntegratedSecurity);
    }
}
