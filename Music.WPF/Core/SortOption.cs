using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Music.WPF.Core
{
    public enum SortOption
    {
        [Description("Date added")]
        //[Display(Name = "Date added")]
        Date,

        [Description("Title")]
        //[Display(Name = "Title")]
        Title,

        [Description("Artist")]
        //[Display(Name = "Artist")]
        Artist,

        [Description("Length")]
        //[Display(Name = "Length")]
        Length
    }
}
