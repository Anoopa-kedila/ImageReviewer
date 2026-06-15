using ImageReviewerApp.Models;

namespace ImageReviewerApp.Contracts;

public interface IImageOperation
{
    string Name { get; }

    ImageData Execute(ImageData image, IProgress<double>? progress = null, CancellationToken cancellationToken = default);
}
