using EBooking.Domain.Entities;
using EBooking.Domain.Interfaces;
using EBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EBooking.Infrastructure.Repositories;

public class WalletTransactionRepository : Repository<WalletTransaction>, IWalletTransactionRepository
{
    public WalletTransactionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<WalletTransaction>> GetTransactionsByWalletIdAsync(Guid walletId) =>
        await _dbSet.AsNoTracking()
            .Where(t => t.WalletId == walletId)
            .OrderByDescending(t => t.CreatedOn)
            .ToListAsync();
}
