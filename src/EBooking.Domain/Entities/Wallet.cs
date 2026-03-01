namespace EBooking.Domain.Entities;

public record Wallet : EntityBase
{
    public decimal Balance { get; set; }
    public ApplicationUser? User { get; set; }
    public Guid UserId { get; set; }
    public ICollection<WalletTransaction> Transactions { get; set; } = [];
}
