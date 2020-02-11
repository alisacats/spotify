using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpotifyBot.Persistence.Model;
using SpotifyBot.Persistence.Model.Entities;

namespace SpotifyBot.Persistence
{
    public sealed class AccountStorageService
    {
        readonly StorageUow _uow;
        StorageDbContext Db => _uow.Context;
        public AccountStorageService(StorageUow uow) => _uow = uow;

        public async Task AddAccount(string login, string password, int proxyId)
        {
            var account = new Account()
            {
                Email = login,
                Password = password,
                CurrentProxyId = proxyId
            };

            var dbAccount = await Db.Accounts.FirstOrDefaultAsync(x => x.Email == account.Email);
            if (dbAccount == null) await Db.Accounts.AddAsync(account);
        }

        public async Task<AccountInfo[]> GetAllAccounts() => await Db
            .Accounts
            .Join(Db.Proxies,
                a => a.CurrentProxyId,
                p => p.Id,
                (a, p) => new AccountInfo()
                {
                    AccountId = a.AccountId,
                    Proxy = $"{p.IpAddress}:{p.Port}",
                    SpotifyCredentials = new SpotifyCredentials()
                    {
                        Login = a.Email,
                        Password = a.Password
                    }
                })
            .ToArrayAsync();
    }
}
