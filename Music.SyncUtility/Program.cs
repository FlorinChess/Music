using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Music.APIs.Spotify;
using Music.APIs.Spotify.Models;

Main(null);

async static void Main(string[] args)
{ 
  Console.WriteLine("Hello, World1!");
  
  // 1. get access to Spotify API
  var services = new ServiceCollection();
  services.AddHttpClient(); // registers IHttpClientFactory

  var serviceProvider = services.BuildServiceProvider();
  SpotifyService service = new SpotifyService(serviceProvider.GetRequiredService<IHttpClientFactory>());
  Console.WriteLine("Spotify API initialized");

  // 2. pull Spotify playlist
  string playlistId = "0ncfcOY6o1PhQqcuwGw2W9";
  Tracks tracks = await service.GetPlaylist(playlistId);

  foreach(var track in tracks.Items)
  {
    Console.WriteLine(track);
  }

  // 3. convert each entry in Spotify into models

  // 4. for each model, check against local database
}

Console.WriteLine("Hello, World!");
