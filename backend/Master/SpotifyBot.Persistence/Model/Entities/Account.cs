namespace SpotifyBot.Persistence.Model.Entities
{
    public sealed class Account
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int CurrentProxyId { get; set; }
    }
}
