using EBooking.Domain.Entities;
using EBooking.Domain.Enums;
using EBooking.Domain.Interfaces;
using EBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EBooking.Infrastructure.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Booking>> GetUserBookingsAsync(Guid userId) =>
        await _dbSet.AsNoTracking()
            .Include(b => b.Event)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedOn)
            .ToListAsync();

    public async Task<Booking?> GetBookingWithDetailsAsync(Guid id) =>
        await _dbSet.Include(b => b.Event).ThenInclude(x => x.Ticket)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<bool> UserHasBookingForEventAsync(Guid userId, Guid eventId) =>
        await _dbSet.AnyAsync(b => b.UserId == userId && b.EventId == eventId && b.Status == BookingStatus.Confirmed);
}
