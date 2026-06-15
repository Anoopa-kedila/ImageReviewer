using ImageReviewerApp.Models.OperationParameters;
using ImageReviewerApp.ViewModels.OperationParameters;

namespace ImageReviewerApp.Tests.ViewModels.OperationParameters;

public class MedianFilterParametersViewModelTests
{
    [Fact]
    public void Constructor_SetsDefaultValue()
    {
        var viewModel = new MedianFilterParametersViewModel();

        Assert.Equal(3, viewModel.KernelSize);
    }

    [Fact]
    public void SetKernelSize_UpdatesProperty()
    {
        var viewModel = new MedianFilterParametersViewModel();

        viewModel.KernelSize = 5;

        Assert.Equal(5, viewModel.KernelSize);
    }

    [Fact]
    public void SetKernelSize_RaisesPropertyChanged()
    {
        var viewModel = new MedianFilterParametersViewModel();
        bool eventRaised = false;

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.KernelSize))
                eventRaised = true;
        };

        viewModel.KernelSize = 5;

        Assert.True(eventRaised);
    }

    [Fact]
    public void GetParameters_ReturnsCorrectType()
    {
        var viewModel = new MedianFilterParametersViewModel();

        var parameters = viewModel.GetParameters();

        Assert.IsType<MedianFilterParameters>(parameters);
    }

    [Fact]
    public void GetParameters_ReturnsCorrectValue()
    {
        var viewModel = new MedianFilterParametersViewModel
        {
            KernelSize = 7
        };

        var parameters = viewModel.GetParameters() as MedianFilterParameters;

        Assert.NotNull(parameters);
        Assert.Equal(7, parameters.KernelSize);
    }

    [Fact]
    public void SetKernelSize_EvenValue_IsValid()
    {
        var viewModel = new MedianFilterParametersViewModel();

        viewModel.KernelSize = 6;

        Assert.Equal(6, viewModel.KernelSize);
    }

    [Fact]
    public void SetKernelSize_SameValue_DoesNotRaisePropertyChanged()
    {
        var viewModel = new MedianFilterParametersViewModel();
        viewModel.KernelSize = 5;

        bool eventRaised = false;
        viewModel.PropertyChanged += (s, e) => eventRaised = true;

        viewModel.KernelSize = 5;

        Assert.False(eventRaised);
    }
}
