using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyBot.Host.Api.Model;
using SpotifyBot.Persistence;
using SpotifyBot.Persistence.Model;

namespace SpotifyBot.Host.Api
{
    public class SpotifyServiceGroup
    {
        private Dictionary<int, SpotifyService> _spotifyServices;
        private StorageUowProvider _storageUowProvider;
        private SpotifyServiceGroup(Dictionary<int, SpotifyService> spotifyServices, StorageUowProvider storageUowProvider)
        {
            _storageUowProvider = storageUowProvider;
            _spotifyServices = spotifyServices;
        }

        public static async Task AddAccounts(SpotifyAccountsConfig config, StorageUowProvider storageUowProvider)
        {
            foreach (var accountInfo in config.Accounts)
            {
                using (var uow = storageUowProvider.CreateUow())
                {
                    var proxyInfo = accountInfo.Proxy.Split(":");
                    var proxy = uow.ProxyStorageService.AddProxy(proxyInfo[0], Convert.ToUInt16(proxyInfo[1]));

                    await uow.AccountStorageService.AddAccount(accountInfo.SpotifyCredentials.Login,
                        accountInfo.SpotifyCredentials.Password, proxy.Id);
                    await uow.ApplyChanges();
                }
            }
        }

        public static async Task<SpotifyServiceGroup> Create(SpotifyAccountsConfig config, StorageUowProvider storageUowProvider)
        {
            await AddAccounts(config, storageUowProvider);

            AccountInfo[] accounts;
            using (var uow = storageUowProvider.CreateUow())
            {
                accounts = await uow.AccountStorageService.GetAllAccounts();
                await uow.ApplyChanges();
            }

            var spotifyServices = new Dictionary<int, SpotifyService>();
            foreach (var accountInfo in accounts)
            {
                accountInfo.Proxy = null; //TODO: delete after proxy purchase
                spotifyServices.Add(accountInfo.AccountId, await SpotifyService.Create(accountInfo));
            }

            return new SpotifyServiceGroup(spotifyServices, storageUowProvider);
        }

        public SpotifyService GetService(int accountId)
        {
            return _spotifyServices[accountId];
        }

        public async Task<AccountBriefInfo[]> GetBriefInfo()
        {
            AccountInfo[] accounts;
            using (var uow = _storageUowProvider.CreateUow())
            {
                accounts = await uow.AccountStorageService.GetAllAccounts();
                await uow.ApplyChanges();
            }

            var briefInfo = new List<AccountBriefInfo>();
            foreach (var accountInfo in accounts)
            {
                var accountId = accountInfo.AccountId;
                briefInfo.Add(new AccountBriefInfo()
                {
                    Email = accountInfo.SpotifyCredentials.Login,
                    AccountId = accountId,
                    AccountState = new AccountState(){ IsPlaying = GetService(accountId).IsPlaying() }
                });
            }

            return briefInfo.ToArray();
        }

        public async Task<Track[]> GetPlaylist(StorageUowProvider storageUowProvider)
        {
            using (var uow = storageUowProvider.CreateUow())
            {
                var tracks = await uow.TrackStorageService.GetAllTracks();
                return tracks.Select(x => new Track() { Id = x.Id, Title = x.Title }).ToArray();
            }
        }
    }
}
