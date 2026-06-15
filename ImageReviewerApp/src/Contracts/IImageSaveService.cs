using ImageReviewerApp.Models;

namespace ImageReviewerApp.Contracts;

public interface IImageSaveService
{
    Task SaveImageAsync(ImageData imageData, string filePath, CancellationToken cancellationToken = default);
}
