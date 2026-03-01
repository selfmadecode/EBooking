namespace EBooking.Domain.Enums;

public enum EventStatus
{
    Active = 1,
    Cancelled = 2,
    Completed = 3,
    Postponed = 4
}
public enum BookingStatus
{
    Confirmed = 1,
    Cancelled = 2,
    Pending = 3
}

public enum TransactionStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3
}

public enum TransactionType
{
    TopUp = 1,
    BookingPayment = 2,
    BookingRefund = 3
}
