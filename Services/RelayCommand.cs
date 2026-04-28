using System;
using System.Windows.Input;

namespace ScCestinator.Services;

public class RelayCommand : ICommand
{
    private readonly Func<bool>? _canExecute;
    private readonly Action _execute;
    private readonly Action<object?>? _executeWithParameter;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public RelayCommand(Action<object?> execute, Func<bool>? canExecute = null)
    {
        _execute = () => execute(null);
        _executeWithParameter = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute();
    }

    public void Execute(object? parameter)
    {
        if (_executeWithParameter != null)
        {
            _executeWithParameter(parameter);
            return;
        }

        _execute();
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
