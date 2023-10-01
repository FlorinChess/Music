using Music.Domain;
using Music.WPF.Models;

namespace Music.Tests.Domain.Tests
{
    [TestFixture]
    internal sealed class PlaylistPersistenceServiceTests
    {
        private PlaylistPersistenceService _playlistPersistenceService; 

        [SetUp]
        public void Setup()
        {
            _playlistPersistenceService = new();
        }

        [Test]
        public async Task Save_NoErrors_ShouldSaveProperly()
        {
            // Arrange
            var playlistsXmlFileString = await File.ReadAllTextAsync(Path.Combine(Environment.CurrentDirectory, "Domain.Tests\\test_playlists.xml"));

            _playlistPersistenceService.Add("testName", "01/01/2010", string.Empty, new List<string>()
            {
                @"D:\Music\Test1.mp3",
                @"D:\Music\Test2.mp3",
            });

            // Act
            _playlistPersistenceService.Save();

            // Assert
            var result = await File.ReadAllTextAsync(_playlistPersistenceService.SaveFilePath);
            result.Should().Be(playlistsXmlFileString);
        }

        [Test]
        public void Parse_ValidPlaylistFile_ReturnObjectsList()
        {
            // Arrange
            var playlist1 = new PlaylistModel()
            {
                Name = "testName",
                DateCreated = new DateOnly(2010, 1, 1),
                ImagePath = string.Empty,
                Tracks = new List<TrackModel>()
                {
                    new TrackModel() { FilePath = @"D:\Music\Test1.mp3" },
                    new TrackModel() { FilePath = @"D:\Music\Test2.mp3" },
                }
            };

            var playlists = new List<PlaylistModel> { playlist1 };

            // Act
            var result = _playlistPersistenceService.Parse();

            // Assert
            result.Should().NotBeNull();

            var resultingPlaylists = new List<PlaylistModel>();

            for (int i = 0; i < result?.Playlists.Count; i++)
            {
                var currentPlaylist = result.Playlists[i];

                resultingPlaylists.Add(new PlaylistModel()
                {
                    Name = currentPlaylist.Name,
                    DateCreated = DateOnly.Parse(currentPlaylist.DateCreatedString),
                    ImagePath = currentPlaylist.ImagePath,
                });

                for (int j = 0; j < currentPlaylist.TracksFilePaths.Count; j++)
                {
                    resultingPlaylists[i].Tracks.Add(new TrackModel() { FilePath = currentPlaylist.TracksFilePaths[j] });
                }
            }

            resultingPlaylists.Should().BeEquivalentTo(playlists);
        }
    }
}
