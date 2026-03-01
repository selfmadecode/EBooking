namespace EBooking.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEventRepository Events { get; }
    IBookingRepository Bookings { get; }
    IWalletRepository Wallets { get; }
    IWalletTransactionRepository WalletTransactions { get; }
    Task<int> SaveChangesAsync();
}
