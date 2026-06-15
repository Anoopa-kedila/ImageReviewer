using ImageReviewerApp.Models.OperationParameters;
using ImageReviewerApp.ViewModels.OperationParameters;

namespace ImageReviewerApp.Tests.ViewModels.OperationParameters;

public class GammaParametersViewModelTests
{
    [Fact]
    public void Constructor_SetsDefaultValue()
    {
        var viewModel = new GammaParametersViewModel();

        Assert.Equal(2.2, viewModel.Gamma);
    }

    [Fact]
    public void SetGamma_UpdatesProperty()
    {
        var viewModel = new GammaParametersViewModel();

        viewModel.Gamma = 1.8;

        Assert.Equal(1.8, viewModel.Gamma);
    }

    [Fact]
    public void SetGamma_RaisesPropertyChanged()
    {
        var viewModel = new GammaParametersViewModel();
        bool eventRaised = false;

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.Gamma))
                eventRaised = true;
        };

        viewModel.Gamma = 1.5;

        Assert.True(eventRaised);
    }

    [Fact]
    public void GetParameters_ReturnsCorrectType()
    {
        var viewModel = new GammaParametersViewModel();

        var parameters = viewModel.GetParameters();

        Assert.IsType<GammaParameters>(parameters);
    }

    [Fact]
    public void GetParameters_ReturnsCorrectValue()
    {
        var viewModel = new GammaParametersViewModel
        {
            Gamma = 1.8
        };

        var parameters = viewModel.GetParameters() as GammaParameters;

        Assert.NotNull(parameters);
        Assert.Equal(1.8, parameters.Gamma);
    }
}
