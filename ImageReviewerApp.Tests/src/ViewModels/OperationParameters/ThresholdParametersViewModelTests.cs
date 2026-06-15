using ImageReviewerApp.Models.OperationParameters;
using ImageReviewerApp.ViewModels.OperationParameters;

namespace ImageReviewerApp.Tests.ViewModels.OperationParameters;

public class ThresholdParametersViewModelTests
{
    [Fact]
    public void Constructor_SetsDefaultValue()
    {
        var viewModel = new ThresholdParametersViewModel();

        Assert.Equal(32768, viewModel.Threshold);
    }

    [Fact]
    public void SetThreshold_UpdatesProperty()
    {
        var viewModel = new ThresholdParametersViewModel();

        viewModel.Threshold = 40000;

        Assert.Equal(40000, viewModel.Threshold);
    }

    [Fact]
    public void SetThreshold_RaisesPropertyChanged()
    {
        var viewModel = new ThresholdParametersViewModel();
        bool eventRaised = false;

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.Threshold))
                eventRaised = true;
        };

        viewModel.Threshold = 40000;

        Assert.True(eventRaised);
    }

    [Fact]
    public void GetParameters_ReturnsCorrectType()
    {
        var viewModel = new ThresholdParametersViewModel();

        var parameters = viewModel.GetParameters();

        Assert.IsType<ThresholdParameters>(parameters);
    }

    [Fact]
    public void GetParameters_ReturnsCorrectValue()
    {
        var viewModel = new ThresholdParametersViewModel
        {
            Threshold = 40000
        };

        var parameters = viewModel.GetParameters() as ThresholdParameters;

        Assert.NotNull(parameters);
        Assert.Equal(40000, parameters.Threshold);
    }
}
