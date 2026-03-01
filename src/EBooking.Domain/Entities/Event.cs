using EBooking.Domain.Enums;

namespace EBooking.Domain.Entities;

public record Event : EntityBase
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string Location { get; set; }
    public required DateTime EventDate { get; set; }
    public required DateTime EventEndDate { get; set; }
    public Ticket? Ticket { get; set; }
    public Guid TicketId { get; set; }
    public string? ImageUrl { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Active;
    public ICollection<Booking> Bookings { get; set; } = [];
}
