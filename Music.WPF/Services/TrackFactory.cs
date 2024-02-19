using Music.NAudio;
using Music.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TagLib;

namespace Music.WPF.Services
{
    public static class TrackFactory
    {
        /// <summary>
        /// Creates a TrackModel based on the tags of a file 
        /// </summary>
        /// <param name="file"> The path to the file</param>
        /// <returns>A new instance of <see cref="TrackModel"/> with data from the tags of the file</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TrackModel CreateTrack(string file)
        {
            if (string.IsNullOrEmpty(file)) throw new ArgumentNullException();

            using File musicFile = File.Create(file);

            return new TrackModel()
            {
                FilePath = file,
                Title = musicFile.Tag.Title,
                Artist = musicFile.Tag.FirstPerformer.Replace("/", ", "),
                Length = MusicPlayer.GetLengthInSeconds(file),
            };
        }

        /// <summary>
        /// Creates a TrackModel based on the tags of each file path provided
        /// </summary>
        /// <param name="files">The file paths</param>
        /// <returns>A collection of <see cref="TrackModel"/> with data from the tags of the files</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IList<TrackModel> CreateTracks(IEnumerable<string> files)
        {
            if (files is null || !files.Any()) return new List<TrackModel>();

            var output =
                files.AsParallel()
                .WithDegreeOfParallelism(2)
                .Select(file =>
                {
                    using File musicFile = File.Create(file);
                    return new TrackModel()
                    {
                        FilePath = file,
                        Title = musicFile.Tag.Title,
                        Artist = musicFile.Tag.Performers.ConcatAndAddDevider(),
                        Length = MusicPlayer.GetLengthInSeconds(file)
                    };
                })
                .ToList();

            return output;
        }

        /// <summary>
        /// Concatenates the elements of a <see cref="string"/>[] into one <see cref="string"/> with ', ' as a devider between the individual elements.
        /// </summary>
        /// <param name="stringArray"></param>
        /// <returns>The resulting string.</returns>
        private static string ConcatAndAddDevider(this string[] stringArray)
        {
            var result = string.Empty;

            for (int i = 0; i < stringArray.Length; i++)
            {
                result += stringArray[i];

                if (i < stringArray.Length - 1)
                    result += ", ";
            }

            return result;
        }
    }
}
