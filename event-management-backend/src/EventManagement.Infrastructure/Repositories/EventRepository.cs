using EventManagement.Domain.Entities;
using EventManagement.Domain.Interfaces;
using EventManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Infrastructure.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(EventManagementDbContext context) : base(context)
    {
    }

    public override async Task<Event?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(e => e.Creator)
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public override async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _dbSet
            .Include(e => e.Creator)
            .Include(e => e.Registrations)
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetActiveEventsAsync()
    {
        return await _dbSet
            .Include(e => e.Creator)
            .Include(e => e.Registrations)
            .Where(e => e.StartTime > DateTime.UtcNow)
            .OrderBy(e => e.StartTime)
            .ToListAsync();
    }
}
