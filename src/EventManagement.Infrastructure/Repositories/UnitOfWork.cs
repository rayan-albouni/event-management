using EventManagement.Domain.Interfaces;
using EventManagement.Infrastructure.Data;

namespace EventManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EventManagementDbContext _context;

    public UnitOfWork(EventManagementDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Events = new EventRepository(_context);
        Registrations = new RegistrationRepository(_context);
    }

    public IUserRepository Users { get; }
    public IEventRepository Events { get; }
    public IRegistrationRepository Registrations { get; }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
