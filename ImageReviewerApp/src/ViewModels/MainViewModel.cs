using ImageReviewerApp.Commands;
using ImageReviewerApp.Constants;
using ImageReviewerApp.Contracts;
using ImageReviewerApp.Enums;
using ImageReviewerApp.Extensions;
using ImageReviewerApp.Models;
using ImageReviewerApp.Utilities;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ImageReviewerApp.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IImageLoaderService _imageLoader;
    private readonly IImageSaveService _imageSaver;
    private readonly IImageOperationFactory _operationFactory;
    private readonly IDialogService _dialogService;
    private readonly OperationParameterManager _parameterManager;

    private ImageData? _originalImage;
    private ImageData? _processedImage;
    private CancellationTokenSource? _operationCts;
    private ImageOperation? _currentOperation;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public AsyncRelayCommand LoadImageCommand { get; }
    public AsyncRelayCommand ApplyOperationCommand { get; }
    public RelayCommand ResetCommand { get; }
    public RelayCommand CancelCommand { get; }
    public AsyncRelayCommand SaveImageCommand { get; }

    private BitmapSource? _originalDisplayImage;
    public BitmapSource? OriginalDisplayImage
    {
        get => _originalDisplayImage;
        set => SetProperty(ref _originalDisplayImage, value);
    }

    private BitmapSource? _processedDisplayImage;
    public BitmapSource? ProcessedDisplayImage
    {
        get => _processedDisplayImage;
        set => SetProperty(ref _processedDisplayImage, value);
    }

    private ImageMetadata? _imageMetadata;
    public ImageMetadata? ImageMetadata
    {
        get => _imageMetadata;
        set => SetProperty(ref _imageMetadata, value);
    }

    private string? _selectedOperation;
    public string? SelectedOperation
    {
        get => _selectedOperation;
        set
        {
            if (SetProperty(ref _selectedOperation, value))
            {
                HandleOperationSelection(value);
            }
        }
    }

    private List<string> _availableOperations;
    public List<string> AvailableOperations
    {
        get => _availableOperations;
        set => SetProperty(ref _availableOperations, value);
    }

    public OperationParameterManager ParameterManager => _parameterManager;

    private bool _isProcessing;
    public bool IsProcessing
    {
        get => _isProcessing;
        set
        {
            if (SetProperty(ref _isProcessing, value))
            {
                CancelCommand?.RaiseCanExecuteChanged();
            }
        }
    }

    private string _statusMessage = "Ready";
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    private int _progressPercentage;
    public int ProgressPercentage
    {
        get => _progressPercentage;
        set => SetProperty(ref _progressPercentage, value);
    }

    public MainViewModel(
        IImageLoaderService imageLoader,
        IImageSaveService imageSaver,
        IImageOperationFactory operationFactory,
        IDialogService dialogService)
    {
        _imageLoader = imageLoader;
        _imageSaver = imageSaver;
        _operationFactory = operationFactory;
        _dialogService = dialogService;
        _parameterManager = new OperationParameterManager();

        _availableOperations = _operationFactory.GetAvailableOperations()
            .Select(op => op.ToDisplayName())
            .ToList();

        LoadImageCommand = new AsyncRelayCommand(LoadImageAsync);
        ApplyOperationCommand = new AsyncRelayCommand(ApplyOperationAsync, CanApplyOperation);
        ResetCommand = new RelayCommand(Reset, CanReset);
        CancelCommand = new RelayCommand(CancelOperation, CanCancel);
        SaveImageCommand = new AsyncRelayCommand(SaveImageAsync, CanSaveImage);
    }

        private async Task LoadImageAsync()
        {
            var filePath = _dialogService.ShowOpenFileDialog(
                "TIFF Images (*.tif;*.tiff)|*.tif;*.tiff|All Files (*.*)|*.*",
                "Select 16-bit Grayscale TIFF Image");
            if (filePath == null) return;

        await ExecuteWithProgressAsync(
            statusMessage: "Loading image...",
            operation: async () =>
            {
                ClearImages();

                _originalImage = await _imageLoader.LoadImageAsync(filePath, _operationCts!.Token);
                ImageMetadata = ImageMetadata.FromImageData(_originalImage);
                OriginalDisplayImage = ImageUtilities.ConvertToDisplayImage(_originalImage);

                ProcessedDisplayImage = null;
                StatusMessage = $"Image loaded: {_originalImage.Width}x{_originalImage.Height}";
            },
            onComplete: () => NotifyCommandStates()
        );
    }

    private async Task ApplyOperationAsync()
    {
        var operationEnum = ImageOperationExtensions.FromDisplayName(SelectedOperation!);
        if (operationEnum == null)
        {
            StatusMessage = "Invalid operation selected.";
            return;
        }

        await ExecuteWithProgressAsync(
            statusMessage: $"Applying {SelectedOperation}...",
                 operation: async () =>
            {
                var parameters = _parameterManager.GetCurrentParameters();
                var operation = _operationFactory.CreateOperation(operationEnum.Value, parameters);
                _processedImage = null;

                var progress = new Progress<double>(p =>
                {
                    ProgressPercentage = (int)(p * 100);
                });

                _processedImage = await Task.Run(() => 
                    operation.Execute(_originalImage!, progress, _operationCts!.Token), 
                    _operationCts!.Token);

                ProcessedDisplayImage = ImageUtilities.ConvertToDisplayImage(_processedImage);
                StatusMessage = $"{SelectedOperation} applied successfully.";
            },
            onComplete: () => NotifyCommandStates()
        );
    }

    private void Reset()
    {
        _processedImage = null;
        ProcessedDisplayImage = null;
        StatusMessage = "Reset to original image.";

        NotifyCommandStates();
    }

    private async Task SaveImageAsync()
    {
        var filePath = _dialogService.ShowSaveFileDialog(
            DialogConstants.PngFilter,
            DialogConstants.SaveImageTitle,
            DialogConstants.PngExtension);
        if (filePath == null) return;

        await ExecuteWithProgressAsync(
            statusMessage: "Saving image...",
            operation: async () =>
            {
                await _imageSaver.SaveImageAsync(_processedImage!, filePath);
                StatusMessage = $"Image saved: {filePath}";
            }
        );
    }

    private bool CanApplyOperation() =>
        _originalImage != null && !string.IsNullOrWhiteSpace(SelectedOperation) && !IsProcessing;

    private bool CanReset() =>
        _originalImage != null && _processedImage != null && !IsProcessing;

    private bool CanCancel() =>
        IsProcessing;

    private bool CanSaveImage() =>
        _processedImage != null && !IsProcessing;

    private async Task ExecuteWithProgressAsync(
        string statusMessage,
        Func<Task> operation,
        Action? onComplete = null)
    {
        CancelCurrentOperation();

        try
        {
            StatusMessage = statusMessage;
            ProgressPercentage = 0;
            IsProcessing = true;

            await operation();

            ProgressPercentage = 100;
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Operation cancelled.";
            ProgressPercentage = 0;
        }
        catch (FileNotFoundException ex)
        {
            HandleException("File not found.", "The selected file could not be found.", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            HandleException("Access denied.", "You don't have permission to access this file.", ex);
        }
        catch (InvalidDataException ex)
        {
            HandleException("Invalid image format.", "The file is not a valid 16-bit grayscale TIFF image.", ex);
        }
        catch (OutOfMemoryException ex)
        {
            HandleException("Out of memory.", "The image is too large to load. Try a smaller image.", ex);
        }
        catch (Exception ex)
        {
            HandleException($"Error: {ex.Message}", $"Operation failed: {ex.Message}", ex);
        }
        finally
        {
            IsProcessing = false;
            ProgressPercentage = 0;
            onComplete?.Invoke();
        }
    }

    private void HandleOperationSelection(string? displayName)
    {
        _currentOperation = ImageOperationExtensions.FromDisplayName(displayName);
        _parameterManager.SetOperation(_currentOperation);

        ApplyOperationCommand.RaiseCanExecuteChanged();
    }

    private void HandleException(string statusMessage, string dialogMessage, Exception ex)
    {
        StatusMessage = statusMessage;
        _dialogService.ShowError($"{dialogMessage}: {ex.Message}");
    }

    private void CancelOperation()
    {
        _operationCts?.Cancel();
        StatusMessage = "Cancelling operation...";
    }

    private void CancelCurrentOperation()
    {
        _operationCts?.Cancel();
        _operationCts?.Dispose();
        _operationCts = new CancellationTokenSource();
    }

    private void ClearImages()
    {
        _originalImage = null;
        _processedImage = null;
    }

    private void NotifyCommandStates()
    {
        ApplyOperationCommand.RaiseCanExecuteChanged();
        ResetCommand.RaiseCanExecuteChanged();
        CancelCommand.RaiseCanExecuteChanged();
        SaveImageCommand.RaiseCanExecuteChanged();
    }

    }
