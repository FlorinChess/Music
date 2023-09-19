using System.Windows;
using System.Windows.Controls;

namespace Music.CustomControls.EqualizerSlider
{
    public sealed class EqualizerSlider : Slider
    {
        static EqualizerSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EqualizerSlider), new FrameworkPropertyMetadata(typeof(EqualizerSlider)));
        }
    }
}
