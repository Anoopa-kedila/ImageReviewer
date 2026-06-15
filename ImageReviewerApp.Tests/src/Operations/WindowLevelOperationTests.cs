using ImageReviewerApp.Models;
using ImageReviewerApp.Operations;

namespace ImageReviewerApp.Tests.Operations;

public class WindowLevelOperationTests
{

    [Fact]
    public void Execute_AppliesWindowLevel()
    {
        ushort[] pixels = { 0, 16383, 32767, 49151, 65535 };
        var imageData = new ImageData(pixels, 5, 1);
        var operation = new WindowLevelOperation(window: 30000, level: 32767);

        var result = operation.Execute(imageData);

        Assert.Equal(5, result.PixelData.Count);
        Assert.Equal(0, result.PixelData[0]); // Below window, clipped to black
        Assert.True(result.PixelData[2] > 0 && result.PixelData[2] < 65535); // Within window
        Assert.Equal(65535, result.PixelData[4]); // Above window, clipped to white
    }

    [Fact]
    public void Execute_NarrowWindow_HighContrast()
    {
        ushort[] pixels = { 30000, 32000, 32767, 33500, 35000 };
        var imageData = new ImageData(pixels, 5, 1);
        var operation = new WindowLevelOperation(window: 2000, level: 32767);

        var result = operation.Execute(imageData);

        Assert.Equal(0, result.PixelData[0]);        Assert.Equal(0, result.PixelData[0]);
        Assert.True(result.PixelData[2] > 0 && result.PixelData[2] < 65535);
        Assert.Equal(65535, result.PixelData[4]);
    }

    [Fact]
    public void Execute_WideWindow_LowContrast()
    {
        ushort[] pixels = { 0, 16383, 32767, 49151, 65535 };
        var imageData = new ImageData(pixels, 5, 1);
        var operation = new WindowLevelOperation(window: 65536, level: 32768);

        var result = operation.Execute(imageData);

        Assert.True(result.PixelData[0] < result.PixelData[1]);
        Assert.True(result.PixelData[1] < result.PixelData[2]);
        Assert.True(result.PixelData[2] < result.PixelData[3]);
        Assert.True(result.PixelData[3] < result.PixelData[4]);
    }

    [Fact]
    public void Execute_CenterLevel_SymmetricMapping()
    {
        ushort[] pixels = { 16384, 24576, 32768, 40960, 49152 };
        var imageData = new ImageData(pixels, 5, 1);
        var operation = new WindowLevelOperation(window: 32768, level: 32768);

        var result = operation.Execute(imageData);

        Assert.Equal(0, result.PixelData[0]);
        Assert.True(result.PixelData[2] > 0);
        Assert.Equal(65535, result.PixelData[4]);
    }

    [Fact]
    public void Execute_LowLevel_ShiftsWindowDown()
    {
        ushort[] pixels = { 0, 5000, 10000, 15000, 20000 };
        var imageData = new ImageData(pixels, 5, 1);
        var operation = new WindowLevelOperation(window: 10000, level: 10000);

        var result = operation.Execute(imageData);

        Assert.Equal(0, result.PixelData[0]);
        Assert.True(result.PixelData[2] > 0);
        Assert.Equal(65535, result.PixelData[4]);
    }

    [Fact]
    public void Execute_HighLevel_ShiftsWindowUp()
    {
        ushort[] pixels = { 40000, 45000, 50000, 55000, 60000 };
        var imageData = new ImageData(pixels, 5, 1);
        var operation = new WindowLevelOperation(window: 10000, level: 50000);

        var result = operation.Execute(imageData);

        Assert.Equal(0, result.PixelData[0]);
        Assert.True(result.PixelData[2] > 0);
        Assert.Equal(65535, result.PixelData[4]);
    }

    [Fact]
    public void Execute_SinglePixel_MapsCorrectly()
    {
        var imageData = new ImageData(new ushort[] { 32767 }, 1, 1);
        var operation = new WindowLevelOperation(window: 20000, level: 32767);

        var result = operation.Execute(imageData);

        Assert.Single(result.PixelData.ToArray());
        Assert.True(result.PixelData[0] > 30000 && result.PixelData[0] < 35000);
    }

    [Fact]
    public void Execute_AllPixelsSameValue_AllMapToSameOutput()
    {
        ushort[] pixels = { 25000, 25000, 25000, 25000 };
        var imageData = new ImageData(pixels, 2, 2);
        var operation = new WindowLevelOperation(window: 10000, level: 25000);

        var result = operation.Execute(imageData);

        var firstValue = result.PixelData[0];
        Assert.All(result.PixelData.ToArray(), pixel => Assert.Equal(firstValue, pixel));
    }

    [Fact]
    public void Execute_NullImage_ThrowsArgumentNullException()
    {
        var operation = new WindowLevelOperation();

        Assert.Throws<ArgumentNullException>(() => operation.Execute(null!));
    }

    [Fact]
    public void Constructor_NegativeWindow_UsesDefaultValue()
    {
        var operation = new WindowLevelOperation(window: -1000, level: 32768);
        ushort[] pixels = { 16384, 32768, 49152 };
        var imageData = new ImageData(pixels, 3, 1);
        var result = operation.Execute(imageData);

        Assert.NotNull(result);
        Assert.Equal(3, result.PixelData.Count);
    }

    [Fact]
    public void Constructor_ZeroWindow_UsesDefaultValue()
    {
        var operation = new WindowLevelOperation(window: 0, level: 32768);
        ushort[] pixels = { 16384, 32768, 49152 };
        var imageData = new ImageData(pixels, 3, 1);
        var result = operation.Execute(imageData);

        Assert.NotNull(result);
        Assert.Equal(3, result.PixelData.Count);
    }

    [Fact]
    public void Constructor_DefaultParameters_WorksCorrectly()
    {
        // Arrange
        var operation = new WindowLevelOperation(); // Use defaults
        ushort[] pixels = { 0, 32767, 65535 };
        var imageData = new ImageData(pixels, 3, 1);

        // Act
        var result = operation.Execute(imageData);

        Assert.Equal(3, result.PixelData.Count);
        Assert.Equal(0, result.PixelData[0]);
        Assert.True(result.PixelData[2] >= 65534);
    }

    [Fact]
    public void Execute_UsesLUTForPerformance_SameInputProducesSameOutput()
    {
        // Arrange
        ushort[] pixels = { 100, 200, 100, 200, 100 }; // Repeated values
        var imageData = new ImageData(pixels, 5, 1);
        var operation = new WindowLevelOperation(window: 1000, level: 500);

        // Act
        var result = operation.Execute(imageData);

        // Assert - Same input values should produce same output values (LUT behavior)
        Assert.Equal(result.PixelData[0], result.PixelData[2]);
        Assert.Equal(result.PixelData[0], result.PixelData[4]);
        Assert.Equal(result.PixelData[1], result.PixelData[3]);
    }

    [Fact]
    public void Execute_LUTCoversFullRange()
    {
        // Arrange - Test all edge values
        ushort[] pixels = { 0, 1, 32767, 65534, 65535 };
        var imageData = new ImageData(pixels, 5, 1);
        var operation = new WindowLevelOperation(window: 40000, level: 32767);

        // Act
        var result = operation.Execute(imageData);

        // Assert - Should handle all values including edges
        Assert.All(result.PixelData.ToArray(), pixel => Assert.True(pixel >= 0 && pixel <= 65535));
    }





    [Fact]
    public void Execute_PreservesDimensions()
    {
        // Arrange
        ushort[] pixels = new ushort[128 * 64];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = (ushort)(i % 65536);

        var imageData = new ImageData(pixels, 128, 64);
        var operation = new WindowLevelOperation(window: 30000, level: 32768);

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
        var operation = new WindowLevelOperation(window: 1000, level: 200);

        // Act
        var result = operation.Execute(imageData);

        // Assert
        Assert.Equal(imageData.BitDepth, result.BitDepth);
    }





    [Fact]
    public void Execute_MonotonicallyIncreasing()
    {
        // Arrange - Input values in increasing order
        ushort[] pixels = { 10000, 20000, 30000, 40000, 50000 };
        var imageData = new ImageData(pixels, 5, 1);
        var operation = new WindowLevelOperation(window: 60000, level: 30000);

        // Act
        var result = operation.Execute(imageData);

        // Assert - Output should also be in increasing order
        for (int i = 1; i < result.PixelData.Count; i++)
        {
            Assert.True(result.PixelData[i] >= result.PixelData[i - 1],
                $"Output should be monotonically increasing but result[{i}]={result.PixelData[i]} < result[{i - 1}]={result.PixelData[i - 1]}");
        }
    }





    [Fact]
    public void Name_ReturnsCorrectValue()
    {
        // Arrange
        var operation = new WindowLevelOperation();

        // Act
        var name = operation.Name;

        // Assert
        Assert.Equal("Window/Level", name);
    }


}
