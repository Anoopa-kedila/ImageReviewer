using ImageReviewerApp.Constants;

namespace ImageReviewerApp.Models.OperationParameters;

public record GammaParameters(
    double Gamma = ImageProcessingDefaults.DefaultGamma);
