using Music.WPF.Commands;
using System.Windows.Input;

namespace Music.WPF.Modals.ViewModels
{
    public interface IModal
    {
        public CloseModalCommand CloseModalCommand { get; set; }
        public ICommand SaveCommand { get; set; }
    }
}
