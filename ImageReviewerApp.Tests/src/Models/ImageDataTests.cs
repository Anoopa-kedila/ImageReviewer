using ImageReviewerApp.Models;

namespace ImageReviewerApp.Tests.Models;

public class ImageDataTests
{
    [Fact]
    public void Constructor_ValidData_CreatesImageDataWithCorrectProperties()
    {
        ushort[] pixels = { 100, 200, 300, 400 };
        int width = 2;
        int height = 2;

        var imageData = new ImageData(pixels, width, height);

        // Assert
        Assert.Equal(width, imageData.Width);
        Assert.Equal(height, imageData.Height);
        Assert.Equal(16, imageData.BitDepth);
        Assert.Equal(4, imageData.PixelData.Count);
    }

    [Fact]
    public void Constructor_CalculatesMinIntensityCorrectly()
    {
        ushort[] pixels = { 1000, 500, 1500, 200 };

        var imageData = new ImageData(pixels, 2, 2);
        var statistics = ImageStatistics.Calculate(imageData.PixelData);

        Assert.Equal(200, statistics.MinIntensity);
    }

    [Fact]
    public void Constructor_CalculatesMaxIntensityCorrectly()
    {
        ushort[] pixels = { 1000, 500, 1500, 200 };

        var imageData = new ImageData(pixels, 2, 2);
        var statistics = ImageStatistics.Calculate(imageData.PixelData);

        Assert.Equal(1500, statistics.MaxIntensity);
    }

    [Fact]
    public void Constructor_CalculatesMeanIntensityCorrectly()
    {
        ushort[] pixels = { 100, 200, 300, 400 };
        double expectedMean = (100 + 200 + 300 + 400) / 4.0;

        var imageData = new ImageData(pixels, 2, 2);
        var statistics = ImageStatistics.Calculate(imageData.PixelData);

        Assert.Equal(expectedMean, statistics.MeanIntensity);
    }

    [Fact]
    public void Constructor_NullPixelData_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ImageData(null!, 2, 2));
    }

    [Fact]
    public void Constructor_InvalidDimensions_ThrowsArgumentException()
    {
        ushort[] pixels = { 100, 200, 300, 400 };

        Assert.Throws<ArgumentException>(() => new ImageData(pixels, 0, 2));
        Assert.Throws<ArgumentException>(() => new ImageData(pixels, 2, 0));
        Assert.Throws<ArgumentException>(() => new ImageData(pixels, -1, 2));
    }

    [Fact]
    public void Constructor_MismatchedPixelDataLength_ThrowsArgumentException()
    {
        ushort[] pixels = { 100, 200, 300 };
        int width = 2;
        int height = 2;

        Assert.Throws<ArgumentException>(() => new ImageData(pixels, width, height));
    }

    [Fact]
    public void Clone_CreatesDeepCopy()
    {
        // Arrange
        ushort[] pixels = { 100, 200, 300, 400 };
        var original = new ImageData(pixels, 2, 2);

        // Act
        var clone = original.Clone();

        // Assert - Verify it's a different object with same values
        Assert.NotSame(original, clone);
        Assert.Equal(original.PixelData.Count, clone.PixelData.Count);
        for (int i = 0; i < original.PixelData.Count; i++)
        {
            Assert.Equal(original.PixelData[i], clone.PixelData[i]);
        }
        Assert.Equal(original.Width, clone.Width);
        Assert.Equal(original.Height, clone.Height);
    }
}
