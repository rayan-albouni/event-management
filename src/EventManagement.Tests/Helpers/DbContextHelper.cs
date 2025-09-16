using EventManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Tests.Helpers;

public static class DbContextHelper
{
    public static EventManagementDbContext CreateInMemoryContext(string databaseName = "")
    {
        if (string.IsNullOrEmpty(databaseName))
        {
            databaseName = Guid.NewGuid().ToString();
        }

        var options = new DbContextOptionsBuilder<EventManagementDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new EventManagementDbContext(options);
    }

    public static void SeedTestData(EventManagementDbContext context)
    {
        context.SaveChanges();
    }
}
