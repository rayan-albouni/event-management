using EventManagement.Domain.Entities;
using EventManagement.Domain.Interfaces;
using EventManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Infrastructure.Repositories;

public class RegistrationRepository : Repository<Registration>, IRegistrationRepository
{
    public RegistrationRepository(EventManagementDbContext context) : base(context)
    {
    }

    public override async Task<Registration?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(r => r.Event)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public override async Task<IEnumerable<Registration>> GetAllAsync()
    {
        return await _dbSet
            .Include(r => r.Event)
            .ToListAsync();
    }

    public async Task<IEnumerable<Registration>> GetRegistrationsByEventIdAsync(Guid eventId)
    {
        return await _dbSet
            .Include(r => r.Event)
            .Where(r => r.EventId == eventId)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> IsEmailRegisteredForEventAsync(string email, Guid eventId)
    {
        return await _dbSet.AnyAsync(r => r.Email == email && r.EventId == eventId);
    }
}
