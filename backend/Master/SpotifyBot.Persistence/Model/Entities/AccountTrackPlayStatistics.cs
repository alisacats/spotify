namespace SpotifyBot.Persistence.Model.Entities
{
    public sealed class AccountTrackPlayStatistics
    {
        public int AccountId { get; set; }
        public string TrackId { get; set; }
        public int CountOfPlays { get; set; }
    }
}
