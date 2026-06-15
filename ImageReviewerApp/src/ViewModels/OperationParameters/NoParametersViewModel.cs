using System.ComponentModel;

namespace ImageReviewerApp.ViewModels.OperationParameters;

/// <summary>
/// Empty ViewModel for operations with no parameters (like Histogram Equalization).
/// </summary>
public class NoParametersViewModel : IOperationParametersViewModel
{
    public object? GetParameters() => null;

    public event PropertyChangedEventHandler? PropertyChanged;
}
