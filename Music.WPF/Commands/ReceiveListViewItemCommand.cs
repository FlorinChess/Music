using Music.WPF.ViewModels;

namespace Music.WPF.Commands
{
    public sealed class ReceiveListViewItemCommand : BaseCommand
    {
        private readonly TrackListComponentViewModel _trackListComponentViewModel;

        public ReceiveListViewItemCommand(TrackListComponentViewModel trackListComponentViewModel)
        {
            _trackListComponentViewModel = trackListComponentViewModel;
        }

        public override void Execute(object? parameter)
        {
            _trackListComponentViewModel.AddNewItem(_trackListComponentViewModel.IncomingItem);
        }
    }
}
