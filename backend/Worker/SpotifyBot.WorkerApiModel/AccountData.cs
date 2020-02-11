namespace SpotifyBot.WorkerApiModel
{
    public class AccountData
    {
        public SpotifyCredentials SpotifyCredentials { get; set; }

        /// <summary>
        /// USERNAME:PASSWORD@PROXYIP:PROXYPORT
        /// </summary>
        public string Proxy { get; set; }
    }
}
