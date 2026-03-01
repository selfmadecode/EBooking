using EBooking.Domain.Entities;

namespace EBooking.Domain.Interfaces
{
    public interface IWalletRepository : IRepository<Wallet>
    {
        Task<Wallet?> GetWalletByUserIdAsync(Guid userId);
        Task<Wallet?> GetWalletWithTransactionsAsync(Guid userId);
    }
}
