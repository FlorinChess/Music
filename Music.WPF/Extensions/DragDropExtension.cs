using Music.CustomControls.AnimatedListView;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Music.WPF.Extensions
{
    public static class DragDropExtension
    {
        public static readonly DependencyProperty ScrollOnDragDropProperty =
            DependencyProperty.RegisterAttached("ScrollOnDragDrop",
                typeof(bool),
                typeof(DragDropExtension),
                new PropertyMetadata(false, HandleScrollOnDragDropChanged));

        public static bool GetScrollOnDragDrop(DependencyObject element)
        {
            if (element is null) throw new ArgumentNullException();

            return (bool)element.GetValue(ScrollOnDragDropProperty);
        }

        public static void SetScrollOnDragDrop(DependencyObject element, bool value)
        {
            if (element is null) throw new ArgumentNullException();

            element.SetValue(ScrollOnDragDropProperty, value);
        }

        private static void HandleScrollOnDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement container)
            {
                Unsubscribe(container);

                if (true.Equals(e.NewValue))
                {
                    Subscribe(container);
                }
            }
        }

        private static void Subscribe(FrameworkElement container)
        {
            container.PreviewDragOver += OnContainerPreviewDragOver;
        }

        private static void OnContainerPreviewDragOver(object sender, DragEventArgs e)
        {
            if (sender is FrameworkElement container && GetFirstVisualChild<ScrollViewer>(container) is AnimatedScrollViewer animatedScrollViewer)
            {
                double threshold = 60;
                double verticalPosition = e.GetPosition(container).Y;
                double verticalOffset = 50;

                if (verticalPosition < threshold) // Top of visible list? 
                {
                    //animatedScrollViewer.TargetVerticalOffset = animatedScrollViewer.VerticalOffset - verticalOffset;
                    animatedScrollViewer.ScrollToVerticalOffset(animatedScrollViewer.VerticalOffset - verticalOffset); //Scroll up. 
                }
                else if (verticalPosition > container.ActualHeight - threshold) //Bottom of visible list? 
                {
                    //animatedScrollViewer.TargetVerticalOffset = animatedScrollViewer.VerticalOffset + verticalOffset;
                    animatedScrollViewer.ScrollToVerticalOffset(animatedScrollViewer.VerticalOffset + verticalOffset); //Scroll down.     
                }
            }
        }

        private static void Unsubscribe(FrameworkElement container)
        {
            container.PreviewDragOver -= OnContainerPreviewDragOver;
        }

        public static T? GetFirstVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj is not null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child is not null)
                    {
                        if (child is T t)
                            return t;
                    
                        T? childItem = GetFirstVisualChild<T>(child);
                        if (childItem is not null)
                        {
                            return childItem;
                        }
                    }

                }
            }

            return null;
        }
    }
}
