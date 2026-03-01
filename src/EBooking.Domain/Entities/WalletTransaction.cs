using EBooking.Domain.Enums;

namespace EBooking.Domain.Entities;

public record WalletTransaction : EntityBase
{
    public Wallet? Wallet { get; set; }
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Completed;
    public string Description { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
}
