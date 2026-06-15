namespace ImageReviewerApp.Models;

public class ImageMetadata
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int BitDepth { get; set; }
    public ushort MinIntensity { get; set; }
    public ushort MaxIntensity { get; set; }
    public double MeanIntensity { get; set; }

    public static ImageMetadata FromImageData(ImageData imageData)
    {
        var statistics = ImageStatistics.Calculate(imageData.PixelData);

        return new ImageMetadata
        {
            Width = imageData.Width,
            Height = imageData.Height,
            BitDepth = imageData.BitDepth,
            MinIntensity = statistics.MinIntensity,
            MaxIntensity = statistics.MaxIntensity,
            MeanIntensity = statistics.MeanIntensity
        };
    }
}
