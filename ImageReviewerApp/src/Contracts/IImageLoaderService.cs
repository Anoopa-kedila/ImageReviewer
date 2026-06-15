using ImageReviewerApp.Models;

namespace ImageReviewerApp.Contracts;

public interface IImageLoaderService
{

    Task<ImageData> LoadImageAsync(string filePath, CancellationToken cancellationToken = default);
}
