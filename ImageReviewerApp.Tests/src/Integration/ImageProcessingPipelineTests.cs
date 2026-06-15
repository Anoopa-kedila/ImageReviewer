using ImageReviewerApp.Contracts;
using ImageReviewerApp.Enums;
using ImageReviewerApp.Models;
using ImageReviewerApp.Operations;
using ImageReviewerApp.Services;

namespace ImageReviewerApp.Tests.Integration;

public class ImageProcessingPipelineTests
{
    [Fact]
    public void WindowLevel_ThenGamma_ProducesExpectedOutput()
    {
        var imageData = new ImageData(new ushort[] 
        { 
            0, 16383, 32767, 49151, 65535,
            10000, 20000, 30000, 40000, 50000
        }, 5, 2);

        var windowLevel = new WindowLevelOperation(window: 40000, level: 32767);
        var gammaCorrection = new GammaCorrectionOperation(gamma: 1.5);

        var afterWindowLevel = windowLevel.Execute(imageData);
        var final = gammaCorrection.Execute(afterWindowLevel);

        Assert.NotNull(final);
        Assert.Equal(5, final.Width);
        Assert.Equal(2, final.Height);
        Assert.Equal(10, final.PixelData.Count);
    }

    [Fact]
    public void GaussianFilter_ThenMedianFilter_ProducesValidOutput()
    {
        var imageData = CreateTestImage(10, 10);

        var gaussian = new GaussianFilterOperation(sigma: 1.5, kernelSize: 5);
        var median = new MedianFilterOperation(kernelSize: 3);

        var afterGaussian = gaussian.Execute(imageData);
        var final = median.Execute(afterGaussian);

        Assert.NotNull(final);
        Assert.Equal(10, final.Width);
        Assert.Equal(10, final.Height);
        Assert.Equal(100, final.PixelData.Count);
    }

    [Fact]
    public void BadPixelSuppression_ThenWindowLevel_PreservesImageStructure()
    {
        var imageData = CreateImageWithBadPixels(10, 10);

        var badPixelSuppression = new BadPixelSuppressionOperation(threshold: 3.0, kernelSize: 3);
        var windowLevel = new WindowLevelOperation(window: 50000, level: 30000);

        var afterSuppression = badPixelSuppression.Execute(imageData);
        var final = windowLevel.Execute(afterSuppression);

        Assert.NotNull(final);
        Assert.Equal(10, final.Width);
        Assert.Equal(10, final.Height);
    }

    [Fact]
    public void MultipleOperations_Sequential_MaintainDataIntegrity()
    {
        var imageData = CreateTestImage(5, 5);

        var op1 = new WindowLevelOperation(window: 40000, level: 30000);
        var op2 = new GammaCorrectionOperation(gamma: 2.0);
        var op3 = new MedianFilterOperation(kernelSize: 3);

        var step1 = op1.Execute(imageData);
        Assert.Equal(25, step1.PixelData.Count);

        var step2 = op2.Execute(step1);
        Assert.Equal(25, step2.PixelData.Count);

        var step3 = op3.Execute(step2);
        Assert.Equal(25, step3.PixelData.Count);
        Assert.Equal(5, step3.Width);
        Assert.Equal(5, step3.Height);
    }

    [Fact]
    public void CancellationToken_StopsOperation()
    {
        var imageData = CreateLargeImage();
        var cts = new CancellationTokenSource();
        var operation = new MedianFilterOperation(kernelSize: 7);

        cts.Cancel();

        Assert.Throws<OperationCanceledException>(() =>
            operation.Execute(imageData, null, cts.Token));
    }

    [Fact]
    public void ProgressReporting_WorksThroughPipeline()
    {
        var imageData = CreateTestImage(10, 10);
        var progressValues = new List<double>();

        var progress = new Progress<double>(p => progressValues.Add(p));
        var operation = new GaussianFilterOperation(sigma: 1.5, kernelSize: 5);

        operation.Execute(imageData, progress);

        Assert.NotEmpty(progressValues);
        Assert.True(progressValues[0] >= 0);
        Assert.True(progressValues[^1] <= 1.0);
    }

    [Fact]
    public void AllOperations_ProduceValidBitDepth()
    {
        var imageData = new ImageData(new ushort[] { 100, 200, 300, 400 }, 2, 2, bitDepth: 16);

        var operations = new IImageOperation[]
        {
            new WindowLevelOperation(),
            new GammaCorrectionOperation(),
            new GaussianFilterOperation(),
            new MedianFilterOperation(),
            new ThresholdingOperation(),
            new BadPixelSuppressionOperation()
        };

        foreach (var operation in operations)
        {
            var result = operation.Execute(imageData);
            Assert.Equal(16, result.BitDepth);
        }
    }

    [Fact]
    public void SmallImage_AllOperations_WorkCorrectly()
    {
        var imageData = new ImageData(new ushort[] { 100 }, 1, 1);

        var windowLevel = new WindowLevelOperation();
        var result1 = windowLevel.Execute(imageData);
        Assert.Single(result1.PixelData.ToArray());

        var gamma = new GammaCorrectionOperation();
        var result2 = gamma.Execute(imageData);
        Assert.Single(result2.PixelData.ToArray());

        var threshold = new ThresholdingOperation();
        var result3 = threshold.Execute(imageData);
        Assert.Single(result3.PixelData.ToArray());
    }

    [Fact]
    public void LargeImage_ProcessedWithoutException()
    {
        var imageData = CreateLargeImage();

        var operation = new GaussianFilterOperation(sigma: 1.0, kernelSize: 3);

        var result = operation.Execute(imageData);

        Assert.NotNull(result);
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    private ImageData CreateTestImage(int width, int height)
    {
        var pixels = new ushort[width * height];
        var random = new Random(42);

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = (ushort)random.Next(0, 65536);
        }

        return new ImageData(pixels, width, height);
    }

    private ImageData CreateImageWithBadPixels(int width, int height)
    {
        var pixels = new ushort[width * height];
        var random = new Random(42);

        for (int i = 0; i < pixels.Length; i++)
        {
            if (i % 10 == 0)
                pixels[i] = 65535;
            else
                pixels[i] = (ushort)random.Next(20000, 40000);
        }

        return new ImageData(pixels, width, height);
    }

    private ImageData CreateLargeImage()
    {
        var pixels = new ushort[100 * 100];
        var random = new Random(42);

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = (ushort)random.Next(0, 65536);
        }

        return new ImageData(pixels, 100, 100);
    }
}
