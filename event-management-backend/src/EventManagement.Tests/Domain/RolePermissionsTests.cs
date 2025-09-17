using EventManagement.Domain.Enums;
using EventManagement.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace EventManagement.Tests.Domain;

public class RolePermissionsTests
{
    [Fact]
    public void GetPermissions_ShouldReturnCorrectPermissions_ForEventCreator()
    {
        var permissions = RolePermissions.GetPermissions(UserRole.EventCreator);

        permissions.Should().Contain(Permission.CreateEvent);
        permissions.Should().Contain(Permission.ReadEvent);
        permissions.Should().Contain(Permission.UpdateEvent);
        permissions.Should().Contain(Permission.DeleteEvent);
        permissions.Should().Contain(Permission.CreateRegistration);
        permissions.Should().Contain(Permission.ReadRegistration);
        permissions.Should().Contain(Permission.DeleteRegistration);
        permissions.Should().Contain(Permission.ReadEventRegistrations);
    }

    [Fact]
    public void GetPermissions_ShouldReturnCorrectPermissions_ForEventParticipant()
    {
        var permissions = RolePermissions.GetPermissions(UserRole.EventParticipant);

        permissions.Should().Contain(Permission.CreateRegistration);
        permissions.Should().Contain(Permission.ReadEvent);
        permissions.Should().Contain(Permission.ReadRegistration);
        permissions.Should().NotContain(Permission.CreateEvent);
        permissions.Should().NotContain(Permission.UpdateEvent);
        permissions.Should().NotContain(Permission.DeleteEvent);
        permissions.Should().NotContain(Permission.DeleteRegistration);
        permissions.Should().NotContain(Permission.ReadEventRegistrations);
    }

    [Theory]
    [InlineData(UserRole.EventCreator, Permission.CreateEvent, true)]
    [InlineData(UserRole.EventCreator, Permission.ReadEvent, true)]
    [InlineData(UserRole.EventCreator, Permission.UpdateEvent, true)]
    [InlineData(UserRole.EventCreator, Permission.DeleteEvent, true)]
    [InlineData(UserRole.EventParticipant, Permission.CreateEvent, false)]
    [InlineData(UserRole.EventParticipant, Permission.UpdateEvent, false)]
    [InlineData(UserRole.EventParticipant, Permission.DeleteEvent, false)]
    [InlineData(UserRole.EventParticipant, Permission.ReadEvent, true)]
    [InlineData(UserRole.EventParticipant, Permission.CreateRegistration, true)]
    public void HasPermission_ShouldReturnCorrectResult(UserRole role, Permission permission, bool expectedResult)
    {
        var result = RolePermissions.HasPermission(role, permission);

        result.Should().Be(expectedResult);
    }

    [Fact]
    public void GetPermissions_ShouldReturnEmptyList_ForInvalidRole()
    {
        var invalidRole = (UserRole)999;

        var permissions = RolePermissions.GetPermissions(invalidRole);

        permissions.Should().BeEmpty();
    }

    [Fact]
    public void HasPermission_ShouldReturnFalse_ForInvalidRole()
    {
        var invalidRole = (UserRole)999;

        var result = RolePermissions.HasPermission(invalidRole, Permission.CreateEvent);

        result.Should().BeFalse();
    }
}
