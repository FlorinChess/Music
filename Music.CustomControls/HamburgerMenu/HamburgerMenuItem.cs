using System.Windows;
using System.Windows.Controls;

namespace Music.CustomControls.HamburgerMenu
{
    public sealed class HamburgerMenuItem : RadioButton
    {
        static HamburgerMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HamburgerMenuItem), new FrameworkPropertyMetadata(typeof(HamburgerMenuItem)));
        }
    }
}
