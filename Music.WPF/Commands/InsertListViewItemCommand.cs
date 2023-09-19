using Music.WPF.ViewModels;

namespace Music.WPF.Commands
{
    public sealed class InsertListViewItemCommand : BaseCommand
    {
        private readonly TrackListComponentViewModel _trackListComponentViewModel;

        public InsertListViewItemCommand(TrackListComponentViewModel trackListComponentViewModel)
        {
            _trackListComponentViewModel = trackListComponentViewModel;
        }

        public override void Execute(object? parameter)
        {
            _trackListComponentViewModel.InsertItem(_trackListComponentViewModel.InsertedItem, _trackListComponentViewModel.TargetItem);
        }
    }
}
