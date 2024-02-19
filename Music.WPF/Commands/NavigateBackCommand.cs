using Music.WPF.Store;

namespace Music.WPF.Commands
{
    public sealed class NavigateBackCommand : BaseCommand
    {
        private readonly NavigationStore _navigationStore;

        public NavigateBackCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }

        public override void Execute(object? parameter)
        {
            _navigationStore.Pop();
        }
    }
}
