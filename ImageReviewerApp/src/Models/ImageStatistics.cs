namespace ImageReviewerApp.Models;

public class ImageStatistics
{
    public ushort MinIntensity { get; init; }
    public ushort MaxIntensity { get; init; }
    public double MeanIntensity { get; init; }

    public static ImageStatistics Calculate(IReadOnlyList<ushort> pixelData)
    {
        if (pixelData == null || pixelData.Count == 0)
            throw new ArgumentException("Pixel data cannot be null or empty.", nameof(pixelData));

        ushort min = ushort.MaxValue;
        ushort max = ushort.MinValue;
        long sum = 0;

        for (int i = 0; i < pixelData.Count; i++)
        {
            ushort pixel = pixelData[i];
            if (pixel < min) min = pixel;
            if (pixel > max) max = pixel;
            sum += pixel;
        }

        return new ImageStatistics
        {
            MinIntensity = min,
            MaxIntensity = max,
            MeanIntensity = (double)sum / pixelData.Count
        };
    }
}
