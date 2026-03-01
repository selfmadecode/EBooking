using EBooking.Domain.Entities;

namespace EBooking.Domain.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    Task<IEnumerable<Event>> GetActiveEventsAsync();
    Task<Event?> GetEventWithBookingsAsync(Guid id);
}
