using EBooking.Domain.Entities;

namespace EBooking.Domain.Interfaces;

public interface IWalletTransactionRepository : IRepository<WalletTransaction>
{
    Task<IEnumerable<WalletTransaction>> GetTransactionsByWalletIdAsync(Guid walletId);
}
