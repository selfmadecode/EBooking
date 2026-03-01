using AutoMapper;
using EBooking.Application.Common;
using EBooking.Application.DTOs.Event;
using EBooking.Application.Interfaces;
using EBooking.Domain.Entities;
using EBooking.Domain.Interfaces;

namespace EBooking.Infrastructure.Services;

public class EventService : IEventService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EventService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<EventResponseDto>> CreateEventAsync(CreateEventDto dto, Guid createdBy)
    {
        if (dto.EventDate <= DateTime.UtcNow)
            return ApiResponse<EventResponseDto>.FailureResponse("Event date must be in the future.");

        if (dto.EventEndDate <= dto.EventDate)
            return ApiResponse<EventResponseDto>.FailureResponse("End date must be after start date.");

        if (dto.TicketPrice < 0)
            return ApiResponse<EventResponseDto>.FailureResponse("Ticket price cannot be negative.");

        if (dto.TotalTickets <= 0)
            return ApiResponse<EventResponseDto>.FailureResponse("Total tickets must be greater than zero.");

        var eventEntity = _mapper.Map<Event>(dto);
        eventEntity.CreatedBy = createdBy;
        eventEntity.Ticket.CreatedBy = createdBy;

        await _unitOfWork.Events.AddAsync(eventEntity);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<EventResponseDto>.SuccessResponse(_mapper.Map<EventResponseDto>(eventEntity), "Event created successfully.");
    }

    public async Task<ApiResponse<EventResponseDto>> GetEventByIdAsync(Guid id)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(id, e => e.Ticket);

        if (eventEntity == null)
            return ApiResponse<EventResponseDto>.FailureResponse("Event not found.");

        return ApiResponse<EventResponseDto>.SuccessResponse(_mapper.Map<EventResponseDto>(eventEntity));
    }

    public async Task<ApiResponse<IEnumerable<EventResponseDto>>> GetAllEventsAsync()
    {
        var events = await _unitOfWork.Events.GetAllAsync(e => e.Ticket);

        return ApiResponse<IEnumerable<EventResponseDto>>.SuccessResponse(_mapper.Map<IEnumerable<EventResponseDto>>(events));
    }

    public async Task<ApiResponse<IEnumerable<EventResponseDto>>> GetActiveEventsAsync()
    {
        var events = await _unitOfWork.Events.GetActiveEventsAsync();
        return ApiResponse<IEnumerable<EventResponseDto>>.SuccessResponse(_mapper.Map<IEnumerable<EventResponseDto>>(events));
    }

    public async Task<ApiResponse<EventResponseDto>> UpdateEventAsync(Guid id, UpdateEventDto dto, Guid updatedBy)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(id, e => e.Ticket);

        if (eventEntity is null)
            return ApiResponse<EventResponseDto>.FailureResponse("Event not found.");

        var ticketValidationResult = UpdateTicket(eventEntity, dto);
        
        if (ticketValidationResult is not null)
            return ticketValidationResult;

        UpdateEventFields(eventEntity, dto);

        eventEntity.ModifiedOn = DateTime.UtcNow;
        eventEntity.ModifiedBy = updatedBy;

        await _unitOfWork.Events.UpdateAsync(eventEntity);
        await _unitOfWork.SaveChangesAsync();

        var response = _mapper.Map<EventResponseDto>(eventEntity);
        return ApiResponse<EventResponseDto>.SuccessResponse(response, "Event updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteEventAsync(Guid id)
    {
        var eventEntity = await _unitOfWork.Events.GetEventWithBookingsAsync(id);
        if (eventEntity == null)
            return ApiResponse<bool>.FailureResponse("Event not found.");

        var activeBookings = eventEntity.Bookings.Any(b => b.Status == Domain.Enums.BookingStatus.Confirmed);
        if (activeBookings)
            return ApiResponse<bool>.FailureResponse("Cannot delete event with active bookings. Cancel the event instead.");

        await _unitOfWork.Events.DeleteAsync(eventEntity);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Event deleted successfully.");
    }

    private static void UpdateEventFields(Event eventEntity, UpdateEventDto dto)
    {
        eventEntity.Title = dto.Title ?? eventEntity.Title;
        eventEntity.Description = dto.Description ?? eventEntity.Description;
        eventEntity.Location = dto.Location ?? eventEntity.Location;
        eventEntity.ImageUrl = dto.ImageUrl ?? eventEntity.ImageUrl;

        if (dto.EventDate.HasValue)
            eventEntity.EventDate = dto.EventDate.Value;

        if (dto.EventEndDate.HasValue)
            eventEntity.EventEndDate = dto.EventEndDate.Value;

        if (dto.Status.HasValue)
            eventEntity.Status = dto.Status.Value;
    }

    private static ApiResponse<EventResponseDto>? UpdateTicket(Event eventEntity, UpdateEventDto dto)
    {
        if (eventEntity.Ticket is null)
            return null;

        if (dto.TicketPrice.HasValue)
            eventEntity.Ticket.TicketPrice = dto.TicketPrice.Value;

        if (!dto.TotalTickets.HasValue)
            return null;

        var bookedTickets = eventEntity.Ticket.TotalTickets - eventEntity.Ticket.AvailableTickets;

        if (dto.TotalTickets.Value < bookedTickets)
            return ApiResponse<EventResponseDto>
                .FailureResponse($"Cannot reduce total tickets below booked count ({bookedTickets}).");

        eventEntity.Ticket.TotalTickets = dto.TotalTickets.Value;
        eventEntity.Ticket.AvailableTickets = dto.TotalTickets.Value - bookedTickets;

        return null;
    }
}
