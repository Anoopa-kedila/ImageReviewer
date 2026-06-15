namespace ImageReviewerApp.Models;

public class ImageData
{
    private const int MaxDimension = 65536;

    private readonly ushort[] _pixelData;

    public IReadOnlyList<ushort> PixelData => _pixelData;

    public int Width { get; }
    public int Height { get; }
    public int BitDepth { get; }

    public ImageData(ushort[] pixelData, int width, int height, int bitDepth = 16)
    {
        if (pixelData == null || pixelData.Length == 0)
            throw new ArgumentNullException(nameof(pixelData));

        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive values.");

        if (width > MaxDimension || height > MaxDimension)
            throw new ArgumentException($"Image dimensions exceed maximum supported size of {MaxDimension}x{MaxDimension}.");

        if (pixelData.Length != width * height)
            throw new ArgumentException("Pixel data length must match width * height.");

        _pixelData = pixelData;
        Width = width;
        Height = height;
        BitDepth = bitDepth;
    }

    public ImageData Clone()
    {
        var newPixelData = new ushort[_pixelData.Length];
        Array.Copy(_pixelData, newPixelData, _pixelData.Length);
        return new ImageData(newPixelData, Width, Height, BitDepth);
    }
}

