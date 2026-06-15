using ImageReviewerApp.Contracts;
using ImageReviewerApp.Models;
using ImageReviewerApp.Utilities;
using System.IO;
using System.Windows.Media.Imaging;

namespace ImageReviewerApp.Services;

public class ImageSaveService : IImageSaveService
{
    public Task SaveImageAsync(ImageData imageData, string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(imageData);

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty.", nameof(filePath));

        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var displayImage = ImageUtilities.ConvertToDisplayImage(imageData);

            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(displayImage));
            encoder.Save(fileStream);

        }, cancellationToken);
    }
}
