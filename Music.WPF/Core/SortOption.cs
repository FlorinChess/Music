using System.ComponentModel;

namespace Music.WPF.Core
{
    public enum SortOption
    {
        [Description("Date added")]
        Date,

        [Description("Title")]
        Title,

        [Description("Artist")]
        Artist,

        [Description("Length")]
        Length
    }
}
