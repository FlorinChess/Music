using NAudio.CoreAudioApi;
using NAudio.Extras;
using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace Music.NAudio
{
    public sealed class MusicPlayer : IDisposable
    {
        public event Action PlaybackStopped;

        #region Private Members

        private AudioFileReader _reader;
        private WaveOutEvent _player;
        private Equalizer _equalizer;
        private static EqualizerBand[] _bands;

        #endregion Private Members

        #region Properties
        public PlaybackState PlaybackState => _player.PlaybackState;
        public static EqualizerBand[] Bands => _bands ??= CreateEqaulizerBands();

        private float _volume = 0f;
        public float Volume 
        {
            get
            {
                return _player is not null ? _player.Volume : _volume;
            }
            private set
            {
                if(_volume == value) return;

                // Limit 100
                if (value > 100f) value = 100f;

                float newVolume = value / 100f;
                _volume = newVolume;

                if (_player is null) return;

                _player.Volume = newVolume;
            }
        }
        public string FilePath { get; private set; }

        #endregion

        public MusicPlayer(string filePath, float volume)
        {
            FilePath = filePath;

            CreateReader(filePath);
            
            CreateEqualizer();

            CreatePlayer(volume, _equalizer);

            PlaybackStopped = Stop;
        }
        
        private void CreateEqualizer()
        {
            _equalizer = new Equalizer(_reader, Bands);
        }

        private void CreateReader(string filePath)
        {
            _reader?.Dispose();
            _reader = new AudioFileReader(filePath);
        }

        private void CreatePlayer(float volume, Equalizer equalizer)
        {
            Volume = volume;

            _player?.Dispose();
            _player = new WaveOutEvent() { Volume = Volume};
            _player.Init(equalizer);
            _player.OutputWaveFormat.AsStandardWaveFormat();
            _player.PlaybackStopped += OnPlaybackStopped;

        }

        private static EqualizerBand[] CreateEqaulizerBands()
        {
            return new EqualizerBand[]
            {
                new EqualizerBand {Bandwidth = 0.8f, Frequency = 32, Gain = 0},
                new EqualizerBand {Bandwidth = 0.8f, Frequency = 64, Gain = 0},
                new EqualizerBand {Bandwidth = 0.8f, Frequency = 125, Gain = 0},
                new EqualizerBand {Bandwidth = 0.8f, Frequency = 250, Gain = 0},
                new EqualizerBand {Bandwidth = 0.8f, Frequency = 500, Gain = 0},
                new EqualizerBand {Bandwidth = 0.8f, Frequency = 1000, Gain = 0},
                new EqualizerBand {Bandwidth = 0.8f, Frequency = 2000, Gain = 0},
                new EqualizerBand {Bandwidth = 0.8f, Frequency = 4000, Gain = 0},
                new EqualizerBand {Bandwidth = 0.8f, Frequency = 8000, Gain = 0},
                new EqualizerBand {Bandwidth = 0.8f, Frequency = 16000, Gain = 0},
            };
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            PlaybackStopped?.Invoke();
        }

        #region Public Methods


        public void Stop() => _player?.Stop();

        public void TogglePlayPause()
        {
            if (_player?.PlaybackState == PlaybackState.Playing)
            {
                _player?.Pause();
            }
            else
            {
                _player?.Play();
            }
        }

        public double GetPositionInSeconds()
        {
            return _reader is not null ? _reader.CurrentTime.TotalSeconds : 0;
        }

        public void SetPosition(double value)
        {
            if (_reader is null || value > _reader.TotalTime.TotalSeconds || value < 0.0) return;

            _reader.CurrentTime = TimeSpan.FromSeconds(value);
        }

        public void SetVolume(float value)
        {
            if (_player is null) return;

            Volume = value;
        }

        public void UpdateEqualizer()
        {
            _equalizer?.Update();
        }

        public static double GetLengthInSeconds(string filePath)
        {
            using AudioFileReader reader = new(filePath);
            return reader.TotalTime.TotalSeconds;
        }

        public static MMDevice GetDefaultOutputDevice()
        {
            var enumerator = new MMDeviceEnumerator();
            return enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
        }

        public static Dictionary<Guid, string> GetOutputDevices()
        {
            Dictionary<Guid, string> outputDevices = new();

            foreach (var device in DirectSoundOut.Devices)
            {
                if (device.Description == "Primary Sound Driver") continue;

                outputDevices.Add(device.Guid, device.Description);
            }

            return outputDevices;
        }

        #endregion Public Methods

        public void Dispose()
        {
            if (_player is not null)
            {
                if (_player.PlaybackState == PlaybackState.Playing)
                {
                    _player.Stop();
                }
                _player.PlaybackStopped -= OnPlaybackStopped;
                _player.Dispose();
                _player = null;
            }

            _reader?.Dispose();
            _reader = null;
        }
    }
}
