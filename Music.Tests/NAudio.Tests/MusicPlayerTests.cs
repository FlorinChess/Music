using Music.NAudio;
using NAudio.Wave;

namespace Music.Tests.NAudio.Tests
{
    [TestFixture]
    internal sealed class MusicPlayerTests
    {
        private MusicPlayer _musicPlayer;
        private static readonly string _fileName = Path.Combine(Environment.CurrentDirectory, "NAudio.Tests\\test_music_file.mp3");

        [SetUp]
        public void Setup()
        {
            _musicPlayer = new MusicPlayer(_fileName, 0f);
        }

        [Test]
        public void Stop_PlaybackStateStopped()
        {
            // Arrange
            _musicPlayer.TogglePlayPause();

            // Act
            _musicPlayer.Stop();

            // Assert
            _musicPlayer.PlaybackState.Should().Be(PlaybackState.Stopped);
        }

        [Test]
        public void TogglePlayPause_IsPlaying_PlaybackStatePaused()
        {
            // Arrange
            _musicPlayer.TogglePlayPause();

            // Act
            _musicPlayer.TogglePlayPause();

            // Assert
            _musicPlayer.PlaybackState.Should().Be(PlaybackState.Paused);
        }

        [Test]
        public void TogglePlayPause_IsPaused_PlaybackStatePlaying()
        {
            // Arrange

            // Act
            _musicPlayer.TogglePlayPause();

            // Assert
            _musicPlayer.PlaybackState.Should().Be(PlaybackState.Playing);
        }

        [Test]
        [TestCase(20f, 0.2f)]
        [TestCase(40.4f, 0.404f)]
        [TestCase(100f, 1f)]
        [TestCase(110f, 1f)]
        public void SetVolume_VolumeIsZero_SetVolumeCorrectly(float input, float output)
        {
            // Arrange
            var expectedVolume = Math.Round(output, 3); // Round to avoid floating point error

            // Act
            _musicPlayer.SetVolume(input);
            var volume = Math.Round(_musicPlayer.Volume, 3);

            // Assert
            volume.Should().Be(expectedVolume);
        }

        [Test]
        [TestCase(0.0)]
        [TestCase(2.0)]
        [TestCase(10.0)]
        public void GetPositionInSeconds_PositionShouldBeZero(double postion)
        {
            // Arrange
            _musicPlayer.SetPosition(postion);

            // Act
            var newPosition = _musicPlayer.GetPositionInSeconds();

            // Assert
            newPosition.Should().Be(postion);
        }

        [Test]
        [TestCase(0.0)]
        [TestCase(2.0)]
        [TestCase(10.0)]
        public void SetPosition_ValidPosition_SetPositionCorrectly(double postion)
        {
            // Arrange

            // Act
            _musicPlayer.SetPosition(postion);

            // Assert
            _musicPlayer.GetPositionInSeconds().Should().Be(postion);
        }

        [Test]
        [TestCase(10000.0)]
        [TestCase(-1.0)]
        public void SetPosition_InvalidPosition_DoNothing(double invalidPosition)
        {
            // Arrange

            // Act
            _musicPlayer.SetPosition(invalidPosition);

            // Assert
            _musicPlayer.GetPositionInSeconds().Should().Be(0);
        }

        [Test]
        public void GetLengthInSeconds_ReturnLengthInSeconds()
        {
            // Arrange
            var filePath = Path.Combine(Environment.CurrentDirectory, "NAudio.Tests\\test_music_file.mp3");
            
            var expectedLength = Math.Round(226.056f, 3); // Round to avoid floating point error

            // Act
            var length = Math.Round(MusicPlayer.GetLengthInSeconds(filePath), 3);
            
            // Assert
            length.Should().Be(expectedLength);
        }
    }
}
