namespace EventManagement.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IEventRepository Events { get; }
    IRegistrationRepository Registrations { get; }
    Task<int> SaveChangesAsync();
}
