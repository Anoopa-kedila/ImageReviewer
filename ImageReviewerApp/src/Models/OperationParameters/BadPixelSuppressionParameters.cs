using ImageReviewerApp.Constants;

namespace ImageReviewerApp.Models.OperationParameters;

public record BadPixelSuppressionParameters(
    double Threshold = ImageProcessingDefaults.DefaultBadPixelThreshold,
    int KernelSize = ImageProcessingDefaults.DefaultBadPixelKernelSize);
