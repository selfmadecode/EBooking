using System.ComponentModel.DataAnnotations;

namespace EBooking.Application.DTOs.Wallet;

public class TopUpWalletDto
{
    [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "SimulatedCard";
    public string? CardNumber { get; set; }
}

public class WalletResponseDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public DateTime ModifiedOn { get; set; }
}

public class WalletTransactionResponseDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTime CreatedOn { get; set; }
}
