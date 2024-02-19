using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Music.WPF.Extensions
{
    public sealed class SliderExtension
    {
        public static readonly DependencyProperty DragStartedCommandProperty =
            DependencyProperty.RegisterAttached("DragStartedCommand", typeof(ICommand), typeof(SliderExtension),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(DragStarted)));

        public static readonly DependencyProperty DragCompletedCommandProperty =
            DependencyProperty.RegisterAttached("DragCompletedCommand", typeof(ICommand), typeof(SliderExtension),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(DragCompleted)));

        private static void DragStarted(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (Slider)d;
            var thumb = GetThumbFromSlider(slider);

            if (thumb != null)
            thumb.DragStarted += Thumb_DragStarted;
        }

        private static void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            element.Dispatcher.Invoke(() =>
            {
                var command = GetDragStartedCommand(element);
                var slider = FindParentControl<Slider>(element) as Slider;
                command.Execute(slider!.Value);
            });
        }

        public static void SetDragStartedCommand(UIElement element, ICommand value)
        {
            element.SetValue(DragStartedCommandProperty, value);
        }

        public static ICommand GetDragStartedCommand(FrameworkElement element)
        {
            var slider = FindParentControl<Slider>(element);
            return (ICommand)slider!.GetValue(DragStartedCommandProperty);
        }

        private static void DragCompleted(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (Slider)d;
            var thumb = GetThumbFromSlider(slider);

            if (thumb != null)
            thumb.DragCompleted += Thumb_DragCompleted;
        }

        private static void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                element.Dispatcher.Invoke(() =>
                {
                    var command = GetDragCompletedCommand(element);
                    var slider = FindParentControl<Slider>(element) as Slider;
                    command.Execute(slider!.Value);
                });
            }
        }

        public static void SetDragCompletedCommand(UIElement element, ICommand value)
        {
            element.SetValue(DragCompletedCommandProperty, value);
        }

        public static ICommand GetDragCompletedCommand(FrameworkElement element)
        {
            var slider = FindParentControl<Slider>(element);
            return (ICommand)slider!.GetValue(DragCompletedCommandProperty);
        }

        private static Thumb? GetThumbFromSlider(Slider slider)
        {
            var track = slider.Template.FindName("PART_Track", slider) as Track;
            return track is null ? null : track.Thumb;
        }

        private static DependencyObject? FindParentControl<T>(DependencyObject control)
        {
            var parent = VisualTreeHelper.GetParent(control);
            while (parent is not null && parent is not T)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent;
        }
    }
}
