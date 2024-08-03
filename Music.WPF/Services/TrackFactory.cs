using Music.NAudio;
using Music.WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using TFile = TagLib.File;

namespace Music.WPF.Services
{
    public static class TrackFactory
    {
        private const string UNKNOWN_ARTIST = "Unknown Artist";

        /// <summary>
        /// Creates a TrackModel based on the tags of a file 
        /// </summary>
        /// <param name="filePath"> The path to the file</param>
        /// <returns>A new instance of <see cref="TrackModel"/> with data from the tags of the file</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TrackModel CreateTrack(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException();

            using TFile musicFile = TFile.Create(filePath);

            string artist = musicFile.Tag.Performers.ConcatAndAddDevider();

            return new TrackModel
            {
                FilePath = filePath,
                Title = (string.IsNullOrEmpty(musicFile.Tag.Title)) ? filePath : musicFile.Tag.Title,
                Artist = (string.IsNullOrEmpty(artist)) ? UNKNOWN_ARTIST : artist,
                Length = MusicPlayer.GetLengthInSeconds(filePath)
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
                .Select(filePath =>
                {
                    using TFile musicFile = TFile.Create(filePath);

                    string artist = musicFile.Tag.Performers.ConcatAndAddDevider();

                    return new TrackModel()
                    {
                        FilePath = filePath,
                        Title =  (string.IsNullOrEmpty(musicFile.Tag.Title)) ? GetFileNameWithoutExtension(filePath) : musicFile.Tag.Title,
                        Artist = (string.IsNullOrEmpty(artist)) ? UNKNOWN_ARTIST : artist,
                        Length = MusicPlayer.GetLengthInSeconds(filePath)
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

        /// <summary>
        /// Returns the file name without extension from a fully qulified file path.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>The resulting string.</returns>
        private static string GetFileNameWithoutExtension(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            return Path.GetFileNameWithoutExtension(fileName);
        }
    }
}
