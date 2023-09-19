using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Music.WPF.Components
{
    public sealed partial class TrackListComponent : UserControl
    {
        #region Dependency Properties

        public bool IsReorderEnabled
        {
            get { return (bool)GetValue(IsReorderEnabledProperty); }
            set { SetValue(IsReorderEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsReorderEnabledProperty =
            DependencyProperty.Register("IsReorderEnabled", typeof(bool), typeof(TrackListComponent), new PropertyMetadata(false));

        public object IncomingItem
        {
            get { return GetValue(IncomingItemProperty); }
            set { SetValue(IncomingItemProperty, value); }
        }

        public static readonly DependencyProperty IncomingItemProperty =
            DependencyProperty.Register("IncomingItem", typeof(object), typeof(TrackListComponent),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object RemovedItem
        {
            get { return GetValue(RemovedItemProperty); }
            set { SetValue(RemovedItemProperty, value); }
        }

        public static readonly DependencyProperty RemovedItemProperty =
            DependencyProperty.Register("RemovedItem", typeof(object), typeof(TrackListComponent),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object TargetItem
        {
            get { return GetValue(TargetItemProperty); }
            set { SetValue(TargetItemProperty, value); }
        }

        public static readonly DependencyProperty TargetItemProperty =
            DependencyProperty.Register("TargetItem", typeof(object), typeof(TrackListComponent),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object InsertedItem
        {
            get { return GetValue(InsertedItemProperty); }
            set { SetValue(InsertedItemProperty, value); }
        }

        public static readonly DependencyProperty InsertedItemProperty =
            DependencyProperty.Register("InsertedItem", typeof(object), typeof(TrackListComponent),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ICommand DragOverCommand
        {
            get { return (ICommand)GetValue(DragOverCommandProperty); }
            set { SetValue(DragOverCommandProperty, value); }
        }

        public static readonly DependencyProperty DragOverCommandProperty =
            DependencyProperty.Register("DragOverCommand", typeof(ICommand), typeof(TrackListComponent), new PropertyMetadata(null));

        public ICommand RemoveItemCommand
        {
            get { return (ICommand)GetValue(RemoveItemCommandProperty); }
            set { SetValue(RemoveItemCommandProperty, value); }
        }

        public static readonly DependencyProperty RemoveItemCommandProperty =
            DependencyProperty.Register("RemoveItemCommand", typeof(ICommand), typeof(TrackListComponent), new PropertyMetadata(null));

        public ICommand InsertItemCommand
        {
            get { return (ICommand)GetValue(InsertItemCommandProperty); }
            set { SetValue(InsertItemCommandProperty, value); }
        }

        public static readonly DependencyProperty InsertItemCommandProperty =
            DependencyProperty.Register("InsertItemCommand", typeof(ICommand), typeof(TrackListComponent), new PropertyMetadata(null));

        #endregion Dependency Properties

        public TrackListComponent()
        {
            InitializeComponent();

            topScrollArea.Visibility = Visibility.Collapsed;
            bottomScrollArea.Visibility = Visibility.Collapsed;
        }

        private void ListViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsReorderEnabled) return;

            if (e.LeftButton == MouseButtonState.Pressed && sender is FrameworkElement frameworkElement)
            {
                var dataContext = frameworkElement.DataContext;

                DragDropEffects dragDropResult = DragDrop.DoDragDrop(frameworkElement,
                    new DataObject(DataFormats.Serializable, dataContext),
                    DragDropEffects.Move | DragDropEffects.Scroll);

                if (dragDropResult == DragDropEffects.None)
                {
                    AddItem(dataContext);
                }
            }
            else
            {
                topScrollArea.Visibility = Visibility.Collapsed;
                bottomScrollArea.Visibility = Visibility.Collapsed;
            }
        }

        private void AnimatedListView_DragOver(object sender, DragEventArgs e)
        {
            if (!IsReorderEnabled) return;

            object item = e.Data.GetData(DataFormats.Serializable);
            AddItem(item);

            topScrollArea.Visibility = Visibility.Visible;
            bottomScrollArea.Visibility = Visibility.Visible;
        }

        private void AddItem(object item)
        {
            if (!IsReorderEnabled) return;

            if (DragOverCommand?.CanExecute(null) ?? false)
            {
                IncomingItem = item;
                DragOverCommand.Execute(null);
            }
        }

        private void AnimatedListView_DragLeave(object sender, DragEventArgs e)
        {
            //HitTestResult result = VisualTreeHelper.HitTest(trackListView, e.GetPosition(trackListView));

            // When drag leaves the ListView remove the item from the list view
            //if (result == null)
            //{
            //    if (RemoveItemCommand?.CanExecute(null) ?? false)
            //    {
            //        RemovedItem = e.Data.GetData(DataFormats.Serializable);
            //        RemoveItemCommand.Execute(null);
            //    }
            //}
        }

        private void ListViewItem_DragOver(object sender, DragEventArgs e)
        {
            if (!IsReorderEnabled) return;

            if (InsertItemCommand?.CanExecute(null) ?? false)
            {
                if (sender is FrameworkElement element)
                {
                    TargetItem = element.DataContext;
                    InsertedItem = e.Data.GetData(DataFormats.Serializable);

                    InsertItemCommand.Execute(null);
                }
            }
        }
    }

}
