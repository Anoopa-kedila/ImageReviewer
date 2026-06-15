using ImageReviewerApp.Constants;

namespace ImageReviewerApp.Models.OperationParameters;

public record GaussianFilterParameters(
    double Sigma = ImageProcessingDefaults.DefaultSigma,
    int KernelSize = ImageProcessingDefaults.DefaultKernelSize);
