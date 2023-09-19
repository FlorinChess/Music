using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Music.WPF.Services
{
    public static class MusicFilesService
    {
        /// <summary>
        /// Returns the paths of the music files (.mp3 or .wav) in a directory.
        /// </summary>
        /// <param name="directory">The directory where the music files are stored on disk.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the music file paths.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<string> GetMusicFiles(string directory = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) 
                    throw new ArgumentException("Selected directory could not be found.");

                var directoryPath = string.IsNullOrEmpty(directory) ? Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) : directory;

                return Directory.GetFiles(directoryPath)
                    .Where(filePath => filePath.EndsWith(".mp3") || filePath.EndsWith(".wav"));
            }
            catch (Exception)
            {
                throw;
            }            
        }

        /// <summary>
        /// Returns the paths of the music files (.mp3 or .wav) in a directory.
        /// </summary>
        /// <param name="directories">The directories where the music files are stored on disk.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the music file paths.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<string> GetMusicFiles(IEnumerable<string> directories)
        {
            foreach (var directory in directories)
            {
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) 
                    throw new ArgumentException("Selected directory could not be found.");

                var directoryPath = string.IsNullOrEmpty(directory) ? Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) : directory;

                var filePaths = Directory.GetFiles(directory)
                    .Where(filePath => filePath.EndsWith(".mp3") || filePath.EndsWith(".wav"));

                foreach (var filePath in filePaths) 
                {
                    yield return filePath;
                }
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="OpenFileDialog"/> for selecting music files (.mp3 and .wav).
        /// </summary>
        /// <returns>An array of string containing the full paths of the selected files.</returns>
        public static string[] SelectMusicFiles()
        {
            try
            {
                using OpenFileDialog openFileDialog = new()
                {
                    Title = "Select music files...",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                    Multiselect = true,
                    Filter = "MP3 files (*.mp3)|*.mp3|WAV files (*.wav)|*.wav",
                    CheckPathExists = true
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileNames;
                }

                return Array.Empty<string>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="FolderBrowserDialog"/> for selecting a folder containing music files.
        /// </summary>
        /// <returns>The path of the selected folder.</returns>
        public static string AddNewMusicFolder()
        {
            using FolderBrowserDialog folderBrowserDialog = new();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                return folderBrowserDialog.SelectedPath;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the music folders stored in the settings file.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of folder paths retrieved from the settings file.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IEnumerable<string> GetMusicFolders()
        {
            try
            {
                var musicFolders = Properties.Settings.Default.MusicFolder.Cast<string>().ToArray();

                return musicFolders.Any() ? musicFolders : Enumerable.Empty<string>();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Could not access the settings file");
            }
        }
    }
}
