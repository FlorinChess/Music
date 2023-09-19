using System;
using System.Windows;
using System.Windows.Controls;

namespace Music.CustomControls.AnimatedListView
{
    [TemplatePart(Name = "PART_AnimatedScrollViewer", Type = typeof(AnimatedScrollViewer))]
    public sealed class AnimatedListView : ListView
    {
        private AnimatedScrollViewer _animatedScrollViewer;

        public static readonly DependencyProperty ScrollRegionsEnabledProperty =
            DependencyProperty.Register("ScrollRegionsEnabled", typeof(bool), typeof(AnimatedListView), new PropertyMetadata(false));

        public bool ScrollToSelectedItem
        {
            get => (bool)GetValue(ScrollToSelectedItemProperty);
            set => SetValue(ScrollToSelectedItemProperty, value);
        }
        public static readonly DependencyProperty ScrollToSelectedItemProperty =
            DependencyProperty.Register("ScrollToSelectedItem", typeof(bool), typeof(AnimatedListView), new PropertyMetadata(false));

        public int SelectedIndexOffset
        {
            get => (int)GetValue(SelectedIndexOffsetProperty);
            set => SetValue(SelectedIndexOffsetProperty, value);
        }
        public static readonly DependencyProperty SelectedIndexOffsetProperty =
            DependencyProperty.Register("SelectedIndexOffset", typeof(int), typeof(AnimatedListView), new PropertyMetadata(0));


        static AnimatedListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedListView), new FrameworkPropertyMetadata(typeof(AnimatedListView)));
        }

        private void AnimatedListBox_LayoutUpdated(object? sender, EventArgs e)
        {
            UpdateScrollPosition(sender);
        }

        private void AnimatedListBox_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateScrollPosition(sender);
        }

        private void AnimatedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateScrollPosition(sender);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_AnimatedScrollViewer") is AnimatedScrollViewer animatedScrollViewer)
            {
                _animatedScrollViewer = animatedScrollViewer;
            }

            SelectionChanged += AnimatedListBox_SelectionChanged;
            Loaded += AnimatedListBox_Loaded;
            LayoutUpdated += AnimatedListBox_LayoutUpdated;
        }

        public void UpdateScrollPosition(object? sender)
        {
            if (sender is AnimatedListView animatedListView && animatedListView.ScrollToSelectedItem)
            {
                double scrollDistance = 0;
                for (int i = 0; i < animatedListView.SelectedIndex + animatedListView.SelectedIndexOffset; i++)
                {
                    if (animatedListView.ItemContainerGenerator.ContainerFromItem(animatedListView.Items[i]) is ListBoxItem listBoxItem)
                    {
                        scrollDistance += listBoxItem.ActualHeight;
                    }
                }

                _animatedScrollViewer.TargetVerticalOffset = scrollDistance;
            }
        }
    }
}
