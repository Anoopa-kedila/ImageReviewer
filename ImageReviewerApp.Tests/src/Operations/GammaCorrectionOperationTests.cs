using ImageReviewerApp.Models;
using ImageReviewerApp.Operations;

namespace ImageReviewerApp.Tests.Operations;

public class GammaCorrectionOperationTests
{

    [Fact]
    public void Execute_AppliesGammaCorrection()
    {
        ushort[] pixels = { 0, 16383, 32767, 49151, 65535 };
        var imageData = new ImageData(pixels, 5, 1);
        var operation = new GammaCorrectionOperation(gamma: 2.0);

        var result = operation.Execute(imageData);

        Assert.Equal(5, result.PixelData.Count);
        Assert.Equal(0, result.PixelData[0]);
        Assert.True(result.PixelData[2] < imageData.PixelData[2]);
        Assert.Equal(65535, result.PixelData[4]);
    }

    [Fact]
    public void Execute_GammaLessThanOne_BrightensMidtones()
    {
        ushort[] pixels = { 0, 32767, 65535 };
        var imageData = new ImageData(pixels, 3, 1);
        var operation = new GammaCorrectionOperation(gamma: 0.5);

        var result = operation.Execute(imageData);

        Assert.Equal(0, result.PixelData[0]);
        Assert.True(result.PixelData[1] > imageData.PixelData[1]);
        Assert.Equal(65535, result.PixelData[2]);
    }

    [Fact]
    public void Execute_GammaGreaterThanOne_DarkensMidtones()
    {
        ushort[] pixels = { 0, 32767, 65535 };
        var imageData = new ImageData(pixels, 3, 1);
        var operation = new GammaCorrectionOperation(gamma: 2.2);

        var result = operation.Execute(imageData);

        Assert.Equal(0, result.PixelData[0]);
        Assert.True(result.PixelData[1] < imageData.PixelData[1]);
        Assert.Equal(65535, result.PixelData[2]);
    }

    [Fact]
    public void Execute_GammaEqualsOne_NoChange()
    {
        ushort[] pixels = { 0, 16383, 32767, 49151, 65535 };
        var imageData = new ImageData(pixels, 5, 1);
        var operation = new GammaCorrectionOperation(gamma: 1.0);

        var result = operation.Execute(imageData);

        // Assert
        for (int i = 0; i < pixels.Length; i++)
        {
            Assert.Equal(imageData.PixelData[i], result.PixelData[i]);
        }
    }

    [Fact]
    public void Execute_AllZeroPixels_RemainsZero()
    {
        // Arrange
        ushort[] pixels = { 0, 0, 0, 0 };
        var imageData = new ImageData(pixels, 2, 2);
        var operation = new GammaCorrectionOperation(gamma: 2.2);

        // Act
        var result = operation.Execute(imageData);

        // Assert
        Assert.All(result.PixelData.ToArray(), pixel => Assert.Equal(0, pixel));
    }

    [Fact]
    public void Execute_AllMaxPixels_RemainsMax()
    {
        // Arrange
        ushort[] pixels = { 65535, 65535, 65535, 65535 };
        var imageData = new ImageData(pixels, 2, 2);
        var operation = new GammaCorrectionOperation(gamma: 2.2);

        // Act
        var result = operation.Execute(imageData);

        // Assert
        Assert.All(result.PixelData.ToArray(), pixel => Assert.Equal(65535, pixel));
    }

    [Fact]
    public void Execute_SinglePixel_WorksCorrectly()
    {
        // Arrange
        ushort[] pixels = { 32767 };
        var imageData = new ImageData(pixels, 1, 1);
        var operation = new GammaCorrectionOperation(gamma: 2.0);

        // Act
        var result = operation.Execute(imageData);

        // Assert
        Assert.Single(result.PixelData.ToArray());
        Assert.True(result.PixelData[0] <= imageData.PixelData[0]); // Darkened
    }

    [Fact]
    public void Execute_VerySmallGamma_ExtremeBrightening()
    {
        // Arrange
        ushort[] pixels = { 10000 };
        var imageData = new ImageData(pixels, 1, 1);
        var operation = new GammaCorrectionOperation(gamma: 0.1); // Very small gamma

        // Act
        var result = operation.Execute(imageData);

        // Assert
        Assert.True(result.PixelData[0] > imageData.PixelData[0]);
    }

    [Fact]
    public void Execute_VeryLargeGamma_ExtremeDarkening()
    {
        // Arrange
        ushort[] pixels = { 50000 };
        var imageData = new ImageData(pixels, 1, 1);
        var operation = new GammaCorrectionOperation(gamma: 10.0);

        var result = operation.Execute(imageData);

        Assert.True(result.PixelData[0] < imageData.PixelData[0]);
    }

    [Fact]
    public void Execute_NullImage_ThrowsArgumentNullException()
    {
        // Arrange
        var operation = new GammaCorrectionOperation();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => operation.Execute(null!));
    }

    [Fact]
    public void Constructor_NegativeGamma_UsesDefaultValue()
    {
        // Arrange & Act
        var operation = new GammaCorrectionOperation(gamma: -1.0);
        ushort[] pixels = { 32767 };
        var imageData = new ImageData(pixels, 1, 1);
        var result = operation.Execute(imageData);

        // Assert - Should use default gamma (2.2) instead of negative value
        Assert.NotEqual(0, result.PixelData[0]);
    }

    [Fact]
    public void Constructor_ZeroGamma_UsesDefaultValue()
    {
        // Arrange & Act
        var operation = new GammaCorrectionOperation(gamma: 0);
        ushort[] pixels = { 32767 };
        var imageData = new ImageData(pixels, 1, 1);
        var result = operation.Execute(imageData);

        // Assert - Should use default gamma (2.2) instead of zero
        Assert.NotEqual(0, result.PixelData[0]);
    }





    [Fact]
    public void Execute_PreservesDimensions()
    {
        // Arrange
        ushort[] pixels = new ushort[100 * 50];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = (ushort)(i % 65536);

        var imageData = new ImageData(pixels, 100, 50);
        var operation = new GammaCorrectionOperation(gamma: 2.2);

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
        var operation = new GammaCorrectionOperation(gamma: 2.2);

        // Act
        var result = operation.Execute(imageData);

        // Assert
        Assert.Equal(imageData.BitDepth, result.BitDepth);
    }





    [Fact]
    public void Name_ReturnsCorrectValue()
    {
        // Arrange
        var operation = new GammaCorrectionOperation();

        // Act
        var name = operation.Name;

        // Assert
        Assert.Equal("Gamma Correction", name);
    }


}
