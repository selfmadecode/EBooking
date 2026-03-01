using EBooking.Application.Common;
using EBooking.Application.DTOs.Booking;
using EBooking.Domain.Entities;
using EBooking.Domain.Enums;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace EBooking.Tests.Services;

public class BookingServiceTests : BookingServiceTestBase
{
    [Fact]
    public async Task CreateBooking_ValidRequest_DeductsWalletAndCreatesBooking()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tickets = 2;
        var price = 5_000m;
        var totalAmount = tickets * price;

        var evt = TestData.MakeEvent(available: 100, price: price);
        var dto = new CreateBookingDto { EventId = evt.Id, NumberOfTickets = tickets };

        SetupEvent(evt);
        SetupWalletDeduction(userId, totalAmount);
        SetupBookingResult(userId, evt, tickets);

        var sut = CreateSut();

        // Act
        var result = await sut.CreateBookingAsync(userId, dto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.NumberOfTickets.Should().Be(tickets);
        result.Data.TotalAmount.Should().Be(totalAmount);

        VerifyWalletDeduction(userId, totalAmount);
        VerifyTicketUpdate(98);
    }

    [Fact]
    public async Task CreateBooking_EventNotFound_ReturnsFailure()
    {
        // Arrange
        EventRepo
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Expression<Func<Event, object>>>()))
            .ReturnsAsync((Event?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.CreateBookingAsync(Guid.NewGuid(),
            new CreateBookingDto { EventId = Guid.NewGuid(), NumberOfTickets = 1 });

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task CreateBooking_CancelledEvent_ReturnsFailure()
    {
        // Arrange
        var evt = TestData.MakeEvent(status: EventStatus.Cancelled);
        SetupEvent(evt);

        var sut = CreateSut();

        // Act
        var result = await sut.CreateBookingAsync(Guid.NewGuid(),
            new CreateBookingDto { EventId = evt.Id, NumberOfTickets = 1 });

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not available");
    }

    [Fact]
    public async Task CreateBooking_PastEvent_ReturnsFailure()
    {
        // Arrange
        var evt = TestData.MakeEvent(
            eventDate: DateTime.UtcNow.AddDays(-1),
            eventEndDate: DateTime.UtcNow.AddDays(-1).AddHours(2));

        SetupEvent(evt);

        var sut = CreateSut();

        // Act
        var result = await sut.CreateBookingAsync(Guid.NewGuid(),
            new CreateBookingDto { EventId = evt.Id, NumberOfTickets = 1 });

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("already passed");
    }

    [Fact]
    public async Task CreateBooking_NotEnoughTickets_ReturnsFailure()
    {
        // Arrange
        var evt = TestData.MakeEvent(available: 1);
        SetupEvent(evt);

        var sut = CreateSut();

        // Act
        var result = await sut.CreateBookingAsync(Guid.NewGuid(),
            new CreateBookingDto { EventId = evt.Id, NumberOfTickets = 5 });

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Not enough");
    }

    [Fact]
    public async Task CreateBooking_InsufficientWalletBalance_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var evt = TestData.MakeEvent(available: 10, price: 50_000m);

        SetupEvent(evt);

        WalletService
            .Setup(w => w.DeductFromWalletAsync(
                userId,
                It.IsAny<decimal>(),
                It.IsAny<string>(),
                It.IsAny<string?>()))
            .ReturnsAsync(ApiResponse<bool>.FailureResponse("Insufficient wallet balance."));

        var sut = CreateSut();

        // Act
        var result = await sut.CreateBookingAsync(userId, new CreateBookingDto { EventId = evt.Id, NumberOfTickets = 1 });

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Insufficient");
    }

    [Fact]
    public async Task CancelBooking_ValidRequest_RefundsWalletAndRestoresTickets()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var evt = TestData.MakeEvent(totalTickets: 100, available: 98);
        var booking = TestData.MakeBooking(userId: userId, evt: evt, tickets: 2);
        var wallet = TestData.MakeWallet(userId, balance: 5_000m);
        var initialBalance = wallet.Balance;

        BookingRepo
            .Setup(r => r.GetBookingWithDetailsAsync(booking.Id))
            .ReturnsAsync(booking);

        WalletRepo
            .Setup(r => r.GetWalletByUserIdAsync(userId))
            .ReturnsAsync(wallet);

        TxRepo
            .Setup(r => r.AddAsync(It.IsAny<WalletTransaction>()))
            .ReturnsAsync((WalletTransaction t) => t);

        WalletRepo
            .Setup(r => r.UpdateAsync(wallet))
            .Returns(Task.CompletedTask);

        EventRepo
            .Setup(r => r.UpdateAsync(evt))
            .Returns(Task.CompletedTask);

        BookingRepo
            .Setup(r => r.UpdateAsync(booking))
            .Returns(Task.CompletedTask);

        var sut = CreateSut();

        // Act
        var result = await sut.CancelBookingAsync(userId, booking.Id);

        // Assert
        result.Success.Should().BeTrue();

        VerifyTicketsRestored(expectedAvailableTickets: 100);
        VerifyBookingCancelled();

        wallet.Balance.Should().Be(initialBalance + booking.TotalAmount);
    }

    [Fact]
    public async Task CancelBooking_EventAlreadyOccurred_ReturnsFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pastEvt = TestData.MakeEvent(
            eventDate: DateTime.UtcNow.AddDays(-2),
            eventEndDate: DateTime.UtcNow.AddDays(-2).AddHours(3));

        var booking = TestData.MakeBooking(userId: userId, evt: pastEvt);

        BookingRepo
            .Setup(r => r.GetBookingWithDetailsAsync(booking.Id))
            .ReturnsAsync(booking);

        var sut = CreateSut();

        // Act
        var result = await sut.CancelBookingAsync(userId, booking.Id);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("already occurred");
    }
}
