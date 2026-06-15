using ImageReviewerApp.Models.OperationParameters;
using ImageReviewerApp.ViewModels.OperationParameters;

namespace ImageReviewerApp.Tests.ViewModels.OperationParameters;

public class WindowLevelParametersViewModelTests
{
    [Fact]
    public void Constructor_SetsDefaultValues()
    {
        var viewModel = new WindowLevelParametersViewModel();

        Assert.Equal(65536, viewModel.Window);
        Assert.Equal(32768, viewModel.Level);
    }

    [Fact]
    public void SetWindow_UpdatesProperty()
    {
        var viewModel = new WindowLevelParametersViewModel();

        viewModel.Window = 30000;

        Assert.Equal(30000, viewModel.Window);
    }

    [Fact]
    public void SetLevel_UpdatesProperty()
    {
        var viewModel = new WindowLevelParametersViewModel();

        viewModel.Level = 40000;

        Assert.Equal(40000, viewModel.Level);
    }

    [Fact]
    public void SetWindow_RaisesPropertyChanged()
    {
        var viewModel = new WindowLevelParametersViewModel();
        bool eventRaised = false;

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.Window))
                eventRaised = true;
        };

        viewModel.Window = 30000;

        Assert.True(eventRaised);
    }

    [Fact]
    public void SetLevel_RaisesPropertyChanged()
    {
        var viewModel = new WindowLevelParametersViewModel();
        bool eventRaised = false;

        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.Level))
                eventRaised = true;
        };

        viewModel.Level = 40000;

        Assert.True(eventRaised);
    }

    [Fact]
    public void GetParameters_ReturnsCorrectType()
    {
        var viewModel = new WindowLevelParametersViewModel();

        var parameters = viewModel.GetParameters();

        Assert.IsType<WindowLevelParameters>(parameters);
    }

    [Fact]
    public void GetParameters_ReturnsCorrectValues()
    {
        var viewModel = new WindowLevelParametersViewModel
        {
            Window = 30000,
            Level = 40000
        };

        var parameters = viewModel.GetParameters() as WindowLevelParameters;

        Assert.NotNull(parameters);
        Assert.Equal(30000, parameters.Window);
        Assert.Equal(40000, parameters.Level);
    }

    [Fact]
    public void SetWindow_SameValue_DoesNotRaisePropertyChanged()
    {
        var viewModel = new WindowLevelParametersViewModel();
        viewModel.Window = 30000;

        bool eventRaised = false;
        viewModel.PropertyChanged += (s, e) => eventRaised = true;

        viewModel.Window = 30000;

        Assert.False(eventRaised);
    }
}
