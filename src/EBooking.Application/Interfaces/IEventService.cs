using EBooking.Application.Common;
using EBooking.Application.DTOs.Event;

namespace EBooking.Application.Interfaces
{
    public interface IEventService
    {
        Task<ApiResponse<EventResponseDto>> CreateEventAsync(CreateEventDto dto, Guid createdBy);
        Task<ApiResponse<EventResponseDto>> GetEventByIdAsync(Guid id);
        Task<ApiResponse<IEnumerable<EventResponseDto>>> GetAllEventsAsync();
        Task<ApiResponse<IEnumerable<EventResponseDto>>> GetActiveEventsAsync();
        Task<ApiResponse<EventResponseDto>> UpdateEventAsync(Guid id, UpdateEventDto dto, Guid updatedBy);
        Task<ApiResponse<bool>> DeleteEventAsync(Guid id);
    }
}
