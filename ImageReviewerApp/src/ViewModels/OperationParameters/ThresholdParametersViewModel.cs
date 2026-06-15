using ImageReviewerApp.Models.OperationParameters;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageReviewerApp.ViewModels.OperationParameters;

public class ThresholdParametersViewModel : IOperationParametersViewModel
{
    private int _threshold = 32768;

    public int Threshold
    {
        get => _threshold;
        set
        {
            if (_threshold != value)
            {
                _threshold = value;
                OnPropertyChanged();
            }
        }
    }

    public object GetParameters()
    {
        return new ThresholdParameters(Threshold: (ushort)_threshold);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
