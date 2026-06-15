using ImageReviewerApp.Models;

namespace ImageReviewerApp.Tests.Models;

public class ImageStatisticsTests
{
    [Fact]
    public void Calculate_ReturnsCorrectMinValue()
    {
        ushort[] pixels = { 1000, 500, 1500, 200 };
        var imageData = new ImageData(pixels, 2, 2);

        var statistics = ImageStatistics.Calculate(imageData.PixelData);

        // Assert
        Assert.Equal(200, statistics.MinIntensity);
    }

    [Fact]
    public void Calculate_ReturnsCorrectMaxValue()
    {
        ushort[] pixels = { 1000, 500, 1500, 200 };
        var imageData = new ImageData(pixels, 2, 2);

        var statistics = ImageStatistics.Calculate(imageData.PixelData);

        // Assert
        Assert.Equal(1500, statistics.MaxIntensity);
    }

    [Fact]
    public void Calculate_ReturnsCorrectMean()
    {
        ushort[] pixels = { 100, 200, 300, 400 };
        var imageData = new ImageData(pixels, 2, 2);
        double expectedMean = (100 + 200 + 300 + 400) / 4.0;

        var statistics = ImageStatistics.Calculate(imageData.PixelData);

        // Assert
        Assert.Equal(expectedMean, statistics.MeanIntensity);
    }

    [Fact]
    public void Calculate_AllSameValue_ReturnsCorrectStats()
    {
        ushort[] pixels = { 5000, 5000, 5000, 5000 };
        var imageData = new ImageData(pixels, 2, 2);

        var statistics = ImageStatistics.Calculate(imageData.PixelData);

        // Assert
        Assert.Equal(5000, statistics.MinIntensity);
        Assert.Equal(5000, statistics.MaxIntensity);
        Assert.Equal(5000.0, statistics.MeanIntensity);
    }

    [Fact]
    public void Calculate_EdgeValues_HandlesMinAndMax()
    {
        ushort[] pixels = { 0, ushort.MaxValue, 100, 200 };
        var imageData = new ImageData(pixels, 2, 2);

        var statistics = ImageStatistics.Calculate(imageData.PixelData);

        // Assert
        Assert.Equal(0, statistics.MinIntensity);
        Assert.Equal(ushort.MaxValue, statistics.MaxIntensity);
    }

    [Fact]
    public void Calculate_NullPixelData_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            ImageStatistics.Calculate(null!));
    }

    [Fact]
    public void Calculate_EmptyPixelData_ThrowsArgumentException()
    {
        var emptyList = new List<ushort>();

        Assert.Throws<ArgumentException>(() =>
            ImageStatistics.Calculate(emptyList));
    }

    [Fact]
    public void Calculate_LargeDataset_CalculatesCorrectly()
    {
        // Arrange
        ushort[] pixels = new ushort[10000];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = (ushort)(i % 1000);
        
        var imageData = new ImageData(pixels, 100, 100);

        // Act
        var statistics = ImageStatistics.Calculate(imageData.PixelData);

        // Assert
        Assert.Equal(0, statistics.MinIntensity);
        Assert.Equal(999, statistics.MaxIntensity);
        Assert.True(statistics.MeanIntensity >= 0 && statistics.MeanIntensity < 1000);
    }
}
