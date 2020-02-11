namespace SpotifyBot.Persistence.Model
{
    public class AccountInfo
    {
        public SpotifyCredentials SpotifyCredentials { get; set; }
        public string Proxy { get; set; }

        public int AccountId { get; set; }
    }
}
