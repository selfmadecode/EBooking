using EBooking.Domain.Interfaces;
using EBooking.Infrastructure.Persistence;
using EBooking.Infrastructure.Repositories;

namespace EBooking.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IEventRepository? _events;
        private IBookingRepository? _bookings;
        private IWalletRepository? _wallets;
        private IWalletTransactionRepository? _walletTransactions;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEventRepository Events => _events ??= new EventRepository(_context);
        public IBookingRepository Bookings => _bookings ??= new BookingRepository(_context);
        public IWalletRepository Wallets => _wallets ??= new WalletRepository(_context);
        public IWalletTransactionRepository WalletTransactions => _walletTransactions ??= new WalletTransactionRepository(_context);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
