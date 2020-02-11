using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SpotifyBot.WorkerHost
{
    public sealed class Config
    {
        public string MasterEndpoint { get; set; }
        public int ConnectJitterSeconds { get; set; }

        const string FileName = "config.json";
        public static async Task<Config> Read()
        {
            var json = await File.ReadAllTextAsync(FileName);
            return JsonConvert.DeserializeObject<Config>(json);
        }
    }
}
