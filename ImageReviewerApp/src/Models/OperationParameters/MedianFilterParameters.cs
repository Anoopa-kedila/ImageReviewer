using ImageReviewerApp.Constants;

namespace ImageReviewerApp.Models.OperationParameters;

public record MedianFilterParameters(
    int KernelSize = ImageProcessingDefaults.DefaultMedianKernelSize);
