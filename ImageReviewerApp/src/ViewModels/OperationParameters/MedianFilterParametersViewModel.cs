using ImageReviewerApp.Models.OperationParameters;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageReviewerApp.ViewModels.OperationParameters;

public class MedianFilterParametersViewModel : IOperationParametersViewModel
{
    private int _kernelSize = 3;

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
        return new MedianFilterParameters(KernelSize: _kernelSize);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
