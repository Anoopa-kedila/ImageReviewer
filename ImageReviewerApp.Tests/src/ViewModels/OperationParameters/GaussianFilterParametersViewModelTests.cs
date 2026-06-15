using ImageReviewerApp.Models.OperationParameters;
using ImageReviewerApp.ViewModels.OperationParameters;

namespace ImageReviewerApp.Tests.ViewModels.OperationParameters;

public class GaussianFilterParametersViewModelTests
{
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        var viewModel = new GaussianFilterParametersViewModel();

        Assert.Equal(1.5, viewModel.Sigma);
        Assert.Equal(5, viewModel.KernelSize);
    }

    [Fact]
    public void SetSigma_UpdatesProperty()
    {
        var viewModel = new GaussianFilterParametersViewModel();

        viewModel.Sigma = 2.0;

        Assert.Equal(2.0, viewModel.Sigma);
    }

    [Fact]
    public void SetKernelSize_UpdatesProperty()
    {
        var viewModel = new GaussianFilterParametersViewModel();

        viewModel.KernelSize = 7;

        Assert.Equal(7, viewModel.KernelSize);
    }

    [Fact]
    public void SetSigma_RaisesPropertyChanged()
    {
        var viewModel = new GaussianFilterParametersViewModel();
        bool eventRaised = false;

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.Sigma))
                eventRaised = true;
        };

        viewModel.Sigma = 2.5;

        Assert.True(eventRaised);
    }

    [Fact]
    public void SetKernelSize_RaisesPropertyChanged()
    {
        var viewModel = new GaussianFilterParametersViewModel();
        bool eventRaised = false;

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.KernelSize))
                eventRaised = true;
        };

        viewModel.KernelSize = 9;

        Assert.True(eventRaised);
    }

    [Fact]
    public void GetParameters_ReturnsCorrectType()
    {
        var viewModel = new GaussianFilterParametersViewModel();

        var parameters = viewModel.GetParameters();

        Assert.IsType<GaussianFilterParameters>(parameters);
    }

    [Fact]
    public void GetParameters_ReturnsCorrectValues()
    {
        var viewModel = new GaussianFilterParametersViewModel
        {
            Sigma = 2.0,
            KernelSize = 7
        };

        var parameters = viewModel.GetParameters() as GaussianFilterParameters;

        Assert.NotNull(parameters);
        Assert.Equal(2.0, parameters.Sigma);
        Assert.Equal(7, parameters.KernelSize);
    }

    [Fact]
    public void SetSigma_SameValue_DoesNotRaisePropertyChanged()
    {
        var viewModel = new GaussianFilterParametersViewModel();
        viewModel.Sigma = 2.0;

        bool eventRaised = false;
        viewModel.PropertyChanged += (s, e) => eventRaised = true;

        viewModel.Sigma = 2.0;

        Assert.False(eventRaised);
    }
}
