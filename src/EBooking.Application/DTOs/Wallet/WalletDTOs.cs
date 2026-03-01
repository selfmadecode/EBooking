using System.ComponentModel.DataAnnotations;

namespace EBooking.Application.DTOs.Wallet;

public record TopUpWalletDto
{
    [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "SimulatedCard";
    public string? CardNumber { get; set; }
}

public record WalletResponseDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime ModifiedOn { get; set; }
}

public record WalletTransactionResponseDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTime CreatedOn { get; set; }
}
