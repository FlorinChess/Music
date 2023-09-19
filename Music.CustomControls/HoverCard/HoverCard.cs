using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Music.CustomControls.HoverCard
{
    public sealed class HoverCard : ContentControl
    {
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(HoverCard), new PropertyMetadata(null));

        public double ShadowOpacity
        {
            get { return (double)GetValue(ShadowOpacityProperty); }
            set { SetValue(ShadowOpacityProperty, value); }
        }

        public static readonly DependencyProperty ShadowOpacityProperty =
            DependencyProperty.Register("ShadowOpacity", typeof(double), typeof(HoverCard), new PropertyMetadata(1.0d));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(HoverCard), new PropertyMetadata(new CornerRadius(0)));

        static HoverCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HoverCard), new FrameworkPropertyMetadata(typeof(HoverCard)));
        }
    }
}
