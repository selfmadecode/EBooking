using EBooking.Application.Common;
using EBooking.Application.Interfaces;
using EBooking.Domain.Entities;
using EBooking.Domain.Enums;
using EBooking.Domain.Interfaces;
using EBooking.Infrastructure.Services;
using EBooking.Tests.Services;
using Moq;

namespace EBooking.Tests;

public abstract class BookingServiceTestBase
{
    protected readonly Mock<IUnitOfWork> Uow = new();
    protected readonly Mock<IEventRepository> EventRepo = new();
    protected readonly Mock<IBookingRepository> BookingRepo = new();
    protected readonly Mock<IWalletRepository> WalletRepo = new();
    protected readonly Mock<IWalletTransactionRepository> TxRepo = new();
    protected readonly Mock<IWalletService> WalletService = new();

    protected BookingService CreateSut()
    {
        Uow.SetupGet(x => x.Events).Returns(EventRepo.Object);
        Uow.SetupGet(x => x.Bookings).Returns(BookingRepo.Object);
        Uow.SetupGet(x => x.Wallets).Returns(WalletRepo.Object);
        Uow.SetupGet(x => x.WalletTransactions).Returns(TxRepo.Object);
        Uow.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        return new BookingService(Uow.Object, TestData.CreateMapper(), WalletService.Object);
    }

    protected void SetupWalletDeduction(Guid userId, decimal amount)
    {
        WalletService
            .Setup(w => w.DeductFromWalletAsync(
                userId,
                amount,
                It.IsAny<string>(),
                It.IsAny<string?>()))
            .ReturnsAsync(ApiResponse<bool>.SuccessResponse(true));
    }

    protected void SetupEvent(Event evt)
    {
        EventRepo
            .Setup(r => r.GetByIdAsync(evt.Id, t => t.Ticket))
            .ReturnsAsync(evt);

        EventRepo
            .Setup(r => r.UpdateAsync(It.IsAny<Event>()))
            .Returns(Task.CompletedTask);
    }

    protected void SetupBookingResult(Guid userId, Event evt, int tickets)
    {
        BookingRepo
            .Setup(r => r.AddAsync(It.IsAny<Booking>()))
            .ReturnsAsync((Booking b) => b);

        BookingRepo
            .Setup(r => r.GetBookingWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync(TestData.MakeBooking(userId: userId, evt: evt, tickets: 2));
    }

    protected void VerifyWalletDeduction(Guid userId, decimal amount)
    {
        WalletService.Verify(w =>
            w.DeductFromWalletAsync(
                userId,
                amount,
                It.IsAny<string>(),
                It.IsAny<string?>()),
            Times.Once);
    }

    protected void VerifyTicketUpdate(int expectedAvailableTickets)
    {
        EventRepo.Verify(r =>
            r.UpdateAsync(It.Is<Event>(e =>
                e.Ticket.AvailableTickets == expectedAvailableTickets)),
            Times.Once);
    }

    protected void VerifyTicketsRestored(int expectedAvailableTickets)
    {
        EventRepo.Verify(r =>
            r.UpdateAsync(It.Is<Event>(e =>
                e.Ticket.AvailableTickets == expectedAvailableTickets)),
            Times.Once);
    }

    protected void VerifyBookingCancelled()
    {
        BookingRepo.Verify(r =>
            r.UpdateAsync(It.Is<Booking>(b =>
                b.Status == BookingStatus.Cancelled)),
            Times.Once);
    }
}
