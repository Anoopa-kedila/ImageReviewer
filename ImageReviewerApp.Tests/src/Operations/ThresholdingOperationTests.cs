using ImageReviewerApp.Models;
using ImageReviewerApp.Operations;

namespace ImageReviewerApp.Tests.Operations;

public class ThresholdingOperationTests
{

    [Fact]
    public void Execute_AppliesThresholding()
    {
        ushort[] pixels = { 10000, 20000, 30000, 40000, 50000 };
        var imageData = new ImageData(pixels, 5, 1);
        var operation = new ThresholdingOperation(threshold: 30000);

        var result = operation.Execute(imageData);

        Assert.Equal(0, result.PixelData[0]);
        Assert.Equal(0, result.PixelData[1]);
        Assert.Equal(65535, result.PixelData[2]);
        Assert.Equal(65535, result.PixelData[3]);
        Assert.Equal(65535, result.PixelData[4]);
    }

    [Fact]
    public void Execute_ThresholdAtMidpoint_SplitsCorrectly()
    {
        ushort[] pixels = { 0, 32767, 32768, 65535 };
        var imageData = new ImageData(pixels, 4, 1);
        var operation = new ThresholdingOperation(threshold: 32768);

        var result = operation.Execute(imageData);

        Assert.Equal(0, result.PixelData[0]);
        Assert.Equal(0, result.PixelData[1]);
        Assert.Equal(65535, result.PixelData[2]);
        Assert.Equal(65535, result.PixelData[3]);
    }

    [Fact]
    public void Execute_AllPixelsBelowThreshold_AllBlack()
    {
        ushort[] pixels = { 100, 200, 300, 400 };
        var imageData = new ImageData(pixels, 2, 2);
        var operation = new ThresholdingOperation(threshold: 500);

        var result = operation.Execute(imageData);

        // Assert
        Assert.All(result.PixelData.ToArray(), pixel => Assert.Equal(0, pixel));
    }

    [Fact]
    public void Execute_AllPixelsAboveThreshold_AllWhite()
    {
        ushort[] pixels = { 10000, 20000, 30000, 40000 };
        var imageData = new ImageData(pixels, 2, 2);
        var operation = new ThresholdingOperation(threshold: 100);

        var result = operation.Execute(imageData);

        Assert.All(result.PixelData.ToArray(), pixel => Assert.Equal(65535, pixel));
    }

    [Fact]
    public void Execute_ThresholdAtZero_AllWhite()
    {
        ushort[] pixels = { 0, 1, 100, 65535 };
        var imageData = new ImageData(pixels, 4, 1);
        var operation = new ThresholdingOperation(threshold: 0);

        var result = operation.Execute(imageData);

        // Assert
        Assert.All(result.PixelData.ToArray(), pixel => Assert.Equal(65535, pixel));
    }

    [Fact]
    public void Execute_ThresholdAtMax_AllBlack()
    {
        // Arrange
        ushort[] pixels = { 0, 32767, 65534 };
        var imageData = new ImageData(pixels, 3, 1);
        var operation = new ThresholdingOperation(threshold: 65535);

        // Act
        var result = operation.Execute(imageData);

        // Assert
        Assert.Equal(0, result.PixelData[0]);
        Assert.Equal(0, result.PixelData[1]);
        Assert.Equal(0, result.PixelData[2]);
    }

    [Fact]
    public void Execute_ThresholdAtMaxValue_OnlyMaxIsWhite()
    {
        // Arrange
        ushort[] pixels = { 0, 32767, 65534, 65535 };
        var imageData = new ImageData(pixels, 4, 1);
        var operation = new ThresholdingOperation(threshold: 65535);

        // Act
        var result = operation.Execute(imageData);

        // Assert
        Assert.Equal(0, result.PixelData[0]);
        Assert.Equal(0, result.PixelData[1]);
        Assert.Equal(0, result.PixelData[2]);
        Assert.Equal(65535, result.PixelData[3]); // Only max value passes
    }

    [Fact]
    public void Execute_SinglePixel_ThresholdsCorrectly()
    {
        // Arrange - Below threshold
        var imageBelow = new ImageData(new ushort[] { 100 }, 1, 1);
        var operation = new ThresholdingOperation(threshold: 200);

        // Act
        var resultBelow = operation.Execute(imageBelow);

        // Assert
        Assert.Equal(0, resultBelow.PixelData[0]);

        // Arrange - Above threshold
        var imageAbove = new ImageData(new ushort[] { 300 }, 1, 1);

        // Act
        var resultAbove = operation.Execute(imageAbove);

        // Assert
        Assert.Equal(65535, resultAbove.PixelData[0]);
    }

    [Fact]
    public void Execute_AlternatingPattern_CreatesCheckerboard()
    {
        // Arrange
        ushort[] pixels = { 100, 200, 100, 200, 100, 200 };
        var imageData = new ImageData(pixels, 3, 2);
        var operation = new ThresholdingOperation(threshold: 150);

        // Act
        var result = operation.Execute(imageData);

        // Assert
        Assert.Equal(0, result.PixelData[0]);
        Assert.Equal(65535, result.PixelData[1]);
        Assert.Equal(0, result.PixelData[2]);
        Assert.Equal(65535, result.PixelData[3]);
        Assert.Equal(0, result.PixelData[4]);
        Assert.Equal(65535, result.PixelData[5]);
    }

    [Fact]
    public void Execute_NullImage_ThrowsArgumentNullException()
    {
        var operation = new ThresholdingOperation();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => operation.Execute(null!));
    }

    [Fact]
    public void Constructor_DefaultThreshold_IsHalfOfMaxValue()
    {
        // Arrange & Act
        var operation = new ThresholdingOperation(); // Default threshold
        ushort[] pixels = { 32767, 32768, 32769 };
        var imageData = new ImageData(pixels, 3, 1);
        var result = operation.Execute(imageData);

        // Assert - Default threshold is 32768
        Assert.Equal(0, result.PixelData[0]); // Below default
        Assert.Equal(65535, result.PixelData[1]); // At default
        Assert.Equal(65535, result.PixelData[2]); // Above default
    }





    [Fact]
    public void Execute_PreservesDimensions()
    {
        // Arrange
        ushort[] pixels = new ushort[75 * 40];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = (ushort)(i % 65536);

        var imageData = new ImageData(pixels, 75, 40);
        var operation = new ThresholdingOperation(threshold: 30000);

        // Act
        var result = operation.Execute(imageData);

        // Assert
        Assert.Equal(imageData.Width, result.Width);
        Assert.Equal(imageData.Height, result.Height);
        Assert.Equal(imageData.PixelData.Count, result.PixelData.Count);
    }

    [Fact]
    public void Execute_PreservesBitDepth()
    {
        // Arrange
        ushort[] pixels = { 100, 200, 300 };
        var imageData = new ImageData(pixels, 3, 1, bitDepth: 16);
        var operation = new ThresholdingOperation(threshold: 200);

        // Act
        var result = operation.Execute(imageData);

        // Assert
        Assert.Equal(imageData.BitDepth, result.BitDepth);
    }





    [Fact]
    public void Execute_OutputOnlyContainsBlackOrWhite()
    {
        // Arrange
        ushort[] pixels = new ushort[100];
        var random = new Random(42);
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = (ushort)random.Next(0, 65536);

        var imageData = new ImageData(pixels, 10, 10);
        var operation = new ThresholdingOperation(threshold: 32768);

        // Act
        var result = operation.Execute(imageData);

        // Assert
        foreach (var pixel in result.PixelData.ToArray())
        {
            Assert.True(pixel == 0 || pixel == 65535, 
                $"Thresholded pixel should be either 0 or 65535, but was {pixel}");
        }
    }





    [Fact]
    public void Name_ReturnsCorrectValue()
    {
        // Arrange
        var operation = new ThresholdingOperation();

        // Act
        var name = operation.Name;

        // Assert
        Assert.Equal("Thresholding", name);
    }


}
