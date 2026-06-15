using ImageReviewerApp.Models.OperationParameters;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageReviewerApp.ViewModels.OperationParameters;

public class WindowLevelParametersViewModel : IOperationParametersViewModel
{
    private double _window = 65536;
    private double _level = 32768;

    public double Window
    {
        get => _window;
        set
        {
            if (_window != value)
            {
                _window = value;
                OnPropertyChanged();
            }
        }
    }

    public double Level
    {
        get => _level;
        set
        {
            if (_level != value)
            {
                _level = value;
                OnPropertyChanged();
            }
        }
    }

    public object GetParameters()
    {
        return new WindowLevelParameters(Window: _window, Level: _level);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
