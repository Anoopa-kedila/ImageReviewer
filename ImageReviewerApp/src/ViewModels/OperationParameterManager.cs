using ImageReviewerApp.Enums;
using ImageReviewerApp.ViewModels.OperationParameters;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageReviewerApp.ViewModels;

public class OperationParameterManager : INotifyPropertyChanged
{
    private readonly Dictionary<ImageOperation, IOperationParametersViewModel> _parameterViewModels;
    private IOperationParametersViewModel? _currentParameters;

    public OperationParameterManager()
    {
        _parameterViewModels = new Dictionary<ImageOperation, IOperationParametersViewModel>
        {
            [ImageOperation.WindowLevel] = new WindowLevelParametersViewModel(),
            [ImageOperation.GammaCorrection] = new GammaParametersViewModel(),
            [ImageOperation.GaussianFilter] = new GaussianFilterParametersViewModel(),
            [ImageOperation.MedianFilter] = new MedianFilterParametersViewModel(),
            [ImageOperation.Thresholding] = new ThresholdParametersViewModel(),
            [ImageOperation.BadPixelSuppression] = new BadPixelSuppressionParametersViewModel()
        };
    }

    public IOperationParametersViewModel? CurrentParameters
    {
        get => _currentParameters;
        private set
        {
            if (_currentParameters != value)
            {
                _currentParameters = value;
                OnPropertyChanged();
            }
        }
    }

    public void SetOperation(ImageOperation? operation)
    {
        if (operation.HasValue && _parameterViewModels.TryGetValue(operation.Value, out var parameters))
        {
            CurrentParameters = parameters;
        }
        else
        {
            CurrentParameters = null;
        }
    }

    public object? GetCurrentParameters()
    {
        return CurrentParameters?.GetParameters();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
