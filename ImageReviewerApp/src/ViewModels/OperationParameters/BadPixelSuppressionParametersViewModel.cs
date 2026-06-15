using ImageReviewerApp.Models.OperationParameters;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageReviewerApp.ViewModels.OperationParameters;

public class BadPixelSuppressionParametersViewModel : IOperationParametersViewModel
{
    private double _threshold = 3.0;
    private int _kernelSize = 3;

    public double Threshold
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

    public int KernelSize
    {
        get => _kernelSize;
        set
        {
            if (_kernelSize != value)
            {
                _kernelSize = value;
                OnPropertyChanged();
            }
        }
    }

    public object GetParameters()
    {
        return new BadPixelSuppressionParameters(Threshold: _threshold, KernelSize: _kernelSize);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
