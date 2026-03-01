using AutoMapper;
using EBooking.Application.Common;
using EBooking.Application.DTOs.Booking;
using EBooking.Application.Interfaces;
using EBooking.Domain.Entities;
using EBooking.Domain.Enums;
using EBooking.Domain.Interfaces;

namespace EBooking.Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IWalletService _walletService;

    public BookingService(IUnitOfWork unitOfWork, IMapper mapper, IWalletService walletService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _walletService = walletService;
    }

    public async Task<ApiResponse<BookingResponseDto>> CreateBookingAsync(Guid userId, CreateBookingDto dto)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(dto.EventId, t => t.Ticket);
        if (eventEntity == null)
            return ApiResponse<BookingResponseDto>.FailureResponse("Event not found.");

        if (eventEntity.Status != EventStatus.Active)
            return ApiResponse<BookingResponseDto>.FailureResponse("This event is not available for booking.");

        if (eventEntity.EventDate <= DateTime.UtcNow)
            return ApiResponse<BookingResponseDto>.FailureResponse("This event has already passed.");

        if (eventEntity.Ticket.AvailableTickets < dto.NumberOfTickets)
            return ApiResponse<BookingResponseDto>.FailureResponse($"Not enough tickets available. Only {eventEntity.Ticket.AvailableTickets} left.");

        var totalAmount = eventEntity.Ticket.TicketPrice * dto.NumberOfTickets;

        // Deduct from wallet
        var bookingRef = $"BK-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        var deductResult = await _walletService.DeductFromWalletAsync(
            userId,
            totalAmount,
            $"Payment for {dto.NumberOfTickets} ticket(s) to {eventEntity.Title}",
            bookingRef
        );

        if (!deductResult.Success)
            return ApiResponse<BookingResponseDto>.FailureResponse(deductResult.Message);

        eventEntity.Ticket.AvailableTickets -= dto.NumberOfTickets;
        eventEntity.ModifiedOn = DateTime.UtcNow;
        await _unitOfWork.Events.UpdateAsync(eventEntity);

        // Create booking
        var booking = new Booking
        {
            UserId = userId,
            EventId = dto.EventId,
            NumberOfTickets = dto.NumberOfTickets,
            TotalAmount = totalAmount,
            Status = BookingStatus.Confirmed,
            BookingReference = bookingRef,
            CreatedOn = DateTime.UtcNow
        };

        await _unitOfWork.Bookings.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync();

        // Reload with details
        var bookingWithDetails = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(booking.Id);
        return ApiResponse<BookingResponseDto>.SuccessResponse(
            _mapper.Map<BookingResponseDto>(bookingWithDetails),
            "Booking confirmed successfully.");
    }

    public async Task<ApiResponse<bool>> CancelBookingAsync(Guid userId, Guid bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(bookingId);
        if (booking == null)
            return ApiResponse<bool>.FailureResponse("Booking not found.");

        if (booking.UserId != userId)
            return ApiResponse<bool>.FailureResponse("Unauthorized to cancel this booking.");

        if (booking.Status == BookingStatus.Cancelled)
            return ApiResponse<bool>.FailureResponse("This booking is already cancelled.");

        if (booking.Event!.EventDate <= DateTime.UtcNow)
            return ApiResponse<bool>.FailureResponse("Cannot cancel booking for an event that has already occurred.");

        // Refund wallet
        var wallet = await _unitOfWork.Wallets.GetWalletByUserIdAsync(userId);
        if (wallet != null)
        {
            var balanceBefore = wallet.Balance;
            wallet.Balance += booking.TotalAmount;
            wallet.ModifiedOn = DateTime.UtcNow;

            var refundTransaction = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = booking.TotalAmount,
                Type = TransactionType.BookingRefund,
                Status = TransactionStatus.Completed,
                Description = $"Refund for cancelled booking {booking.BookingReference}",
                Reference = booking.BookingReference,
                BalanceBefore = balanceBefore,
                BalanceAfter = wallet.Balance,
                CreatedOn = DateTime.UtcNow,
            };

            await _unitOfWork.WalletTransactions.AddAsync(refundTransaction);
            await _unitOfWork.Wallets.UpdateAsync(wallet);
        }

        // Restore tickets
        booking.Event.Ticket.AvailableTickets += booking.NumberOfTickets;
        booking.Event.Ticket.ModifiedOn = DateTime.UtcNow;
        await _unitOfWork.Events.UpdateAsync(booking.Event);

        // Cancel booking
        booking.Status = BookingStatus.Cancelled;
        booking.ModifiedOn = DateTime.UtcNow;
        await _unitOfWork.Bookings.UpdateAsync(booking);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResponse(true, "Booking cancelled and refund processed.");
    }

    public async Task<ApiResponse<IEnumerable<BookingResponseDto>>> GetUserBookingsAsync(Guid userId)
    {
        var bookings = await _unitOfWork.Bookings.GetUserBookingsAsync(userId);

        return ApiResponse<IEnumerable<BookingResponseDto>>.SuccessResponse(
            _mapper.Map<IEnumerable<BookingResponseDto>>(bookings));
    }

    public async Task<ApiResponse<BookingResponseDto>> GetBookingByIdAsync(Guid userId, Guid bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(bookingId);
        if (booking == null)
            return ApiResponse<BookingResponseDto>.FailureResponse("Booking not found.");

        if (booking.UserId != userId)
            return ApiResponse<BookingResponseDto>.FailureResponse("Unauthorized to view this booking.");

        return ApiResponse<BookingResponseDto>.SuccessResponse(_mapper.Map<BookingResponseDto>(booking));
    }
}
