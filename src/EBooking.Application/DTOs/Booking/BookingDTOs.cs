using System.ComponentModel.DataAnnotations;

namespace EBooking.Application.DTOs.Booking;

public class CreateBookingDto
{
    public Guid EventId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Number of tickets must be at least 1.")]
    public int NumberOfTickets { get; set; }
}

public class BookingResponseDto
{
    public Guid Id { get; set; }
    public string? BookingReference { get; set; }
    public Guid EventId { get; set; }
    public string? EventTitle { get; set; }
    public string? EventLocation { get; set; } 
    public DateTime EventDate { get; set; }
    public int NumberOfTickets { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedOn { get; set; }
}
