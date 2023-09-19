using System;
using System.Collections.Generic;

namespace Music.WPF.Models
{
    public sealed class PlaylistModel : ICloneable
    {
        /// <summary>
        /// The name of the playlist.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The date the playlist was created.
        /// </summary>
        public DateOnly DateCreated { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        /// <summary>
        /// The tracks of the playlist.
        /// </summary>
        public List<TrackModel> Tracks { get; set; } = new();

        /// <summary>
        /// The tags of the playlist.
        /// </summary>
        public List<string> Tags { get; set; } = new();

        /// <summary>
        /// The path to the cover image of the playlist.
        /// </summary>
        public string ImagePath { get; set; } = string.Empty;

        /// <summary>
        /// Returns <see cref="bool">true</see> if the given track is contained in the playlist.
        /// </summary>
        /// <param name="track"></param>
        /// <returns></returns>
        public bool Contains(TrackModel track) => Tracks.Contains(track);

        public object Clone()
        {
            var clone = new PlaylistModel()
            {
                Name = Name,
                DateCreated = DateCreated,
                Tracks = new List<TrackModel>(Tracks),
                Tags = new List<string>(Tags),
                ImagePath = ImagePath,
            };

            return clone;
        }
    }
}
