using ImageReviewerApp.Enums;

namespace ImageReviewerApp.Extensions;

public static class ImageOperationExtensions
{
    public static string ToDisplayName(this ImageOperation operation)
    {
        return operation switch
        {
            ImageOperation.WindowLevel => "Window/Level",
            ImageOperation.GammaCorrection => "Gamma Correction",
            ImageOperation.GaussianFilter => "Gaussian Filter",
            ImageOperation.MedianFilter => "Median Filter",
            ImageOperation.Thresholding => "Thresholding",
            ImageOperation.BadPixelSuppression => "Bad Pixel Suppression",
            _ => operation.ToString()
        };
    }

    public static ImageOperation? FromDisplayName(string displayName)
    {
        return displayName switch
        {
            "Window/Level" => ImageOperation.WindowLevel,
            "Gamma Correction" => ImageOperation.GammaCorrection,
            "Gaussian Filter" => ImageOperation.GaussianFilter,
            "Median Filter" => ImageOperation.MedianFilter,
            "Thresholding" => ImageOperation.Thresholding,
            "Bad Pixel Suppression" => ImageOperation.BadPixelSuppression,
            _ => null
        };
    }
}
