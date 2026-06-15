using ImageReviewerApp.Contracts;
using ImageReviewerApp.Models;
using System.IO;
using System.Windows.Media.Imaging;

namespace ImageReviewerApp.Services;

public class ImageLoaderService : IImageLoaderService
{
    public Task<ImageData> LoadImageAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Image file not found.", filePath);

        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filePath, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();

            cancellationToken.ThrowIfCancellationRequested();

            BitmapSource processedBitmap = bitmap;

            if (bitmap.Format != System.Windows.Media.PixelFormats.Gray16)
            {
                processedBitmap = new FormatConvertedBitmap(bitmap, System.Windows.Media.PixelFormats.Gray16, null, 0);
                processedBitmap.Freeze();
            }

            int width = processedBitmap.PixelWidth;
            int height = processedBitmap.PixelHeight;
            int stride = width * 2;

            byte[] rawData = new byte[stride * height];
            processedBitmap.CopyPixels(rawData, stride, 0);

            cancellationToken.ThrowIfCancellationRequested();

            ushort[] pixelData = new ushort[width * height];
            for (int i = 0; i < pixelData.Length; i++)
            {
                pixelData[i] = (ushort)(rawData[i * 2] | (rawData[i * 2 + 1] << 8));
            }

            return new ImageData(pixelData, width, height, 16);
        }, cancellationToken);
    }
}
