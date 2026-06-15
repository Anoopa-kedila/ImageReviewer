using ImageReviewerApp.Enums;
using ImageReviewerApp.ViewModels;
using ImageReviewerApp.ViewModels.OperationParameters;

namespace ImageReviewerApp.Tests.ViewModels;

public class OperationParameterManagerTests
{
    private readonly OperationParameterManager _manager;

    public OperationParameterManagerTests()
    {
        _manager = new OperationParameterManager();
    }

    [Fact]
    public void Constructor_InitializesAllViewModels()
    {
        Assert.Null(_manager.CurrentParameters);
    }

    [Fact]
    public void SetOperation_WindowLevel_UpdatesCurrentParameters()
    {
        _manager.SetOperation(ImageOperation.WindowLevel);

        Assert.NotNull(_manager.CurrentParameters);
        Assert.IsType<WindowLevelParametersViewModel>(_manager.CurrentParameters);
    }

    [Fact]
    public void SetOperation_GammaCorrection_UpdatesCurrentParameters()
    {
        _manager.SetOperation(ImageOperation.GammaCorrection);

        Assert.NotNull(_manager.CurrentParameters);
        Assert.IsType<GammaParametersViewModel>(_manager.CurrentParameters);
    }

    [Fact]
    public void SetOperation_GaussianFilter_UpdatesCurrentParameters()
    {
        _manager.SetOperation(ImageOperation.GaussianFilter);

        Assert.NotNull(_manager.CurrentParameters);
        Assert.IsType<GaussianFilterParametersViewModel>(_manager.CurrentParameters);
    }

    [Fact]
    public void SetOperation_MedianFilter_UpdatesCurrentParameters()
    {
        _manager.SetOperation(ImageOperation.MedianFilter);

        Assert.NotNull(_manager.CurrentParameters);
        Assert.IsType<MedianFilterParametersViewModel>(_manager.CurrentParameters);
    }

    [Fact]
    public void SetOperation_Thresholding_UpdatesCurrentParameters()
    {
        _manager.SetOperation(ImageOperation.Thresholding);

        Assert.NotNull(_manager.CurrentParameters);
        Assert.IsType<ThresholdParametersViewModel>(_manager.CurrentParameters);
    }

    [Fact]
    public void SetOperation_BadPixelSuppression_UpdatesCurrentParameters()
    {
        _manager.SetOperation(ImageOperation.BadPixelSuppression);

        Assert.NotNull(_manager.CurrentParameters);
        Assert.IsType<BadPixelSuppressionParametersViewModel>(_manager.CurrentParameters);
    }

    [Fact]
    public void SetOperation_Null_ClearsCurrentParameters()
    {
        _manager.SetOperation(ImageOperation.WindowLevel);
        Assert.NotNull(_manager.CurrentParameters);

        _manager.SetOperation(null);

        Assert.Null(_manager.CurrentParameters);
    }

    [Fact]
    public void SetOperation_RaisesPropertyChanged()
    {
        bool eventRaised = false;
        _manager.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(_manager.CurrentParameters))
                eventRaised = true;
        };

        _manager.SetOperation(ImageOperation.GammaCorrection);

        Assert.True(eventRaised);
    }

    [Fact]
    public void GetCurrentParameters_ReturnsCorrectParameterObject()
    {
        _manager.SetOperation(ImageOperation.GammaCorrection);

        var parameters = _manager.GetCurrentParameters();

        Assert.NotNull(parameters);
        Assert.IsType<ImageReviewerApp.Models.OperationParameters.GammaParameters>(parameters);
    }

    [Fact]
    public void GetCurrentParameters_NoOperationSet_ReturnsNull()
    {
        var parameters = _manager.GetCurrentParameters();

        Assert.Null(parameters);
    }

    [Fact]
    public void SwitchingOperations_RetainsSeparateViewModels()
    {
        _manager.SetOperation(ImageOperation.WindowLevel);
        var windowLevelVM = _manager.CurrentParameters as WindowLevelParametersViewModel;
        windowLevelVM!.Window = 30000;

        _manager.SetOperation(ImageOperation.GammaCorrection);
        var gammaVM = _manager.CurrentParameters as GammaParametersViewModel;
        gammaVM!.Gamma = 1.5;

        _manager.SetOperation(ImageOperation.WindowLevel);

        Assert.Equal(30000, ((WindowLevelParametersViewModel)_manager.CurrentParameters!).Window);
    }

    [Fact]
    public void AllOperations_HaveParameterViewModels()
    {
        var allOperations = Enum.GetValues<ImageOperation>();

        foreach (var operation in allOperations)
        {
            _manager.SetOperation(operation);
            Assert.NotNull(_manager.CurrentParameters);
        }
    }
}
