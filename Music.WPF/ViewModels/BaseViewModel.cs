using Music.WPF.Core;
using Music.WPF.Extensions;
using Music.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Music.WPF.ViewModels
{
    public abstract class BaseViewModel : ObservableObject
    {
        protected static IList<TrackModel> Sort(IEnumerable<TrackModel> tracks, string sortOptionString)
        {
            SortOption sortOption = EnumExtension.GetValueFromDescription<SortOption>(sortOptionString);

            IEnumerable<TrackModel> orderedTracks = new List<TrackModel>();

            switch (sortOption)
            {
                case SortOption.Date:
                    orderedTracks = tracks;
                    break;
                case SortOption.Title:
                    orderedTracks = tracks.OrderBy(x => x.Title);
                    break;
                case SortOption.Artist:
                    orderedTracks = tracks.OrderBy(x => x.Artist);
                    break;
                case SortOption.Length:
                    orderedTracks = tracks.OrderBy(x => x.Length);
                    break;
            }

            return orderedTracks.ToList();
        }

        protected static string GetStringTotalTimeFromTracks(IList<TrackModel> tracks)
        {
            double totalTime = 0;

            for (int i = 0; i < tracks.Count; i++)
            {
                totalTime += tracks[i].Length;
            }

            var timeSpan = TimeSpan.FromSeconds(totalTime);

            return (timeSpan.Hours > 1) ? $"{timeSpan.Hours}h {timeSpan.Minutes}min" : $"{timeSpan.Minutes}min";
        }

        /// <summary>
        /// Reserved for clean up operations (e.g. disposing of unmanaged resources, unsubscribing from events).
        /// </summary>
        public virtual void Dispose() { }
    }
}