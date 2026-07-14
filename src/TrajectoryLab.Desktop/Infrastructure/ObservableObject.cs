using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TrajectoryLab.Desktop.Infrastructure;

public abstract class ObservableObject :
    INotifyPropertyChanged
{
    public event PropertyChangedEventHandler?
        PropertyChanged;

    protected bool SetProperty<TValue>(
        ref TValue Field,
        TValue Value,
        [CallerMemberName] string? PropertyName = null)
    {
        if (EqualityComparer<TValue>.Default.Equals(
                Field,
                Value))
        {
            return false;
        }

        Field = Value;

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(PropertyName));

        return true;
    }
}