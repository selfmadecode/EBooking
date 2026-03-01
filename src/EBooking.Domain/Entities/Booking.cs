using EBooking.Domain.Enums;

namespace EBooking.Domain.Entities;

public record Booking : EntityBase
{
    public ApplicationUser? User { get; set; }
    public Guid UserId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public int NumberOfTickets { get; set; }
    public decimal TotalAmount { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
    public Event? Event { get; set; }
    public Guid EventId { get; set; }
}
