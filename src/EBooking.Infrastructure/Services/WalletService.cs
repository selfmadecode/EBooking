using AutoMapper;
using EBooking.Application.Common;
using EBooking.Application.DTOs.Wallet;
using EBooking.Application.Interfaces;
using EBooking.Domain.Entities;
using EBooking.Domain.Enums;
using EBooking.Domain.Interfaces;

namespace EBooking.Infrastructure.Services;

public class WalletService : IWalletService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPaymentGatewayService _paymentGateway;

    public WalletService(IUnitOfWork unitOfWork, IMapper mapper, IPaymentGatewayService paymentGateway)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _paymentGateway = paymentGateway;
    }

    public async Task<ApiResponse<WalletResponseDto>> GetWalletAsync(Guid userId)
    {
        var wallet = await _unitOfWork.Wallets.GetWalletByUserIdAsync(userId);
        if (wallet == null)
            return ApiResponse<WalletResponseDto>.FailureResponse("Wallet not found.");

        return ApiResponse<WalletResponseDto>.SuccessResponse(_mapper.Map<WalletResponseDto>(wallet));
    }

    public async Task<ApiResponse<WalletResponseDto>> TopUpWalletAsync(Guid userId, TopUpWalletDto dto)
    {
        if (dto.Amount > 1_000_000)
            return ApiResponse<WalletResponseDto>.FailureResponse("Top-up amount cannot exceed 1,000,000.");

        var wallet = await _unitOfWork.Wallets.GetWalletByUserIdAsync(userId);
        if (wallet == null)
            return ApiResponse<WalletResponseDto>.FailureResponse("Wallet not found.");

        // Simulate payment gateway processing
        var paymentSuccess = await _paymentGateway.ProcessPaymentAsync(dto.Amount, dto.PaymentMethod, dto.CardNumber);
        if (!paymentSuccess)
            return ApiResponse<WalletResponseDto>.FailureResponse("Payment processing failed. Please try again.");

        var balanceBefore = wallet.Balance;
        wallet.Balance += dto.Amount;
        wallet.ModifiedOn = DateTime.UtcNow;

        var transaction = new WalletTransaction
        {
            WalletId = wallet.Id,
            Amount = dto.Amount,
            Type = TransactionType.TopUp,
            Status = TransactionStatus.Completed,
            Description = $"Wallet top-up via {dto.PaymentMethod}",
            Reference = Guid.NewGuid().ToString("N")[..12].ToUpper(),
            BalanceBefore = balanceBefore,
            BalanceAfter = wallet.Balance,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = userId
        };

        await _unitOfWork.WalletTransactions.AddAsync(transaction);
        await _unitOfWork.Wallets.UpdateAsync(wallet);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<WalletResponseDto>.SuccessResponse(_mapper.Map<WalletResponseDto>(wallet), $"Wallet topped up successfully. New balance: {wallet.Balance:C}");
    }

    public async Task<ApiResponse<IEnumerable<WalletTransactionResponseDto>>> GetTransactionHistoryAsync(Guid userId)
    {
        var wallet = await _unitOfWork.Wallets.GetWalletByUserIdAsync(userId);
        if (wallet == null)
            return ApiResponse<IEnumerable<WalletTransactionResponseDto>>.FailureResponse("Wallet not found.");

        var transactions = await _unitOfWork.WalletTransactions.GetTransactionsByWalletIdAsync(wallet.Id);
        var data = _mapper.Map<IEnumerable<WalletTransactionResponseDto>>(transactions);

        return ApiResponse<IEnumerable<WalletTransactionResponseDto>>.SuccessResponse(data);
    }

    public async Task<ApiResponse<bool>> DeductFromWalletAsync(Guid userId, decimal amount, string description, string? reference = null)
    {
        var wallet = await _unitOfWork.Wallets.GetWalletByUserIdAsync(userId);
        if (wallet == null)
            return ApiResponse<bool>.FailureResponse("Wallet not found.");

        if (wallet.Balance < amount)
            return ApiResponse<bool>.FailureResponse($"Insufficient wallet balance. Available: {wallet.Balance}, Required: {amount}");

        var balanceBefore = wallet.Balance;
        wallet.Balance -= amount;
        wallet.ModifiedOn = DateTime.UtcNow;

        var transaction = new WalletTransaction
        {
            WalletId = wallet.Id,
            Amount = amount,
            Type = TransactionType.BookingPayment,
            Status = TransactionStatus.Completed,
            Description = description,
            Reference = reference,
            BalanceBefore = balanceBefore,
            BalanceAfter = wallet.Balance,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = userId
        };

        await _unitOfWork.WalletTransactions.AddAsync(transaction);
        await _unitOfWork.Wallets.UpdateAsync(wallet);

        return ApiResponse<bool>.SuccessResponse(true);
    }
}
