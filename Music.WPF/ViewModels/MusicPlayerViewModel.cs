using Music.NAudio;
using Music.WPF.Commands;
using Music.WPF.Modals.ViewModels;
using Music.WPF.Models;
using Music.WPF.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Windows.Input;

namespace Music.WPF.ViewModels
{
    public sealed class MusicPlayerViewModel : BaseViewModel
    {
        #region Public Events

        public event Action PlayingStateChanged;
        public event Action NavigateToNowPlaying;

        #endregion Public Events

        #region Private Members

        private bool _isSeeking = false;
        private float _savedVolume;
        private MusicPlayer _musicPlayer;
        private readonly Timer _timer;
        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly TrackStore _trackStore;
        private readonly List<TrackModel> _queue;
        private ICommand _playPauseCommand;
        private ICommand _playNextCommand;
        private ICommand _playPreviousCommand;
        private ICommand _seekDragCompletedCommand;
        private ICommand _seekDragStartedCommand;
        private ICommand _volumeDragStartedCommand;
        private ICommand _muteCommand;
        private ICommand _navigateToNowPlayingCommand;
        private ICommand _clearQueueCommand;
        private ICommand _openEqualizerCommand;

        #endregion

        #region Properties

        private bool _isDropDownOpen = false;
        public bool IsDropDownOpen
        {
            get => _isDropDownOpen;
            set
            {
                if (_isDropDownOpen == value) return;

                _isDropDownOpen = value;
                OnPropertyChanged(nameof(IsDropDownOpen));
            }
        }

        private bool _leftPanelVisible;
        public bool LeftPanelVisible
        {
            get => _leftPanelVisible;
            set
            {
                _leftPanelVisible = value;
                OnPropertyChanged(nameof(LeftPanelVisible));
            }
        }

        private bool _clearQueueButtonVisible = true;
        public bool ClearQueueButtonVisible
        {
            get => _clearQueueButtonVisible;
            set
            {
                _clearQueueButtonVisible = value;
                OnPropertyChanged(nameof(ClearQueueButtonVisible));
            }
        }

        private bool _isMuted;
        public bool IsMuted
        {
            get => _isMuted;
            set
            {
                if (value == _isMuted) return;

                _isMuted = value;

                MuteButtonToolTip = _isMuted ? "Unmute" : "Mute";
                OnPropertyChanged(nameof(IsMuted));
            }
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        private bool _isRepeatEnabled = false;
        public bool IsRepeatEnabled
        {
            get => _isRepeatEnabled;
            set
            {
                _isRepeatEnabled = value;
                RepeatButtonToolTip = _isRepeatEnabled ? "Repeat: On" : "Repeat: Off";
                OnPropertyChanged(nameof(IsRepeatEnabled));
            }
        }

        private bool _isShuffleEnabled = false;
        public bool IsShuffleEnabled
        {
            get => _isShuffleEnabled;
            set
            {
                _isShuffleEnabled = value;
                ShuffleButtonToolTip = _isShuffleEnabled ? "Shuffle: On" : "Shuffle: Off";
                OnPropertyChanged(nameof(IsShuffleEnabled));
            }
        }

        private float _volume;
        public float Volume
        {
            get => _volume;
            set
            {
                if (value == _volume) return;

                _volume = value;

                IsMuted = _volume == 0;

                _musicPlayer?.SetVolume(value);
                OnPropertyChanged(nameof(Volume));
            }
        }

        private TrackModel? _currentTrack;
        public TrackModel? CurrentTrack
        {
            get => _currentTrack;
            set
            {
                if (_currentTrack == value) return;

                _currentTrack = value;
                OnPropertyChanged(nameof(CurrentTrack));

                LeftPanelVisible = _currentTrack is not null;
            }
        }

        private double _trackCurrentPosition;
        public double TrackCurrentPosition
        {
            get => _trackCurrentPosition;
            set
            {
                _trackCurrentPosition = value;
                OnPropertyChanged(nameof(TrackCurrentPosition));
            }
        }

        private double _trackLength;
        public double TrackLength
        {
            get => _trackLength;
            set
            {
                _trackLength = value;
                OnPropertyChanged(nameof(TrackLength));
            }
        }

        private bool _areTracksInQueue;
        public bool AreTracksInQueue
        {
            get => _areTracksInQueue;
            set
            {
                _areTracksInQueue = value;
                OnPropertyChanged(nameof(AreTracksInQueue));
            }
        }

        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;

                PlayPauseButtonToolTip = _isPlaying ? "Pause" : "Play";

                PlayingStateChanged?.Invoke();
                OnPropertyChanged(nameof(IsPlaying));
            }
        }

        private string _muteButtonToolTip;
        public string MuteButtonToolTip
        {
            get => _muteButtonToolTip;
            set
            {
                _muteButtonToolTip = value;
                OnPropertyChanged(nameof(MuteButtonToolTip));
            }
        }

        private string _playPauseButtonToolTip = "Play";
        public string PlayPauseButtonToolTip
        {
            get => _playPauseButtonToolTip;
            set
            {
                _playPauseButtonToolTip = value;
                OnPropertyChanged(nameof(PlayPauseButtonToolTip));
            }
        }

        private string _shuffleButtonToolTip = "Shuffle: Off";
        public string ShuffleButtonToolTip
        {
            get => _shuffleButtonToolTip;
            set
            {
                _shuffleButtonToolTip = value;
                OnPropertyChanged(nameof(ShuffleButtonToolTip));
            }
        }

        private string _repeatButtonToolTip = "Repeat: Off";
        public string RepeatButtonToolTip
        {
            get => _repeatButtonToolTip;
            set
            {
                _repeatButtonToolTip = value;
                OnPropertyChanged(nameof(RepeatButtonToolTip));
            }
        }

        private bool _commandButtonsEnabled = true;
        public bool CommandButtonsEnabled
        {
            get => _commandButtonsEnabled;
            set
            {
                if (_commandButtonsEnabled == value) return;

                _commandButtonsEnabled = value;
                OnPropertyChanged(nameof(CommandButtonsEnabled));
            }
        }

        #endregion

        #region Commands 

        public ICommand PlayPauseCommand => _playPauseCommand ??= new RelayCommand((_) => PlayPause());
        public ICommand PlayNextCommand => _playNextCommand ??= new RelayCommand((_) => PlayNextTrackInQueue());
        public ICommand PlayPreviousCommand => _playPreviousCommand ??= new RelayCommand((_) => PlayPreviousTrackInQueue());
        public ICommand MuteCommand => _muteCommand ??= new RelayCommand((_) => Mute());
        public ICommand SeekDragStartedCommand => _seekDragStartedCommand ??= new RelayCommand((_) => _isSeeking = true);
        public ICommand SeekDragCompletedCommand => _seekDragCompletedCommand ??= new RelayCommand((_) => OnSeekDragCompleted());
        public ICommand VolumeDragStartedCommand => _volumeDragStartedCommand ??= new RelayCommand((_) => OnVolumeDragStarted());
        public ICommand NavigateToNowPlayingCommand => _navigateToNowPlayingCommand ??= new RelayCommand((_) => { NavigateToNowPlaying?.Invoke(); });
        public ICommand ClearQueueCommand => _clearQueueCommand ??= new RelayCommand((_) => ClearQueue());
        public ICommand OpenEqualizerCommand => _openEqualizerCommand ??= new RelayCommand(_ => OpenEqualizer());

        #endregion

        public MusicPlayerViewModel(ModalNavigationStore modalNavigationStore, TrackStore trackStore)
        {
            _modalNavigationStore = modalNavigationStore;
            _trackStore = trackStore;
            _trackStore.CurrentTrackChanged += OnCurrentTrackChanged;
            _trackStore.QueueChanged += OnQueueChanged;
            _timer = new Timer(1000);
            _timer.Elapsed += OnTimerElapsed;

            _queue = _trackStore.Queue;

            CommandButtonsEnabled = _trackStore.CurrentTrack is not null;

            LeftPanelVisible = CurrentTrack is not null;

            Volume = Properties.Settings.Default.Volume;

            MuteButtonToolTip = Volume == 0 ? "Unmute" : "Mute";
        }

        #region Private Methods

        private void ClearQueue()
        {
            _trackStore.ClearQueue();

            ResetPlayer();

            TrackLength = 0;
        }

        private void OpenEqualizer()
        {
            IsDropDownOpen = false;

            _modalNavigationStore.CurrentViewModel = new EqualizerModalViewModel(_modalNavigationStore, _musicPlayer);
        }

        private void OnVolumeDragStarted()
        {
            if (Volume == 0 || IsMuted) return;

            _savedVolume = Volume;
        }

        private void OnQueueChanged()
        {
            // If there are no tracks in the queue, disable command buttons
            CommandButtonsEnabled = _queue.Count != 0 || _trackStore.CurrentTrack is not null;
        }

        private void OnCurrentTrackChanged()
        {
            // Set current track
            CurrentTrack = _trackStore.CurrentTrack;

            if (_trackStore.CurrentTrack is null) return;

            TrackLength = CurrentTrack!.Length;

            // Activate command buttons
            CommandButtonsEnabled = true;

            // Reset player object
            ResetPlayer();

            try // TODO Fix; Case: only 1 Track in que => infinite loop
            {
                if (_musicPlayer is not null)
                {
                    _musicPlayer.PlaybackStopped -= OnPlaybackStopped;
                    _musicPlayer?.Dispose();
                }

                _musicPlayer = new MusicPlayer(CurrentTrack.FilePath, Volume);
                _musicPlayer.PlaybackStopped += OnPlaybackStopped;
            }
            catch (Exception ex)
            {
                _currentTrack!.HasErrors = true;
                _currentTrack!.ErrorMessage = ex.Message;

                PlayNextTrackInQueue();
            }

            PlayPause();
        }

        private void OnSeekDragCompleted()
        {
            _musicPlayer?.SetPosition(_trackCurrentPosition);
            _isSeeking = false;
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if (!_isSeeking && _musicPlayer is not null) TrackCurrentPosition = _musicPlayer.GetPositionInSeconds();
        }

        private void OnPlaybackStopped() //BUG: One track in queue: after first finish PlaybackStopped is called twice
        {
            ResetPlayer();

            PlayNextTrackInQueue();
        }

        private void PlayNextTrackInQueue()
        {
            if (_queue.Count == 0) return;

            try
            {
                var index = _queue.IndexOf(_currentTrack!);

                if (index < _queue.Count - 1)
                {
                    index++;
                    _trackStore.CurrentTrack = _queue[index];
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void PlayPreviousTrackInQueue()
        {
            if (_queue.Count == 0) return;

            try
            {
                var index = _queue.IndexOf(_currentTrack!);

                if (index > 0)
                {
                    index--;
                    _trackStore.CurrentTrack = _queue[index];
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void ResetPlayer()
        {
            IsPlaying = false;
            TrackCurrentPosition = 0;

            _musicPlayer?.SetPosition(TrackCurrentPosition);

            _timer?.Stop();
        }

        private void PlayPause()
        {
            if (_currentTrack is null) return;

            _musicPlayer?.TogglePlayPause();

            if (_timer.Enabled)
            {
                _timer.Stop();
                IsPlaying = false;
                PlayPauseButtonToolTip = "Play";
            }
            else
            {
                _timer.Start();
                IsPlaying = true;
                PlayPauseButtonToolTip = "Pause";
            }
        }

        private void Mute()
        {
            if (!IsMuted)
            {
                Volume = _savedVolume;
            }
            else
            {
                _savedVolume = Volume;
                Volume = 0;
            }
        }

        #endregion

        public override void Dispose()
        {
            if (_musicPlayer is not null)
                _musicPlayer.PlaybackStopped -= OnPlaybackStopped;

            _musicPlayer?.Dispose();

            if (_timer.Enabled)
                _timer.Stop();

            _timer.Elapsed -= OnTimerElapsed;
            _timer.Dispose();

            _trackStore.CurrentTrackChanged -= OnCurrentTrackChanged;
            _trackStore.QueueChanged -= OnQueueChanged;

            base.Dispose();
        }
    }
}
