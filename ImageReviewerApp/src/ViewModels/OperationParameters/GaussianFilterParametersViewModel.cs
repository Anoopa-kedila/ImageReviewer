using ImageReviewerApp.Models.OperationParameters;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageReviewerApp.ViewModels.OperationParameters;

public class GaussianFilterParametersViewModel : IOperationParametersViewModel
{
    private int _kernelSize = 5;
    private double _sigma = 1.5;

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

    public double Sigma
    {
        get => _sigma;
        set
        {
            if (_sigma != value)
            {
                _sigma = value;
                OnPropertyChanged();
            }
        }
    }

    public object GetParameters()
    {
        return new GaussianFilterParameters(Sigma: _sigma, KernelSize: _kernelSize);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
