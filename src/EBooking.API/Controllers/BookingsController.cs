using EBooking.Application.DTOs.Booking;
using EBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EBooking.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BookingsController : BaseController
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var result = await _bookingService.CreateBookingAsync(UserId, dto);
        return ReturnResponse(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        var result = await _bookingService.GetUserBookingsAsync(UserId);
        return ReturnResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBookingById(Guid id)
    {
        var result = await _bookingService.GetBookingByIdAsync(UserId, id);
        return ReturnResponse(result);
    }

    [HttpDelete("{id:guid}/cancel")]
    public async Task<IActionResult> CancelBooking(Guid id)
    {
        var result = await _bookingService.CancelBookingAsync(UserId, id);
        return ReturnResponse(result);
    }
}
