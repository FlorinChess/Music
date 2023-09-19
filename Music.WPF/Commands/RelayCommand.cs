using System;

namespace Music.WPF.Commands
{
    public sealed class RelayCommand : BaseCommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;
        
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object? parameter)
        {
            return _canExecute is null || _canExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
