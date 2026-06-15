using ImageReviewerApp.Models;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageReviewerApp.Utilities;

public static class ImageUtilities
{
    public static BitmapSource ConvertToDisplayImage(ImageData imageData)
    {
        ArgumentNullException.ThrowIfNull(imageData);

        var statistics = ImageStatistics.Calculate(imageData.PixelData);

        byte[] displayData = new byte[imageData.PixelData.Count];
        ushort min = statistics.MinIntensity;
        ushort max = statistics.MaxIntensity;
        double range = max - min;
        if (range < 1) range = 1;

        for (int i = 0; i < imageData.PixelData.Count; i++)
        {
            double normalized = ((imageData.PixelData[i] - min) / range) * 255.0;
            displayData[i] = (byte)Math.Clamp(normalized, 0, 255);
        }

        var bitmap = BitmapSource.Create(
            imageData.Width,
            imageData.Height,
            96,
            96,
            PixelFormats.Gray8,
            null,
            displayData,
            imageData.Width);

        bitmap.Freeze();
        return bitmap;
    }
}
