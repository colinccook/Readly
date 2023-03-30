using Microsoft.EntityFrameworkCore;
using Readly.Entities;
using Readly.Persistence;

namespace Readly.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ReadlyContext _context;
        public AccountRepository(ReadlyContext context)
        {
            _context = context;
        }
        public async Task<Account?> GetAccount(int accountId)
        {
            return await _context.Customers.Include(c => c.MeterReadings).SingleOrDefaultAsync(c => c.AccountId == accountId);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
