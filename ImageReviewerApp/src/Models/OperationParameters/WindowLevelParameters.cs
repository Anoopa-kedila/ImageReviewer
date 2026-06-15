using ImageReviewerApp.Constants;

namespace ImageReviewerApp.Models.OperationParameters;

public record WindowLevelParameters(
    double Window = ImageProcessingDefaults.DefaultWindow,
    double Level = ImageProcessingDefaults.DefaultLevel);
