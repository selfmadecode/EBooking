using EBooking.Domain.Entities;

namespace EBooking.Domain.Interfaces;

public interface IBookingRepository : IRepository<Booking>
{
    Task<IEnumerable<Booking>> GetUserBookingsAsync(Guid userId);
    Task<Booking?> GetBookingWithDetailsAsync(Guid id);
    Task<bool> UserHasBookingForEventAsync(Guid userId, Guid eventId);
}
