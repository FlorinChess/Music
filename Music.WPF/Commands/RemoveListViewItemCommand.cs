using Music.WPF.ViewModels;

namespace Music.WPF.Commands
{
    public sealed class RemoveListViewItemCommand : BaseCommand
    {
        private readonly TrackListComponentViewModel _trackListComponentViewModel;
        public RemoveListViewItemCommand(TrackListComponentViewModel trackListComponentViewModel)
        {
            _trackListComponentViewModel = trackListComponentViewModel;
        }

        public override void Execute(object? parameter)
        {
            _trackListComponentViewModel.RemoveItem(_trackListComponentViewModel.RemovedItem);
        }
    }
}
