namespace SpotifyBot.Persistence.Model.Entities
{
    public sealed class Proxy
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public ushort Port { get; set; }
    }
}
