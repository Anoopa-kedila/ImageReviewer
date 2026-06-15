using ImageReviewerApp.Contracts;
using ImageReviewerApp.Enums;
using ImageReviewerApp.Models.OperationParameters;
using ImageReviewerApp.Operations;

namespace ImageReviewerApp.Factories;

public class ImageOperationFactory : IImageOperationFactory
{
    public IImageOperation CreateOperation(ImageOperation operation, object? parameters = null)
    {
        return operation switch
        {
            ImageOperation.WindowLevel => CreateWindowLevel(parameters),
            ImageOperation.GammaCorrection => CreateGammaCorrection(parameters),
            ImageOperation.GaussianFilter => CreateGaussianFilter(parameters),
            ImageOperation.MedianFilter => CreateMedianFilter(parameters),
            ImageOperation.Thresholding => CreateThresholding(parameters),
            ImageOperation.BadPixelSuppression => CreateBadPixelSuppression(parameters),
            _ => throw new NotSupportedException($"Operation '{operation}' is not supported.")
        };
    }

    public IEnumerable<ImageOperation> GetAvailableOperations()
    {
        return Enum.GetValues<ImageOperation>();
    }

    private IImageOperation CreateWindowLevel(object? parameters)
    {
        var p = parameters as WindowLevelParameters ?? new WindowLevelParameters();
        return new WindowLevelOperation(window: p.Window, level: p.Level);
    }

    private IImageOperation CreateGammaCorrection(object? parameters)
    {
        var p = parameters as GammaParameters ?? new GammaParameters();
        return new GammaCorrectionOperation(gamma: p.Gamma);
    }

    private IImageOperation CreateGaussianFilter(object? parameters)
    {
        var p = parameters as GaussianFilterParameters ?? new GaussianFilterParameters();
        return new GaussianFilterOperation(sigma: p.Sigma, kernelSize: p.KernelSize);
    }

    private IImageOperation CreateMedianFilter(object? parameters)
    {
        var p = parameters as MedianFilterParameters ?? new MedianFilterParameters();
        return new MedianFilterOperation(kernelSize: p.KernelSize);
    }

    private IImageOperation CreateThresholding(object? parameters)
    {
        var p = parameters as ThresholdParameters ?? new ThresholdParameters();
        return new ThresholdingOperation(threshold: p.Threshold);
    }

    private IImageOperation CreateBadPixelSuppression(object? parameters)
    {
        var p = parameters as BadPixelSuppressionParameters ?? new BadPixelSuppressionParameters();
        return new BadPixelSuppressionOperation(threshold: p.Threshold, kernelSize: p.KernelSize);
    }
}
