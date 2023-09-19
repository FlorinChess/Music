using Music.WPF.Core;

namespace Music.WPF.Models
{
    public sealed class TrackModel : ObservableObject
    {
        /// <summary>
        /// Indicates if the track is currently being played.
        /// </summary>
        private bool _isCurrentlyPlaying = false;
        public bool IsCurrentlyPlaying 
        { 
            get => _isCurrentlyPlaying; 
            set
            {
                _isCurrentlyPlaying = value;
                OnPropertyChanged(nameof(IsCurrentlyPlaying));
            }
        }
        /// <summary>
        /// The full path of the music file.
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// The title tag of the music file.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The main artist of the music file.
        /// </summary>
        public string Artist { get; set; }
        /// <summary>
        /// The total duration of a music file in seconds.
        /// </summary>
        public double Length { get; set; }
        /// <summary>
        /// Indicates if the track is marked as favorite.
        /// </summary>
        public bool IsFavorite { get; set; }
        /// <summary>
        /// Indicates if the track has errors.
        /// </summary>
        public bool HasErrors { get; set; } = false;
        /// <summary>
        /// The error message.
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// The number of repeats for the current track.
        /// </summary>
        public int RepeatCount { get; set; }
    }
}
