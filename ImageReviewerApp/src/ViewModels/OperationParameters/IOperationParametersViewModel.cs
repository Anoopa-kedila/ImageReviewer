using System.ComponentModel;

namespace ImageReviewerApp.ViewModels.OperationParameters;

/// <summary>
/// Base interface for operation parameter ViewModels.
/// Each operation has its own ViewModel for parameters.
/// </summary>
public interface IOperationParametersViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the operation-specific parameter object to pass to the operation.
    /// </summary>
    object GetParameters();
}
