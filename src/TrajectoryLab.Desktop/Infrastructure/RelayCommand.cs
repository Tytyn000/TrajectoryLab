using System.Windows.Input;

namespace TrajectoryLab.Desktop.Infrastructure;

public sealed class RelayCommand : ICommand
{
    private readonly Action ExecuteAction;
    private readonly Func<bool>? CanExecutePredicate;

    public RelayCommand(
        Action ExecuteAction,
        Func<bool>? CanExecutePredicate = null)
    {
        ArgumentNullException.ThrowIfNull(ExecuteAction);

        this.ExecuteAction = ExecuteAction;
        this.CanExecutePredicate = CanExecutePredicate;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? Parameter)
    {
        return CanExecutePredicate?.Invoke() ?? true;
    }

    public void Execute(object? Parameter)
    {
        ExecuteAction();
    }
}