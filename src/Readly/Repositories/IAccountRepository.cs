using Readly.Entities;

namespace Readly.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccount(int accountId);
        Task Save();
    }
}