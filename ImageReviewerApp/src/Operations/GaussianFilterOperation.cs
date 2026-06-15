using ImageReviewerApp.Contracts;
using ImageReviewerApp.Models;

namespace ImageReviewerApp.Operations;

public class GaussianFilterOperation : IImageOperation
{
    private const int MaxKernelSize = 31; // Reasonable limit for real-time processing
    private readonly double _sigma;
    private readonly int _kernelSize;

    public string Name => "Gaussian Filter";

    public GaussianFilterOperation(double sigma = 1.5, int kernelSize = 5)
    {
        if (kernelSize > MaxKernelSize)
            throw new ArgumentOutOfRangeException(nameof(kernelSize), $"Kernel size must be <= {MaxKernelSize}");

        _sigma = sigma > 0 ? sigma : 1.5;
        _kernelSize = kernelSize % 2 == 1 ? kernelSize : kernelSize + 1;
    }

    public ImageData Execute(ImageData image, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        progress?.Report(0.1);
        cancellationToken.ThrowIfCancellationRequested();

        var sourceData = image.PixelData;

        progress?.Report(0.2);
        cancellationToken.ThrowIfCancellationRequested();

        double[] kernel = GenerateGaussianKernel(_kernelSize, _sigma);

        progress?.Report(0.3);

        var tempData = ApplyConvolution1D(sourceData, image.Width, image.Height, kernel, true, 
            p => progress?.Report(0.3 + p * 0.35), cancellationToken);

        var resultData = ApplyConvolution1D(tempData, image.Width, image.Height, kernel, false,
            p => progress?.Report(0.65 + p * 0.35), cancellationToken);

        progress?.Report(1.0);
        return new ImageData(resultData, image.Width, image.Height, image.BitDepth);
    }

    private double[] GenerateGaussianKernel(int size, double sigma)
    {
        double[] kernel = new double[size];
        int radius = size / 2;
        double sum = 0;

        for (int i = 0; i < size; i++)
        {
            int x = i - radius;
            kernel[i] = Math.Exp(-(x * x) / (2 * sigma * sigma));
            sum += kernel[i];
        }

        for (int i = 0; i < size; i++)
        {
            kernel[i] /= sum;
        }

        return kernel;
    }

    private ushort[] ApplyConvolution1D(IReadOnlyList<ushort> data, int width, int height, double[] kernel, bool horizontal,
        Action<double>? progress = null, CancellationToken cancellationToken = default)
    {
        ushort[] result = new ushort[data.Count];
        int radius = kernel.Length / 2;
        int totalPixels = width * height;
        int progressInterval = Math.Max(1, totalPixels / 100);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                double sum = 0;

                for (int k = 0; k < kernel.Length; k++)
                {
                    int offset = k - radius;
                    int nx = horizontal ? x + offset : x;
                    int ny = horizontal ? y : y + offset;

                    nx = Math.Clamp(nx, 0, width - 1);
                    ny = Math.Clamp(ny, 0, height - 1);

                    int index = ny * width + nx;
                    sum += data[index] * kernel[k];
                }

                result[y * width + x] = (ushort)Math.Clamp(sum, 0, 65535);

                // Report progress and check cancellation
                int pixelIndex = y * width + x;
                if (pixelIndex % progressInterval == 0)
                {
                    progress?.Invoke((double)pixelIndex / totalPixels);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        return result;
    }
}
