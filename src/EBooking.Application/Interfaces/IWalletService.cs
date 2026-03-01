using EBooking.Application.Common;
using EBooking.Application.DTOs.Wallet;

namespace EBooking.Application.Interfaces;

public interface IWalletService
{
    Task<ApiResponse<WalletResponseDto>> GetWalletAsync(Guid userId);
    Task<ApiResponse<WalletResponseDto>> TopUpWalletAsync(Guid userId, TopUpWalletDto dto);
    Task<ApiResponse<IEnumerable<WalletTransactionResponseDto>>> GetTransactionHistoryAsync(Guid userId);
    Task<ApiResponse<bool>> DeductFromWalletAsync(Guid userId, decimal amount, string description, string? reference = null);
}
