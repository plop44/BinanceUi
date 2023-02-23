using System;
using System.Windows.Input;

namespace BinanceUi.Various;

public class DelegateCommand : DelegateCommand<object>
{
    public DelegateCommand(Action execute, Func<bool>? canExecute = null) : base(_ => execute(), canExecute != null ? (_ => canExecute()) : null)
    {
    }
}

public class DelegateCommand<T> : ICommand where T : class
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool> _canExecute;

    public event EventHandler? CanExecuteChanged;

    public DelegateCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute ?? (_ => true);
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute.Invoke(parameter as T);
    }

    public void Execute(object? parameter)
    {
        _execute(parameter as T);
    }
}