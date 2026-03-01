using AutoMapper;
using EBooking.Application.DTOs.Booking;
using EBooking.Application.DTOs.Event;
using EBooking.Application.DTOs.Wallet;
using EBooking.Domain.Entities;

namespace EBooking.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateEventDto, Event>()
            .ForMember(dest => dest.Ticket,
                opt => opt.MapFrom(src => new Ticket
                {
                    TicketPrice = src.TicketPrice,
                    TotalTickets = src.TotalTickets,
                    AvailableTickets = src.TotalTickets
                }));

        CreateMap<Event, EventResponseDto>()
            .ForMember(dest => dest.TicketPrice,
                opt => opt.MapFrom(src => src.Ticket != null ? src.Ticket.TicketPrice : 0))
            .ForMember(dest => dest.TotalTickets,
                opt => opt.MapFrom(src => src.Ticket != null ? src.Ticket.TotalTickets : 0))
            .ForMember(dest => dest.AvailableTickets,
                opt => opt.MapFrom(src => src.Ticket != null ? src.Ticket.AvailableTickets : 0))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Booking, BookingResponseDto>()
            .ForMember(d => d.EventTitle, o => o.MapFrom(s => s.Event != null ? s.Event.Title : string.Empty))
            .ForMember(d => d.EventLocation, o => o.MapFrom(s => s.Event != null ? s.Event.Location : string.Empty))
            .ForMember(d => d.EventDate, o => o.MapFrom(s => s.Event != null ? s.Event.EventDate : default))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        // Wallet mappings
        CreateMap<Wallet, WalletResponseDto>();
        CreateMap<WalletTransaction, WalletTransactionResponseDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
    }
}
