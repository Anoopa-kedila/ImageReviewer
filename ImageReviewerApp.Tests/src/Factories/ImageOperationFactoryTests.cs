using ImageReviewerApp.Contracts;
using ImageReviewerApp.Enums;
using ImageReviewerApp.Factories;
using ImageReviewerApp.Services;

namespace ImageReviewerApp.Tests.Factories;

public class ImageOperationFactoryTests
{
    [Fact]
    public void CreateOperation_ValidOperation_ReturnsOperation()
    {
        var factory = new ImageOperationFactory();

        var operation = factory.CreateOperation(ImageOperation.GammaCorrection, null);

        // Assert
        Assert.NotNull(operation);
        Assert.Equal("Gamma Correction", operation.Name);
    }

    [Fact]
    public void CreateOperation_WithParameters_UsesParameters()
    {
        var factory = new ImageOperationFactory();

        var operation = factory.CreateOperation(ImageOperation.WindowLevel,
            new ImageReviewerApp.Models.OperationParameters.WindowLevelParameters(Window: 30000, Level: 40000));

        // Assert
        Assert.NotNull(operation);
        Assert.Equal("Window/Level", operation.Name);
    }

    [Fact]
    public void CreateOperation_InvalidOperation_ThrowsNotSupportedException()
    {
        var factory = new ImageOperationFactory();

        Assert.Throws<NotSupportedException>(() =>
            factory.CreateOperation((ImageOperation)999, null));
    }

    [Fact]
    public void GetAvailableOperations_ReturnsAllOperations()
    {
        var factory = new ImageOperationFactory();

        var operations = factory.GetAvailableOperations().ToList();

        // Assert
        Assert.Contains(ImageOperation.WindowLevel, operations);
        Assert.Contains(ImageOperation.GammaCorrection, operations);
        Assert.Contains(ImageOperation.GaussianFilter, operations);
        Assert.Contains(ImageOperation.MedianFilter, operations);
        Assert.Contains(ImageOperation.Thresholding, operations);
        Assert.Contains(ImageOperation.BadPixelSuppression, operations);
        Assert.Equal(6, operations.Count);
    }
}
