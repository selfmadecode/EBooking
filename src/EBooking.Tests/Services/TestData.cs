using AutoMapper;
using EBooking.Application.Mappings;
using EBooking.Domain.Entities;
using EBooking.Domain.Enums;

namespace EBooking.Tests.Services;

public class TestData
{
    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return config.CreateMapper();
    }

    public static Event MakeEvent(Guid? id = null,
        string title = "Test Event",
        int totalTickets = 100,
        int available = 100,
        decimal price = 5_000m,
        EventStatus status = EventStatus.Active,
        DateTime? eventDate = null,
        DateTime? eventEndDate = null)
    {
        var start = eventDate ?? DateTime.UtcNow.AddDays(7);
        var end = eventEndDate ?? start.AddHours(3);
        return new Event
        {
            Id = id ?? Guid.NewGuid(),
            Title = title,
            Description = "A test event",
            Location = "Lagos, Nigeria",
            EventDate = start,
            EventEndDate = end,
            Ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                TicketPrice = price,
                TotalTickets = totalTickets,
                AvailableTickets = available,
            },
            Status = status,
        };
    }

    public static Booking MakeBooking(
        Guid? id = null,
        Guid? userId = null,
        Event? evt = null,
        int tickets = 2,
        BookingStatus status = BookingStatus.Confirmed)
    {
        var e = evt ?? MakeEvent();
        return new Booking
        {
            Id = id ?? Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            EventId = e.Id,
            Event = e,
            NumberOfTickets = tickets,
            TotalAmount = e.Ticket.TicketPrice * tickets,
            Status = status,
            BookingReference = $"BK-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
        };
    }

    public static Wallet MakeWallet(Guid userId, decimal balance = 20_000m) =>
        new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Balance = balance,
        };
}
