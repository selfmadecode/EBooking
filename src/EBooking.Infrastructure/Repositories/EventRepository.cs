using EBooking.Domain.Entities;
using EBooking.Domain.Enums;
using EBooking.Domain.Interfaces;
using EBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EBooking.Infrastructure.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Event>> GetActiveEventsAsync() =>
        await _dbSet.AsNoTracking().Include(e => e.Ticket)
            .Where(e => e.Status == EventStatus.Active && e.EventDate > DateTime.UtcNow)
            .OrderBy(e => e.EventDate)
            .ToListAsync();

    public async Task<Event?> GetEventWithBookingsAsync(Guid id) =>
        await _dbSet.Include(x => x.Ticket).Include(e => e.Bookings)
            .FirstOrDefaultAsync(e => e.Id == id);
}
