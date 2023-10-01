﻿using Music.Domain.DataModels;
using Music.WPF.Models;
using Music.WPF.Store;

namespace Music.Tests.WPF.Tests.Stores
{
    internal sealed class TrackStoreTests
    {
        private TrackStore _trackStore;

        [SetUp]
        public void Setup()
        {
            _trackStore = new TrackStore();
            _trackStore.QueueChanged += delegate { }; // Test listener
        }

        [Test]
        public void SetQueue_TrackListEmpty_Return()
        {
            // Arrange
            var tracks = new List<TrackModel>();

            // Act
            _trackStore.SetQueue(tracks);

            // Assert
            _trackStore.Queue.Should().BeEmpty();

            _trackStore.CurrentTrack.Should().BeNull();
        }

        [Test]
        public void SetQueue_TrackListNotEmptyAndQueueClear_AddNewTracks()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };
            var track2 = new TrackModel() { Title = "testTitle2" };

            var tracks = new List<TrackModel>
            {
                track1,
                track2,
            };

            // Act
            _trackStore.SetQueue(tracks);

            // Assert
            _trackStore.Queue.Count.Should().Be(2);
            _trackStore.Queue[0].Should().BeEquivalentTo(track1);
            _trackStore.Queue[1].Should().BeEquivalentTo(track2);

            _trackStore.CurrentTrack.Should().BeEquivalentTo(track1);
        }

        [Test]
        public void SetQueue_TrackListNotEmptyAndQueueNotClear_ClearAndAddNewTracks()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };
            var track2 = new TrackModel() { Title = "testTitle2" };
            var track3 = new TrackModel() { Title = "testTitle3" };
            var track4 = new TrackModel() { Title = "testTitle4" };

            var tracks = new List<TrackModel>
            {
                track1,
                track2,
                track3,
                track4,
            };

            _trackStore.SetQueue(tracks);

            var newTracks = new List<TrackModel>
            {
                track2,
                track4,
            };

            // Act
            _trackStore.SetQueue(newTracks);

            // Assert
            _trackStore.Queue.Count.Should().Be(2);
            _trackStore.Queue[0].Should().BeEquivalentTo(track2);
            _trackStore.Queue[1].Should().BeEquivalentTo(track4);

            _trackStore.CurrentTrack.Should().BeEquivalentTo(track2);
        }

        [Test]
        public void SetQueue_TrackListNotEmptyAndQueueNotClearAndSetFirstFalse_ClearAndAddNewTracks()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };
            var track2 = new TrackModel() { Title = "testTitle2" };
            var track3 = new TrackModel() { Title = "testTitle3" };
            var track4 = new TrackModel() { Title = "testTitle4" };

            var tracks = new List<TrackModel>
            {
                track1,
                track2,
                track3,
                track4,
            };

            _trackStore.SetQueue(tracks);

            var newTracks = new List<TrackModel>
            {
                track2,
                track4,
            };

            // Act
            _trackStore.SetQueue(newTracks, false);

            // Assert
            _trackStore.Queue.Count.Should().Be(2);
            _trackStore.Queue[0].Should().BeEquivalentTo(track2);
            _trackStore.Queue[1].Should().BeEquivalentTo(track4);

            _trackStore.CurrentTrack.Should().BeEquivalentTo(track1);
        }

        [Test]
        public void SetQueue_OneNewTrackAndQueueIsEmpty_AddNewTrack()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };

            _trackStore.AddToQueue(track1);

            // Act
            _trackStore.SetQueue(track1);

            // Assert
            _trackStore.Queue.Count.Should().Be(1);
            _trackStore.CurrentTrack.Should().BeEquivalentTo(track1);
        }

        [Test]
        public void SetQueue_OneNewTrackAndQueueIsNotEmpty_ClearAndAddNewTrack()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };
            var track2 = new TrackModel() { Title = "testTitle2" };
            var track3 = new TrackModel() { Title = "testTitle3" };
            var track4 = new TrackModel() { Title = "testTitle4" };

            var tracks = new List<TrackModel>
            {
                track1,
                track2,
                track3,
                track4,
            };

            _trackStore.SetQueue(tracks);

            // Act
            _trackStore.SetQueue(track3);

            // Assert
            _trackStore.Queue.Count.Should().Be(1);
            _trackStore.CurrentTrack.Should().BeEquivalentTo(track3);
        }

        [Test]
        public void SetQueue_OneNewTrackAndQueueIsNotEmptyAndSetFirstFalse_ClearAndAddNewTrack()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };
            var track2 = new TrackModel() { Title = "testTitle2" };
            var track3 = new TrackModel() { Title = "testTitle3" };
            var track4 = new TrackModel() { Title = "testTitle4" };

            var tracks = new List<TrackModel>
            {
                track1,
                track2,
                track3,
                track4,
            };

            _trackStore.SetQueue(tracks);

            // Act
            _trackStore.SetQueue(track3, false);

            // Assert
            _trackStore.Queue.Count.Should().Be(1);
            _trackStore.CurrentTrack.Should().BeEquivalentTo(track1);
        }

        [Test]
        public void AddToQueue_TrackListEmpty_Return()
        {
            // Arrange
            var tracks = new List<TrackModel>();

            // Act
            _trackStore.AddToQueue(tracks);

            // Assert
            _trackStore.Queue.Should().BeEmpty();
            _trackStore.CurrentTrack.Should().BeNull();
        }

        [Test]
        public void AddToQueue_TrackListNotEmptyAndQueueIsEmpty_AddNewTracks()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };
            var track2 = new TrackModel() { Title = "testTitle2" };

            var tracks = new List<TrackModel>()
            { 
                track1, 
                track2,
            };

            // Act
            _trackStore.AddToQueue(tracks);

            // Assert
            _trackStore.Queue.Count.Should().Be(2);
            
            // TODO: check if bahavior is correct when calling QueueChanged()
            //_trackStore.CurrentTrack.Should().BeNull();

            //_trackStore.Monitor().Should().Raise(nameof(_trackStore.QueueChanged));
        }

        [Test]
        public void AddToQueue_OneNewTrackAndQueueEmpty_AddNewTrack()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };

            // Act
            _trackStore.AddToQueue(track1);

            // Assert
            _trackStore.Queue.Count.Should().Be(1);

            //_trackStore.Monitor().Should().Raise(nameof(_trackStore.QueueChanged));
        }

        [Test]
        public void AddToQueue_OneNewTrackAndQueueNotEmpty_AddNewTrack()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };
            var track2 = new TrackModel() { Title = "testTitle2" };

            var tracks = new List<TrackModel>() 
            { 
                track1, 
                track2,
            };

            _trackStore.SetQueue(tracks);

            // Act
            _trackStore.AddToQueue(track1);

            // Assert
            _trackStore.Queue.Count.Should().Be(3);
            _trackStore.Queue[2].Should().BeEquivalentTo(track1);
            _trackStore.CurrentTrack.Should().BeEquivalentTo(track1);

            //_trackStore.Monitor().Should().Raise(nameof(_trackStore.QueueChanged));
        }

        [Test]
        public void AddNextInQueue_QueueEmpty_AddNewTrack()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };

            // Act
            _trackStore.AddNextInQueue(track1);

            // Assert
            _trackStore.Queue.Count.Should().Be(1);
            _trackStore.CurrentTrack.Should().BeEquivalentTo(track1);

        }

        [Test]
        public void AddNextInQueue_QueueNotEmpty_AddNewTrack()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };
            var track2 = new TrackModel() { Title = "testTitle2" };
            var track3 = new TrackModel() { Title = "testTitle3" };

            var tracks = new List<TrackModel>()
            { 
                track1,
                track2, // <-- Current
                track3,
            };

            _trackStore.SetQueue(tracks);
            _trackStore.SetTrackAsCurrent(track2);

            // Act
            _trackStore.AddNextInQueue(track1);

            // Assert
            _trackStore.Queue.Count.Should().Be(4);
            _trackStore.Queue[1].Should().BeEquivalentTo(track2);
            _trackStore.Queue[2].Should().BeEquivalentTo(track1);
            _trackStore.CurrentTrack.Should().BeEquivalentTo(track2);
        }

        [Test]
        public void ClearQueue_QueueEmpty_Return()
        {
            // Arrange
            _trackStore.SetQueue(new List<TrackModel>());

            // Act
            _trackStore.ClearQueue();

            // Assert
            _trackStore.Queue.Should().BeEmpty();
        }

        [Test]
        public void ClearQueue_QueueNotEmpty_ClearQueue()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };
            var track2 = new TrackModel() { Title = "testTitle2" };
            var track3 = new TrackModel() { Title = "testTitle3" };

            var tracks = new List<TrackModel>()
            {
                track1,
                track2,
                track3,
            };

            _trackStore.SetQueue(tracks);

            // Act
            _trackStore.ClearQueue();

            // Assert
            _trackStore.Queue.Should().BeEmpty();
        }

        [Test]
        public void SetCurrentPlaylist_CurrentPlaylistNull_SetCurrentPlaylist()
        {
            // Arrange
            var playlist1 = new PlaylistModel() { Name = "testPlaylist1" };

            // Act
            _trackStore.SetCurrentPlaylist(playlist1);

            // Assert
            _trackStore.CurrentPlaylist.Should().BeEquivalentTo(playlist1);
        }

        [Test]
        public void AddPlaylist_AvailablePlaylistsEmpty_AddPlaylist()
        {
            // Arrange
            var playlist1 = new PlaylistModel() { Name = "testPlaylist1" };

            // Act
            _trackStore.AddPlaylist(playlist1);

            // Assert
            _trackStore.AvailablePlaylists.Count.Should().Be(1);
        }

        [Test]
        public void AddPlaylist_AvailablePlaylistsNotEmpty_AddPlaylist()
        {
            // Arrange
            var playlist1 = new PlaylistModel() { Name = "testPlaylist1" };
            var playlist2 = new PlaylistModel() { Name = "testPlaylist1" };

            _trackStore.AddPlaylist(playlist1);

            // Act
            _trackStore.AddPlaylist(playlist2);

            // Assert
            _trackStore.AvailablePlaylists.Count.Should().Be(2);
            _trackStore.AvailablePlaylists[0].Should().BeEquivalentTo(playlist1);
            _trackStore.AvailablePlaylists[1].Should().BeEquivalentTo(playlist2);
        }

        [Test]
        public void AddTracks_TrackListEmpty_Return()
        {
            // Arrange
            var tracks = new List<TrackModel>();

            // Act
            _trackStore.AddTracks(tracks);

            // Assert
            _trackStore.AvailableTracks.Should().BeEmpty();
        }

        [Test]
        public void AddTracks_TrackListNotEmptyAndAvailableTracksEmpty_AddNewTracks()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };
            var track2 = new TrackModel() { Title = "testTitle2" };

            var tracks = new List<TrackModel>()
            {
                track1,
                track2,
            };

            // Act
            _trackStore.AddTracks(tracks);

            // Assert
            _trackStore.AvailableTracks.Count.Should().Be(2);
            _trackStore.AvailableTracks[0].Should().BeEquivalentTo(track1);
            _trackStore.AvailableTracks[1].Should().BeEquivalentTo(track2);
        }

        [Test]
        public void AddTracks_TrackListNotEmptyAndAvailableTracksNotEmpty_AddNewTracks()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };
            var track2 = new TrackModel() { Title = "testTitle2" };
            var track3 = new TrackModel() { Title = "testTitle3" };
            var track4 = new TrackModel() { Title = "testTitle4" };

            var tracks = new List<TrackModel>()
            {
                track1,
                track2,
            };

            _trackStore.AddTracks(tracks);

            var newTracks = new List<TrackModel>()
            {
                track3,
                track4,
            };

            // Act
            _trackStore.AddTracks(newTracks);

            // Assert
            _trackStore.AvailableTracks.Count.Should().Be(4);
            _trackStore.AvailableTracks[0].Should().BeEquivalentTo(track1);
            _trackStore.AvailableTracks[1].Should().BeEquivalentTo(track2);
            _trackStore.AvailableTracks[2].Should().BeEquivalentTo(track3);
            _trackStore.AvailableTracks[3].Should().BeEquivalentTo(track4);
        }

        [Test]
        public void SetTrackAsCurrent_CurrentTrackIsNull_SetCurrentTrack()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };
            _trackStore.AddTracks(new List<TrackModel>() { track1 });

            // Act
            _trackStore.SetTrackAsCurrent(_trackStore.AvailableTracks[0]);

            // Assert
            _trackStore.CurrentTrack.Should().BeEquivalentTo(track1);
        }

        [Test]
        public void SetTrackAsCurrent_CurrentTrackIsSame_Return()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };
            _trackStore.AddTracks(new List<TrackModel>() { track1 });
            _trackStore.SetTrackAsCurrent(_trackStore.AvailableTracks[0]);

            // Act
            _trackStore.SetTrackAsCurrent(track1);

            // Assert
            _trackStore.CurrentTrack.Should().BeEquivalentTo(track1);
        }

        [Test]
        public void SetTrackAsCurrent_CurrentTrackIsNotNull_SetCurrentTrack()
        {
            // Arrange
            var track1 = new TrackModel() { Title = "testTitle1" };
            var track2 = new TrackModel() { Title = "testTitle2" };
            _trackStore.AddTracks(new List<TrackModel>() { track1, track2 });
            _trackStore.SetQueue(_trackStore.AvailableTracks[0]);

            // Act
            _trackStore.SetTrackAsCurrent(track2);

            // Assert
            _trackStore.CurrentTrack.Should().BeEquivalentTo(track2);
        }

        [Test]
        public void RemovePlaylist_AvailablePlaylistsEmpty_Return()
        {
            // Arrange
            var playlist1 = new PlaylistModel() { Name = "testPlaylist1" };

            // Act
            _trackStore.RemovePlaylist(playlist1);

            // Assert
            _trackStore.AvailablePlaylists.Should().BeEmpty();
        }

        [Test]
        public void RemovePlaylist_AvailablePlaylistsNotEmpty_RemovePlaylist()
        {
            // Arrange
            var playlist1 = new PlaylistModel() { Name = "testPlaylist1" };
            var playlist2 = new PlaylistModel() { Name = "testPlaylist2" };

            _trackStore.AddPlaylist(playlist1);
            _trackStore.AddPlaylist(playlist2);

            // Act
            _trackStore.RemovePlaylist(playlist1);

            // Assert
            _trackStore.AvailablePlaylists.Count.Should().Be(1);
        }

        [Test]
        public void PlaylistUpdated_PlaylistsChangedShouldBeTrue()
        {
            // Arrange

            // Act
            _trackStore.PlaylistUpdated();

            // Assert
            _trackStore.PlaylistsChanged.Should().BeTrue();
        }

        [Test]
        public void GetTrackByFilePath_TrackFound_ReturnTrack()
        {
            // Arrange
            var filePath = @"D:\Music";

            var track1 = new TrackModel() { FilePath = filePath };
            
            _trackStore.AddTracks(new List<TrackModel> { track1 });

            // Act
            var result = _trackStore.GetTrackByFilePath(filePath);

            // Assert
            result.Should().BeEquivalentTo(track1);
        }

        [Test]
        public void GetTrackByFilePath_MultipleTrackWithSameFilePath_ThrowException()
        {
            // Arrange
            var filePath = @"D:\Music";

            var track1 = new TrackModel() { FilePath = filePath };
            var track2 = new TrackModel() { FilePath = filePath };

            _trackStore.AddTracks(new List<TrackModel> { track1, track2 });

            // Act
            // Assert
            Assert.Throws<InvalidOperationException>(() => _trackStore.GetTrackByFilePath(filePath));
        }

        [Test]
        public void AddMusicFolder_MusicFolderIsEmpty_AddMusicFolder()
        {
            // Arrange
            var musicFolder = new MusicFolderModel() { Path = @"D:\Music"};

            // Act
            _trackStore.AddMusicFolder(musicFolder);

            // Assert
            _trackStore.MusicFolders.Count.Should().Be(1);
        }
    }
}