using EBooking.Domain.Enums;

namespace EBooking.Application.DTOs.Event
{
    public class CreateEventDto
    {
        public required string Title { get; set; }
        public string Description { get; set; }
        public required string Location { get; set; }
        public required DateTime EventDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public decimal TicketPrice { get; set; }
        public int TotalTickets { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UpdateEventDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime? EventDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public decimal? TicketPrice { get; set; }
        public int? TotalTickets { get; set; }
        public string? ImageUrl { get; set; }
        public EventStatus? Status { get; set; }
    }

    public class EventResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public decimal TicketPrice { get; set; }
        public int TotalTickets { get; set; }
        public int AvailableTickets { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedOn { get; set; }
    }

}
