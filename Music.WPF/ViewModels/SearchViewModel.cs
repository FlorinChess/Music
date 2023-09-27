using Music.WPF.Commands;
using Music.WPF.Core;
using Music.WPF.Extensions;
using Music.WPF.Models;
using Music.WPF.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

namespace Music.WPF.ViewModels
{
    public sealed class SearchViewModel : BaseViewModel, ISortable, INavigation
    {
        private readonly TrackStore _trackStore;

        #region Properties

        public List<string> SortOptions => EnumExtension.GetSortOptions();
        public string SearchText { get; set; }

        private string _selectedSortOption;
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (_selectedSortOption == value) return;

                _selectedSortOption = value;
                OnPropertyChanged(nameof(SelectedSortOption));

                var ordererdTracks = Sort(_listComponentViewModel.Tracks, _selectedSortOption);

                _listComponentViewModel.ClearTracks();

                foreach (var track in ordererdTracks)
                {
                    _listComponentViewModel.AddTrack(track);
                }
            }
        }

        private string _searchResult = "";
        public string SearchResult
        {
            get => _searchResult;
            set
            {
                if (_searchResult == value) return;

                _searchResult = value;
                OnPropertyChanged(nameof(SearchResult));
            }
        }

        private ListComponentViewModel _listComponentViewModel;
        public ListComponentViewModel ListComponentViewModel
        {
            get => _listComponentViewModel;
            set
            {
                _listComponentViewModel = value;
                OnPropertyChanged(nameof(ListComponentViewModel));
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading == value) return;

                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        #endregion

        #region Commands

        public ICommand SearchCommand { get; private set; }

        #endregion

        public SearchViewModel(TrackStore trackStore, ListComponentViewModel listComponentViewModel)
        {
            _trackStore = trackStore;
            ListComponentViewModel = listComponentViewModel;
            ListComponentViewModel.CanRemoveTrack = false;

            SearchCommand = new RelayCommand(_ => Search());

            SelectedSortOption = SortOptions[0];
        }

        #region Private Methods

        private void Search()
        {
            if (string.IsNullOrEmpty(SearchText)) return;

            IsLoading = true;
            _listComponentViewModel.ClearTracks();

            try
            {
                List<TrackModel> matchedTracks = SearchTracks(SearchText);

                for (int i = 0; i < matchedTracks.Count; i++)
                    _listComponentViewModel.AddTrack(matchedTracks[i]);

                SearchResult = (_listComponentViewModel.Count == 0) ? "No matches found." : string.Empty;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private List<TrackModel> SearchTracks(string searchText)
        {
            List<TrackModel> availableTracks = _trackStore.AvailableTracks;

            var matchedTracks = new List<TrackModel>();

            foreach (var track in availableTracks)
            {
                if (!track.Title.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) && 
                    !track.Artist.Contains(searchText, StringComparison.CurrentCultureIgnoreCase)) continue;

                matchedTracks.Add(track);
            }

            return matchedTracks;
        }

        #endregion

        public override PageIndex GetPageIndex() => PageIndex.Search;
    }
}
