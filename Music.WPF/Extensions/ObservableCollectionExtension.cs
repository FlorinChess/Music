using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Music.WPF.Extensions
{
    public static class ObservableCollectionExtension
    {
        public static ObservableCollection<T> AddRange<T>(this ObservableCollection<T> collection, IList<T> elements) where T : class
        {
            for (int i = 0; i < elements.Count; i++)
            {
                collection.Add(elements[i]);
            }

            return collection;
        }
    }
}
