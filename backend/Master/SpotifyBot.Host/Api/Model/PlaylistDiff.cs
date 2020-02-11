namespace SpotifyBot.Host.Api.Model
{
    public struct PlaylistDiff
    {
        public Track[] TracksToAdd { get; set; }
        public Track[] TracksToRemove { get; set; }
    }
}
