using Music.APIs.Extensions;
using Music.APIs.Spotify;
using Music.APIs.Spotify.Models;
using System.Diagnostics;
using System.Globalization;
using TagLib;

using TFile = TagLib.File;

namespace Music.APIs
{
    public sealed class MusicMetadataService
    {
        private static readonly string METADATA_COMPLETE = "Metadata complete.";
        private readonly SpotifyService _service;
        private readonly List<MusicFileMetadataModel> _musicFiles = new();

        public MusicMetadataService(SpotifyService service)
        {
            _service = service;
        }

        public void AddMusicFiles(IEnumerable<string> filePaths)
        {
            TFile? file = null;

            foreach (var filePath in filePaths)
            {
                try
                {
                    file = TFile.Create(filePath);

                    if (file.Tag.Comment == METADATA_COMPLETE) continue;

                    _musicFiles.Add(new MusicFileMetadataModel()
                    {
                        FilePath = filePath,
                        Title = file.Tag.Title,
                        Performers = file.Tag.Performers,
                        AlbumArtists = file.Tag.AlbumArtists,
                        Album = file.Tag.Album,
                        Genres = file.Tag.Genres,
                        Year = file.Tag.Year,
                        HasPictures = file.Tag.Pictures is not null && file.Tag.Pictures.Length > 0,
                    });
                }
                catch (CorruptFileException)
                {
                    Debug.WriteLine("The file located at " + filePath + " is corrupted!");

                    // Handle corrupted file
                    //throw;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception thrown in MusicMetadataService:");
                    Debug.WriteLine(ex);
                }
                finally
                {
                    file?.Dispose();
                }
            }
        }

        public async Task FillMetadataAsync()
        {
            if (_musicFiles.Count == 0) return;

            try
            {
                CleanUpMetadata(_musicFiles);

                await CompleteMetadata(_musicFiles);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void CleanUpMetadata(IList<MusicFileMetadataModel> musicFiles)
        {
            TFile? file = null;

            for (int i = 0; i < musicFiles.Count; i++)
            {
                try
                {
                    file = TFile.Create(musicFiles[i].FilePath);

                    file.RemoveTags(TagTypes.AllTags);
                    file.Save();

                    musicFiles[i].TagsCleared = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    throw;
                }
                finally
                {
                    file?.Dispose();
                }
            }
        }

        private async Task CompleteMetadata(IList<MusicFileMetadataModel> musicFiles)
        {
            try
            {
                for (int i = 0; i < musicFiles.Count; i++)
                {
                    Item? item = await GetDataItem(musicFiles[i]);
                    
                    if (item is not null)
                        FillMusicFileMetadata(musicFiles[i], item);
                    else
                        AddOriginalMusicFileMetadata(musicFiles[i]);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // TODO: test a lot
        private async Task<Item?> GetDataItem(MusicFileMetadataModel musicFile)
        {
            int numberOfCalls = 0;
            int maxNumberOfRetries = musicFile.Performers.Length;
            ushort limit = 5;
            int performerIndex = 0;

            Tracks? tracks = await _service.SearchTrackAsync(musicFile.Title);
            Item? item;

            try
            {
                while (!ResponseMatchesArtistName(musicFile, tracks, out item))
                {
                    if (numberOfCalls == maxNumberOfRetries) break;

                    tracks = await _service.SearchTrackAsync(musicFile.Title, musicFile.Performers[performerIndex], limit);
                    numberOfCalls++;

                    performerIndex = (performerIndex < maxNumberOfRetries) ? performerIndex++ : 0;
                }
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static bool ResponseMatchesArtistName(MusicFileMetadataModel musicFile, Tracks? tracks, out Item? matchingItem)
        {
            if (tracks is not null && tracks.Items.Length != 0)
            {
                for (int i = 0; i < tracks.Items.Length; i++)
                {
                    Item? item = tracks.Items[i];

                    for (int j = 0; j < item.Artists.Length; j++)
                    {
                        Artist? artist = item.Artists[j];

                        for (int k = 0; k < musicFile.Performers.Length; k++)
                        {
                            string? artistFromFile = musicFile.Performers[k];
                            var normalizedAritstName = artist.Name.NormalizeString().RemoveForbiddenCharacters();
                            var normalizedArtistFromFile = artistFromFile.NormalizeString().RemoveForbiddenCharacters();

                            if (string.Compare(normalizedAritstName, normalizedArtistFromFile, true, CultureInfo.InvariantCulture) == 0)
                            {
                                matchingItem = item;
                                return true;
                            }
                        }
                    }
                }
            }

            matchingItem = null;
            return false;
        }
        private static void AddOriginalMusicFileMetadata(MusicFileMetadataModel musicFile)
        {
            TFile? file = null;

            try
            {
                file = TFile.Create(musicFile.FilePath);
                file.Tag.Title = musicFile.Title;
                file.Tag.Performers = musicFile.Performers;
                file.Tag.Year = musicFile.Year;

                file.Save();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
            finally
            {
                file?.Dispose();
            }
        }

        private static void FillMusicFileMetadata(MusicFileMetadataModel musicFile, Item item)
        {
            TFile? file = null;

            try
            {
                file = TFile.Create(musicFile.FilePath);

                file.Tag.Title = musicFile.Title;
                file.Tag.Performers = musicFile.Performers;
                file.Tag.Album = item.Album.Name;
                file.Tag.Year = item.Album.ReleaseDate.GetYear();
                file.Tag.AlbumArtists = item.Album.Artists.Select(a => a.Name).ToArray();

                Task.Run(async () =>
                {
                    var picture = await GetAlbumArtAsync(item.Album.Images[0].Url);
                    file.Tag.Pictures = new IPicture[]
                    {
                        new Picture()
                        {
                            Type = PictureType.FrontCover,
                            Description = "Cover",
                            MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                            Data = ByteVector.FromStream(picture)
                        }
                    };
                }).Wait();

                file.Tag.Comment = METADATA_COMPLETE;

                file.Save();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
            finally
            {
                file?.Dispose();
            }
        }

        private static async Task<Stream> GetAlbumArtAsync(string url)
        {
            using HttpClient client = new();

            var result = await client.GetAsync(url);
            var stream = await result.Content.ReadAsStreamAsync();

            return stream;
        }

        private static void RemoveCompleteStatus(MusicFileMetadataModel musicFile)
        {
            TFile file = TFile.Create(musicFile.FilePath);

            file.Tag.Comment = string.Empty;
            file.Save();
        }
    }
}
