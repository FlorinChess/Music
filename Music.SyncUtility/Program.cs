using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Music.APIs.Spotify;
using Music.APIs.Spotify.Models;

namespace Music.SyncUtility
{
    internal class Program
    {
        private static readonly List<Track> Tracks = new List<Track>();
        const string PLAYLIST_NAME = "Salsa";
        const string PLAYLIST_ID = "0ncfcOY6o1PhQqcuwGw2W9";
        static async Task Main(string[] args)
        {
            Console.Write("Args: ");
            Console.WriteLine(args);

            // 1. Get Spotify playlist
            var playlistResponse = await GetPlaylist(PLAYLIST_ID);

            // 2. Map Spotify tracks to models
            var playlistTracks = MapPlaylistResponseToList(playlistResponse);
            
            // 3.

        }

        private static async Task<PlaylistResponse> GetPlaylist(string playlistId)
        {
            var services = new ServiceCollection();

            services.AddHttpClient();

            var serviceProvider = services.BuildServiceProvider();

            SpotifyService spotifyService = new SpotifyService(serviceProvider.GetRequiredService<IHttpClientFactory>());

            Thread.Sleep(5000);

            PlaylistResponse playlistResponse = await spotifyService.GetPlaylist(playlistId);

            if (playlistResponse == null)
                throw new InvalidOperationException("Playlist is null!");

            if (playlistResponse.Name != PLAYLIST_NAME)
                throw new InvalidOperationException("Incorrect playlist fetched!");

            return playlistResponse;
        }

        private static List<Track> MapPlaylistResponseToList(PlaylistResponse playlistResponse)
        {
            var list = new List<Track>();
            for (int i = 0; i < playlistResponse.PlaylistTracks.PlaylistTrackObjects.Count(); i++)
            {
                Track track = playlistResponse.PlaylistTracks.PlaylistTrackObjects[i].Track;
                list.Add(track);
                Console.WriteLine(track);
            }

            return list;
        }
    }
}
