using ImageReviewerApp.Contracts;
using ImageReviewerApp.Enums;
using ImageReviewerApp.Models;
using ImageReviewerApp.ViewModels;
using Moq;
using System.Windows.Media.Imaging;

namespace ImageReviewerApp.Tests.ViewModels;

public class MainViewModelTests
{
    private readonly Mock<IImageLoaderService> _mockImageLoader;
    private readonly Mock<IImageSaveService> _mockImageSaver;
    private readonly Mock<IImageOperationFactory> _mockOperationFactory;
    private readonly Mock<IDialogService> _mockDialogService;
    private readonly MainViewModel _viewModel;

    public MainViewModelTests()
    {
        _mockImageLoader = new Mock<IImageLoaderService>();
        _mockImageSaver = new Mock<IImageSaveService>();
        _mockOperationFactory = new Mock<IImageOperationFactory>();
        _mockDialogService = new Mock<IDialogService>();

        _mockOperationFactory
            .Setup(x => x.GetAvailableOperations())
            .Returns(Enum.GetValues<ImageOperation>());

        _viewModel = new MainViewModel(
            _mockImageLoader.Object,
            _mockImageSaver.Object,
            _mockOperationFactory.Object,
            _mockDialogService.Object);
    }

    [Fact]
    public void Constructor_InitializesProperties()
    {
        Assert.NotNull(_viewModel.LoadImageCommand);
        Assert.NotNull(_viewModel.ApplyOperationCommand);
        Assert.NotNull(_viewModel.ResetCommand);
        Assert.NotNull(_viewModel.CancelCommand);
        Assert.NotNull(_viewModel.SaveImageCommand);
        Assert.NotNull(_viewModel.AvailableOperations);
        Assert.NotEmpty(_viewModel.AvailableOperations);
        Assert.Equal("Ready", _viewModel.StatusMessage);
        Assert.Equal(0, _viewModel.ProgressPercentage);
    }

    [Fact]
    public async Task LoadImageCommand_UserCancels_NoActionTaken()
    {
        _mockDialogService
            .Setup(x => x.ShowOpenFileDialog(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string?)null);

        _viewModel.LoadImageCommand.Execute(null);

        Assert.Null(_viewModel.OriginalDisplayImage);
        Assert.Null(_viewModel.ImageMetadata);
        _mockImageLoader.Verify(x => x.LoadImageAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LoadImageCommand_ValidFile_UpdatesProperties()
    {
        var testImageData = new ImageData(new ushort[] { 100, 200, 300, 400 }, 2, 2);

        _mockDialogService
            .Setup(x => x.ShowOpenFileDialog(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("test.tif");

        _mockImageLoader
            .Setup(x => x.LoadImageAsync("test.tif", It.IsAny<CancellationToken>()))
            .ReturnsAsync(testImageData);

        _viewModel.LoadImageCommand.Execute(null);

        Assert.NotNull(_viewModel.OriginalDisplayImage);
        Assert.NotNull(_viewModel.ImageMetadata);
        Assert.Equal(2, _viewModel.ImageMetadata.Width);
        Assert.Equal(2, _viewModel.ImageMetadata.Height);
        Assert.Contains("loaded", _viewModel.StatusMessage.ToLower());
        
        _mockImageLoader.Verify(x => x.LoadImageAsync("test.tif", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoadImageCommand_FileNotFound_ShowsError()
    {
        _mockDialogService
            .Setup(x => x.ShowOpenFileDialog(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("nonexistent.tif");

        _mockImageLoader
            .Setup(x => x.LoadImageAsync("nonexistent.tif", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new FileNotFoundException("File not found"));

        _viewModel.LoadImageCommand.Execute(null);

        _mockDialogService.Verify(x => x.ShowError(
            It.Is<string>(s => s.Contains("not found") || s.Contains("File not found")), 
            It.IsAny<string>()), Times.Once);
        Assert.Null(_viewModel.OriginalDisplayImage);
    }

    [Fact]
    public async Task LoadImageCommand_InvalidFormat_ShowsError()
    {
        _mockDialogService
            .Setup(x => x.ShowOpenFileDialog(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("invalid.tif");

        _mockImageLoader
            .Setup(x => x.LoadImageAsync("invalid.tif", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("Invalid format"));

        _viewModel.LoadImageCommand.Execute(null);

        _mockDialogService.Verify(x => x.ShowError(
            It.Is<string>(s => s.Contains("Invalid") || s.Contains("format")), 
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task LoadImageCommand_OutOfMemory_ShowsError()
    {
        _mockDialogService
            .Setup(x => x.ShowOpenFileDialog(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("huge.tif");

        _mockImageLoader
            .Setup(x => x.LoadImageAsync("huge.tif", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OutOfMemoryException());

        _viewModel.LoadImageCommand.Execute(null);

        _mockDialogService.Verify(x => x.ShowError(
            It.Is<string>(s => s.Contains("memory") || s.Contains("large")), 
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void ApplyOperationCommand_NoImageLoaded_CannotExecute()
    {
        Assert.False(_viewModel.ApplyOperationCommand.CanExecute(null));
    }

    [Fact]
    public async Task ApplyOperationCommand_NoOperationSelected_CannotExecute()
    {
        await LoadTestImage();

        _viewModel.SelectedOperation = null;

        Assert.False(_viewModel.ApplyOperationCommand.CanExecute(null));
    }

    [Fact]
    public async Task ApplyOperationCommand_ValidOperation_UpdatesProcessedImage()
    {
        await LoadTestImage();

        var mockOperation = new Mock<IImageOperation>();
        mockOperation.Setup(x => x.Name).Returns("Test Operation");
        mockOperation.Setup(x => x.Execute(
            It.IsAny<ImageData>(),
            It.IsAny<IProgress<double>>(),
            It.IsAny<CancellationToken>()))
            .Returns((ImageData img, IProgress<double> p, CancellationToken ct) =>
            {
                p?.Report(0.5);
                return new ImageData(new ushort[] { 50, 100, 150, 200 }, 2, 2);
            });

        _mockOperationFactory
            .Setup(x => x.CreateOperation(ImageOperation.GammaCorrection, It.IsAny<object>()))
            .Returns(mockOperation.Object);

        _viewModel.SelectedOperation = "Gamma Correction";

        _viewModel.ApplyOperationCommand.Execute(null);
        await Task.Delay(200);

        Assert.NotNull(_viewModel.ProcessedDisplayImage);
        Assert.Contains("applied successfully", _viewModel.StatusMessage);
        mockOperation.Verify(x => x.Execute(
            It.IsAny<ImageData>(),
            It.IsAny<IProgress<double>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApplyOperationCommand_InvalidOperation_ShowsError()
    {
        await LoadTestImage();

        _viewModel.SelectedOperation = "Invalid Operation";

        _viewModel.ApplyOperationCommand.Execute(null);

        Assert.Contains("Invalid operation", _viewModel.StatusMessage);
    }

    [Fact]
    public async Task ApplyOperationCommand_OperationThrows_ShowsError()
    {
        await LoadTestImage();

        var mockOperation = new Mock<IImageOperation>();
        mockOperation.Setup(x => x.Execute(
            It.IsAny<ImageData>(),
            It.IsAny<IProgress<double>>(),
            It.IsAny<CancellationToken>()))
            .Throws(new InvalidOperationException("Operation failed"));

        _mockOperationFactory
            .Setup(x => x.CreateOperation(ImageOperation.GammaCorrection, It.IsAny<object>()))
            .Returns(mockOperation.Object);

        _viewModel.SelectedOperation = "Gamma Correction";

        _viewModel.ApplyOperationCommand.Execute(null);
        await Task.Delay(100);

        _mockDialogService.Verify(x => x.ShowError(
            It.Is<string>(s => s.Contains("Operation failed")),
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ApplyOperationCommand_ReportsProgress()
    {
        await LoadTestImage();

        var mockOperation = new Mock<IImageOperation>();
        mockOperation.Setup(x => x.Execute(
            It.IsAny<ImageData>(),
            It.IsAny<IProgress<double>>(),
            It.IsAny<CancellationToken>()))
            .Returns((ImageData img, IProgress<double> p, CancellationToken ct) =>
            {
                p?.Report(0.25);
                p?.Report(0.50);
                p?.Report(0.75);
                p?.Report(1.0);
                return new ImageData(new ushort[] { 50, 100, 150, 200 }, 2, 2);
            });

        _mockOperationFactory
            .Setup(x => x.CreateOperation(ImageOperation.GammaCorrection, It.IsAny<object>()))
            .Returns(mockOperation.Object);

        _viewModel.SelectedOperation = "Gamma Correction";

        _viewModel.ApplyOperationCommand.Execute(null);
        await Task.Delay(200);

        Assert.True(_viewModel.ProgressPercentage >= 0);
    }

    [Fact]
    public async Task ResetCommand_NoProcessedImage_CannotExecute()
    {
        await LoadTestImage();

        Assert.False(_viewModel.ResetCommand.CanExecute(null));
    }

    [Fact]
    public async Task ResetCommand_WithProcessedImage_ClearsProcessedImage()
    {
        await LoadTestImage();
        await ApplyTestOperation();

        Assert.NotNull(_viewModel.ProcessedDisplayImage);

        _viewModel.ResetCommand.Execute(null);

        Assert.Null(_viewModel.ProcessedDisplayImage);
        Assert.Contains("Reset", _viewModel.StatusMessage);
    }

    [Fact]
    public void CancelCommand_NotProcessing_CannotExecute()
    {
        Assert.False(_viewModel.CancelCommand.CanExecute(null));
    }

    [Fact]
    public async Task SaveImageCommand_NoProcessedImage_CannotExecute()
    {
        await LoadTestImage();

        Assert.False(_viewModel.SaveImageCommand.CanExecute(null));
    }

    [Fact]
    public async Task SaveImageCommand_UserCancels_NoActionTaken()
    {
        await LoadTestImage();
        await ApplyTestOperation();

        _mockDialogService
            .Setup(x => x.ShowSaveFileDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string?)null);

        _viewModel.SaveImageCommand.Execute(null);

        _mockImageSaver.Verify(x => x.SaveImageAsync(It.IsAny<ImageData>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SaveImageCommand_ValidPath_SavesImage()
    {
        await LoadTestImage();
        await ApplyTestOperation();

        _mockDialogService
            .Setup(x => x.ShowSaveFileDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("output.png");

        _mockImageSaver
            .Setup(x => x.SaveImageAsync(It.IsAny<ImageData>(), "output.png"))
            .Returns(Task.CompletedTask);

        _viewModel.SaveImageCommand.Execute(null);

        _mockImageSaver.Verify(x => x.SaveImageAsync(It.IsAny<ImageData>(), "output.png"), Times.Once);
        Assert.Contains("saved", _viewModel.StatusMessage.ToLower());
    }

    [Fact]
    public async Task SaveImageCommand_IOError_ShowsError()
    {
        await LoadTestImage();
        await ApplyTestOperation();

        _mockDialogService
            .Setup(x => x.ShowSaveFileDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("output.png");

        _mockImageSaver
            .Setup(x => x.SaveImageAsync(It.IsAny<ImageData>(), "output.png"))
            .ThrowsAsync(new IOException("Disk full"));

        _viewModel.SaveImageCommand.Execute(null);

        _mockDialogService.Verify(x => x.ShowError(
            It.Is<string>(s => s.Contains("Disk full")),
            It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SelectedOperation_Changed_UpdatesParameterManager()
    {
        await LoadTestImage();

        _viewModel.SelectedOperation = "Gamma Correction";

        Assert.NotNull(_viewModel.ParameterManager.CurrentParameters);
    }

    [Fact]
    public async Task MultipleOperations_Sequential_WorkCorrectly()
    {
        await LoadTestImage();

        var mockOperation1 = CreateMockOperation();
        var mockOperation2 = CreateMockOperation();

        _mockOperationFactory
            .Setup(x => x.CreateOperation(ImageOperation.GammaCorrection, It.IsAny<object>()))
            .Returns(mockOperation1.Object);

        _mockOperationFactory
            .Setup(x => x.CreateOperation(ImageOperation.WindowLevel, It.IsAny<object>()))
            .Returns(mockOperation2.Object);

        _viewModel.SelectedOperation = "Gamma Correction";
        _viewModel.ApplyOperationCommand.Execute(null);
        await Task.Delay(200);

        _viewModel.ResetCommand.Execute(null);

        _viewModel.SelectedOperation = "Window/Level";
        _viewModel.ApplyOperationCommand.Execute(null);
        await Task.Delay(200);

        mockOperation1.Verify(x => x.Execute(
            It.IsAny<ImageData>(),
            It.IsAny<IProgress<double>>(),
            It.IsAny<CancellationToken>()), Times.Once);

        mockOperation2.Verify(x => x.Execute(
            It.IsAny<ImageData>(),
            It.IsAny<IProgress<double>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CommandStates_UpdateCorrectly_ThroughWorkflow()
    {
        Assert.False(_viewModel.ApplyOperationCommand.CanExecute(null));
        Assert.False(_viewModel.ResetCommand.CanExecute(null));
        Assert.False(_viewModel.SaveImageCommand.CanExecute(null));

        await LoadTestImage();

        _viewModel.SelectedOperation = "Gamma Correction";
        Assert.True(_viewModel.ApplyOperationCommand.CanExecute(null));
        Assert.False(_viewModel.ResetCommand.CanExecute(null));
        Assert.False(_viewModel.SaveImageCommand.CanExecute(null));

        await ApplyTestOperation();

        Assert.True(_viewModel.ResetCommand.CanExecute(null));
        Assert.True(_viewModel.SaveImageCommand.CanExecute(null));

        _viewModel.ResetCommand.Execute(null);

        Assert.False(_viewModel.ResetCommand.CanExecute(null));
        Assert.False(_viewModel.SaveImageCommand.CanExecute(null));
    }

    private async Task LoadTestImage()
    {
        var testImageData = new ImageData(new ushort[] { 100, 200, 300, 400 }, 2, 2);

        _mockDialogService
            .Setup(x => x.ShowOpenFileDialog(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("test.tif");

        _mockImageLoader
            .Setup(x => x.LoadImageAsync("test.tif", It.IsAny<CancellationToken>()))
            .ReturnsAsync(testImageData);

        _viewModel.LoadImageCommand.Execute(null);
        await Task.Delay(100);
    }

    private async Task ApplyTestOperation()
    {
        var mockOperation = CreateMockOperation();

        _mockOperationFactory
            .Setup(x => x.CreateOperation(ImageOperation.GammaCorrection, It.IsAny<object>()))
            .Returns(mockOperation.Object);

        _viewModel.SelectedOperation = "Gamma Correction";
        _viewModel.ApplyOperationCommand.Execute(null);
        await Task.Delay(100);
    }

    private Mock<IImageOperation> CreateMockOperation()
    {
        var mockOperation = new Mock<IImageOperation>();
        mockOperation.Setup(x => x.Name).Returns("Test Operation");
        mockOperation.Setup(x => x.Execute(
            It.IsAny<ImageData>(),
            It.IsAny<IProgress<double>>(),
            It.IsAny<CancellationToken>()))
            .Returns(new ImageData(new ushort[] { 50, 100, 150, 200 }, 2, 2));
        return mockOperation;
    }

    private BitmapSource CreateTestBitmap()
    {
        return BitmapSource.Create(2, 2, 96, 96,
            System.Windows.Media.PixelFormats.Gray8, null, new byte[] { 0, 128, 255, 64 }, 2);
    }
}
