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
        public static IEnumerable<TrackModel> CreateTracks(IEnumerable<string> files)
        {
            if (files is null || !files.Any()) return Enumerable.Empty<TrackModel>();

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
                        Artist = musicFile.Tag.Performers.MakeString(),
                        Length = MusicPlayer.GetLengthInSeconds(file)
                    };
                })
                .ToList();

            return output;
        }

        public static string MakeString(this string[] stringArray)
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
