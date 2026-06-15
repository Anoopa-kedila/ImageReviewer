using ImageReviewerApp.Constants;

namespace ImageReviewerApp.Models.OperationParameters;

public record ThresholdParameters(
    ushort Threshold = ImageProcessingDefaults.DefaultThreshold);
