using EventManagement.Domain.Entities;
using EventManagement.Domain.Enums;
using EventManagement.Infrastructure.Repositories;
using EventManagement.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace EventManagement.Tests.Infrastructure;

public class UnitOfWorkTests
{
    [Fact]
    public async Task SaveChangesAsync_ShouldPersistAllChanges()
    {
        using var context = DbContextHelper.CreateInMemoryContext();
        var unitOfWork = new UnitOfWork(context);

        var user = new User("test@example.com", "passwordHash", UserRole.EventCreator);
        var eventEntity = new Event(
            "Test Event",
            "Test description",
            "Test location",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(8),
            user.Id
        );

        await unitOfWork.Users.AddAsync(user);
        await unitOfWork.Events.AddAsync(eventEntity);
        var result = await unitOfWork.SaveChangesAsync();

        result.Should().BeGreaterThan(0);
        
        var savedUser = await unitOfWork.Users.GetByIdAsync(user.Id);
        var savedEvent = await unitOfWork.Events.GetByIdAsync(eventEntity.Id);
        
        savedUser.Should().NotBeNull();
        savedEvent.Should().NotBeNull();
    }

    [Fact]
    public void Repositories_ShouldBeInitialized()
    {
        using var context = DbContextHelper.CreateInMemoryContext();
        var unitOfWork = new UnitOfWork(context);

        unitOfWork.Users.Should().NotBeNull();
        unitOfWork.Events.Should().NotBeNull();
        unitOfWork.Registrations.Should().NotBeNull();
    }

    [Fact]
    public void Dispose_ShouldDisposeContext()
    {
        var context = DbContextHelper.CreateInMemoryContext();
        var unitOfWork = new UnitOfWork(context);

        unitOfWork.Dispose();

        var act = () => context.Users.ToList();
        act.Should().Throw<ObjectDisposedException>();
    }
}
