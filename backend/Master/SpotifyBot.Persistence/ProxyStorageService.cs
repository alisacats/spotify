using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpotifyBot.Persistence.Model.Entities;

namespace SpotifyBot.Persistence
{
    public sealed class ProxyStorageService
    {
        readonly StorageUow _uow;
        StorageDbContext Db => _uow.Context;
        public ProxyStorageService(StorageUow uow) => _uow = uow;

        public async Task<Proxy> GetProxy(int proxyId)
        {
            return await Db.Proxies.Where(x => x.Id == proxyId).FirstOrDefaultAsync();
        }

        public async Task<Proxy> AddProxy(string ipAdress, ushort port)
        {
            var proxy = new Proxy { Port = port, IpAddress = ipAdress};
            await Db.Proxies.AddAsync(proxy);
            await Db.SaveChangesAsync();
            
            return proxy;
        }
    }
}
