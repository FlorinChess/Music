using Music.WPF.Commands;
using Music.WPF.Store;
using Music.WPF.ViewModels;
using System;
using System.Windows.Input;

namespace Music.WPF.Modals.ViewModels
{
    public sealed class ConfirmationModalViewModel : BaseViewModel, IModal
    {
        public string ConfirmationText { get; }

        #region Commands

        public CloseModalCommand CloseModalCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        #endregion Commands

        public ConfirmationModalViewModel(ModalNavigationStore modalNavigationStore, string confirmationText, Action callback)
        {
            ConfirmationText = confirmationText;

            CloseModalCommand = modalNavigationStore.CloseModalCommand;
            SaveCommand = new RelayCommand(_ =>
            {
                callback?.Invoke();

                modalNavigationStore.Close();
            });
        }
    }
}
