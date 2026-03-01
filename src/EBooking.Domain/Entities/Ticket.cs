namespace EBooking.Domain.Entities;

public record Ticket : EntityBase
{
    public required decimal TicketPrice { get; set; }
    public int TotalTickets { get; set; }
    public required int AvailableTickets { get; set; }
}
