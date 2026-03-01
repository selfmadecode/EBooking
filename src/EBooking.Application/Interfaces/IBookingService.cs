using EBooking.Application.Common;
using EBooking.Application.DTOs.Booking;

namespace EBooking.Application.Interfaces;

public interface IBookingService
{
    Task<ApiResponse<BookingResponseDto>> CreateBookingAsync(Guid userId, CreateBookingDto dto);
    Task<ApiResponse<bool>> CancelBookingAsync(Guid userId, Guid bookingId);
    Task<ApiResponse<IEnumerable<BookingResponseDto>>> GetUserBookingsAsync(Guid userId);
    Task<ApiResponse<BookingResponseDto>> GetBookingByIdAsync(Guid userId, Guid bookingId);
}
