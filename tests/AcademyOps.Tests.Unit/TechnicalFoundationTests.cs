using AcademyOps.Application.Email;
using AcademyOps.Application.Persistence;
using AcademyOps.Infrastructure.EntityFrameworkCore.SqlServer.Options;
using BuildingBlock.Application.Abstraction.Persistence;

namespace AcademyOps.Tests.Unit;

public sealed class TechnicalFoundationTests
{
    [Fact]
    public void PersistenceMarkers_UseBuildingBlockContracts()
    {
        Assert.True(typeof(IReadPersistenceMarker)
            .IsAssignableFrom(typeof(AcademyOpsReadPersistence)));
        Assert.True(typeof(IWritePersistenceMarker)
            .IsAssignableFrom(typeof(AcademyOpsWritePersistence)));
    }

    [Fact]
    public void EmailOutboxContract_IsDomainNeutral()
    {
        var message = new QueueEmailMessage(
            "technical-test-1",
            "recipient@example.test",
            "Technical test",
            "<p>Technical test</p>",
            "Technical test");

        Assert.Equal("technical-test-1", message.IdempotencyKey);
        Assert.Equal("recipient@example.test", message.RecipientEmail);
        Assert.DoesNotContain(
            message.GetType().GetProperties(),
            property => property.Name.Contains(
                "Aggregate",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DatabaseMigrations_AreOptInByDefault()
    {
        var options = new DatabaseInitializationOptions();

        Assert.False(options.ApplyMigrationsOnStartup);
    }
}
