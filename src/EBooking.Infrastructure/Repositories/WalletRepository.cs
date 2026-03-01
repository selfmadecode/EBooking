using EBooking.Domain.Entities;
using EBooking.Domain.Interfaces;
using EBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EBooking.Infrastructure.Repositories;

public class WalletRepository : Repository<Wallet>, IWalletRepository
{
    public WalletRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Wallet?> GetWalletByUserIdAsync(Guid userId) =>
        await _dbSet.FirstOrDefaultAsync(w => w.UserId == userId);

    public async Task<Wallet?> GetWalletWithTransactionsAsync(Guid userId) =>
        await _dbSet.Include(w => w.Transactions.OrderByDescending(t => t.CreatedOn))
            .FirstOrDefaultAsync(w => w.UserId == userId);
}
