using System.Collections.Generic;

namespace Music.WPF.Core
{
    public interface ISortable
    {
        public string SelectedSortOption { get; set; }
        public List<string> SortOptions { get; }
    }
}
