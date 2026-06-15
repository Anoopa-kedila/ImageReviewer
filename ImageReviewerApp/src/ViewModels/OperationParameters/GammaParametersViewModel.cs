using ImageReviewerApp.Models.OperationParameters;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImageReviewerApp.ViewModels.OperationParameters;

public class GammaParametersViewModel : IOperationParametersViewModel
{
    private double _gamma = 2.2;

    public double Gamma
    {
        get => _gamma;
        set
        {
            if (_gamma != value)
            {
                _gamma = value;
                OnPropertyChanged();
            }
        }
    }

    public object GetParameters()
    {
        return new GammaParameters(Gamma: _gamma);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
