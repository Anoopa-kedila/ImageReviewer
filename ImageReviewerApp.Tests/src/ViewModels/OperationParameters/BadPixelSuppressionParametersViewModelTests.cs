using ImageReviewerApp.Models.OperationParameters;
using ImageReviewerApp.ViewModels.OperationParameters;

namespace ImageReviewerApp.Tests.ViewModels.OperationParameters;

public class BadPixelSuppressionParametersViewModelTests
{
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        var viewModel = new BadPixelSuppressionParametersViewModel();

        Assert.Equal(3.0, viewModel.Threshold);
        Assert.Equal(3, viewModel.KernelSize);
    }

    [Fact]
    public void SetThreshold_UpdatesProperty()
    {
        var viewModel = new BadPixelSuppressionParametersViewModel();

        viewModel.Threshold = 5.0;

        Assert.Equal(5.0, viewModel.Threshold);
    }

    [Fact]
    public void SetKernelSize_UpdatesProperty()
    {
        var viewModel = new BadPixelSuppressionParametersViewModel();

        viewModel.KernelSize = 5;

        Assert.Equal(5, viewModel.KernelSize);
    }

    [Fact]
    public void SetThreshold_RaisesPropertyChanged()
    {
        var viewModel = new BadPixelSuppressionParametersViewModel();
        bool eventRaised = false;

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.Threshold))
                eventRaised = true;
        };

        viewModel.Threshold = 4.0;

        Assert.True(eventRaised);
    }

    [Fact]
    public void SetKernelSize_RaisesPropertyChanged()
    {
        var viewModel = new BadPixelSuppressionParametersViewModel();
        bool eventRaised = false;

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.KernelSize))
                eventRaised = true;
        };

        viewModel.KernelSize = 7;

        Assert.True(eventRaised);
    }

    [Fact]
    public void GetParameters_ReturnsCorrectType()
    {
        var viewModel = new BadPixelSuppressionParametersViewModel();

        var parameters = viewModel.GetParameters();

        Assert.IsType<BadPixelSuppressionParameters>(parameters);
    }

    [Fact]
    public void GetParameters_ReturnsCorrectValues()
    {
        var viewModel = new BadPixelSuppressionParametersViewModel
        {
            Threshold = 4.5,
            KernelSize = 5
        };

        var parameters = viewModel.GetParameters() as BadPixelSuppressionParameters;

        Assert.NotNull(parameters);
        Assert.Equal(4.5, parameters.Threshold);
        Assert.Equal(5, parameters.KernelSize);
    }

    [Fact]
    public void SetThreshold_SameValue_DoesNotRaisePropertyChanged()
    {
        var viewModel = new BadPixelSuppressionParametersViewModel();
        viewModel.Threshold = 3.0;

        bool eventRaised = false;
        viewModel.PropertyChanged += (s, e) => eventRaised = true;

        viewModel.Threshold = 3.0;

        Assert.False(eventRaised);
    }
}
