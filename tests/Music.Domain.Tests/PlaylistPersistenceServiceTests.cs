using FluentAssertions;
using Music.Domain.DataModels;

namespace Music.Domain.Tests;

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
        var playlistsXmlFileString = await File.ReadAllTextAsync(Path.Combine(Environment.CurrentDirectory, "test_playlists.xml"));

        var testExpectedResultFilePath = Path.Combine(Environment.CurrentDirectory, "test_playlists_expected.xml");
        _playlistPersistenceService.SaveFilePath = testExpectedResultFilePath;
        _playlistPersistenceService.Add("testName", new DateOnly(2010, 1, 1), string.Empty,
        [
            @"D:\Music\Test1.mp3",
            @"D:\Music\Test2.mp3",
        ]);


        // Act
        _playlistPersistenceService.Save();

        // Assert
        var result = await File.ReadAllTextAsync(testExpectedResultFilePath);
        result.Should().Be(playlistsXmlFileString);

        File.WriteAllText(testExpectedResultFilePath, string.Empty);
    }

    [Test]
    public void Parse_ValidPlaylistFile_ReturnObjectsList()
    {
        // Arrange
        var playlist1 = new Playlist()
        {
            Name = "testName",
            DateCreated = new DateOnly(2010, 1, 1),
            ImagePath = string.Empty,
            TracksFilePaths =
            [
                @"D:\Music\Test1.mp3",
                @"D:\Music\Test2.mp3",
            ]
        };

        var playlists = new List<Playlist> { playlist1 };

        // Read from the test file, not the actual save file
        _playlistPersistenceService.SaveFilePath = Path.Combine(Environment.CurrentDirectory, "test_playlists.xml");

        // Act
        var result = _playlistPersistenceService.Parse();

        // Assert
        result.Should().NotBeNull();
        result.Playlists.Should().HaveCount(1);
        result.Playlists.Should().BeEquivalentTo(playlists);
    }
}
