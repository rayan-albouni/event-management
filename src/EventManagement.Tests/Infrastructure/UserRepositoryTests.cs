using EventManagement.Domain.Entities;
using EventManagement.Domain.Enums;
using EventManagement.Infrastructure.Repositories;
using EventManagement.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace EventManagement.Tests.Infrastructure;

public class UserRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldAddUserToDatabase()
    {
        using var context = DbContextHelper.CreateInMemoryContext();
        var repository = new UserRepository(context);
        var user = new User("test@example.com", "passwordHash", UserRole.EventCreator);

        await repository.AddAsync(user);
        await context.SaveChangesAsync();

        var savedUser = await repository.GetByIdAsync(user.Id);
        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be("test@example.com");
        savedUser.Role.Should().Be(UserRole.EventCreator);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenEmailExists()
    {
        using var context = DbContextHelper.CreateInMemoryContext();
        var repository = new UserRepository(context);
        var user = new User("test@example.com", "passwordHash", UserRole.EventCreator);
        
        await repository.AddAsync(user);
        await context.SaveChangesAsync();

        var result = await repository.GetByEmailAsync("test@example.com");

        result.Should().NotBeNull();
        result!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenEmailDoesNotExist()
    {
        using var context = DbContextHelper.CreateInMemoryContext();
        var repository = new UserRepository(context);

        var result = await repository.GetByEmailAsync("nonexistent@example.com");

        result.Should().BeNull();
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
    {
        using var context = DbContextHelper.CreateInMemoryContext();
        var repository = new UserRepository(context);
        var user = new User("test@example.com", "passwordHash", UserRole.EventCreator);
        
        await repository.AddAsync(user);
        await context.SaveChangesAsync();

        var result = await repository.EmailExistsAsync("test@example.com");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnFalse_WhenEmailDoesNotExist()
    {
        using var context = DbContextHelper.CreateInMemoryContext();
        var repository = new UserRepository(context);

        var result = await repository.EmailExistsAsync("nonexistent@example.com");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        using var context = DbContextHelper.CreateInMemoryContext();
        var repository = new UserRepository(context);
        var user1 = new User("user1@example.com", "passwordHash", UserRole.EventCreator);
        var user2 = new User("user2@example.com", "passwordHash", UserRole.EventParticipant);
        
        await repository.AddAsync(user1);
        await repository.AddAsync(user2);
        await context.SaveChangesAsync();

        var result = await repository.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().Contain(u => u.Email == "user1@example.com");
        result.Should().Contain(u => u.Email == "user2@example.com");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser()
    {
        using var context = DbContextHelper.CreateInMemoryContext();
        var repository = new UserRepository(context);
        var user = new User("test@example.com", "passwordHash", UserRole.EventCreator);
        
        await repository.AddAsync(user);
        await context.SaveChangesAsync();

        await repository.UpdateAsync(user);
        await context.SaveChangesAsync();

        var updatedUser = await repository.GetByIdAsync(user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveUser()
    {
        using var context = DbContextHelper.CreateInMemoryContext();
        var repository = new UserRepository(context);
        var user = new User("test@example.com", "passwordHash", UserRole.EventCreator);
        
        await repository.AddAsync(user);
        await context.SaveChangesAsync();

        await repository.DeleteAsync(user.Id);
        await context.SaveChangesAsync();

        var deletedUser = await repository.GetByIdAsync(user.Id);
        deletedUser.Should().BeNull();
    }
}
