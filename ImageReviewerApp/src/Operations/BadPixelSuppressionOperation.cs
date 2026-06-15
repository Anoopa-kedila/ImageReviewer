using ImageReviewerApp.Contracts;
using ImageReviewerApp.Models;

namespace ImageReviewerApp.Operations;

public class BadPixelSuppressionOperation : IImageOperation
{
    private const int MaxKernelSize = 31; // Reasonable limit for real-time processing
    private readonly double _threshold;
    private readonly int _kernelSize;

    public string Name => "Bad Pixel Suppression";

    public BadPixelSuppressionOperation(double threshold = 3.0, int kernelSize = 3)
    {
        if (kernelSize > MaxKernelSize)
            throw new ArgumentOutOfRangeException(nameof(kernelSize), $"Kernel size must be <= {MaxKernelSize}");

        _threshold = threshold > 0 ? threshold : 3.0;
        _kernelSize = kernelSize % 2 == 1 ? kernelSize : kernelSize + 1;
    }

    public ImageData Execute(ImageData image, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        progress?.Report(0.0);
        cancellationToken.ThrowIfCancellationRequested();

        var sourceData = image.PixelData;
        ushort[] result = new ushort[sourceData.Count];

        // Copy IReadOnlyList to array
        for (int i = 0; i < sourceData.Count; i++)
        {
            result[i] = sourceData[i];
        }

        progress?.Report(0.1);

        int radius = _kernelSize / 2;
        ushort[] neighbors = new ushort[_kernelSize * _kernelSize - 1];
        int totalPixels = image.Width * image.Height;
        int progressInterval = Math.Max(1, totalPixels / 100);

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                int centerIdx = y * image.Width + x;
                ushort centerPixel = sourceData[centerIdx];
                int count = 0;
                double sum = 0;

                for (int ky = -radius; ky <= radius; ky++)
                {
                    for (int kx = -radius; kx <= radius; kx++)
                    {
                        if (kx == 0 && ky == 0) continue;

                        int nx = Math.Clamp(x + kx, 0, image.Width - 1);
                        int ny = Math.Clamp(y + ky, 0, image.Height - 1);

                        ushort neighborPixel = sourceData[ny * image.Width + nx];
                        neighbors[count++] = neighborPixel;
                        sum += neighborPixel;
                    }
                }

                if (count == 0) continue;

                double mean = sum / count;
                double variance = 0;

                for (int i = 0; i < count; i++)
                {
                    double diff = neighbors[i] - mean;
                    variance += diff * diff;
                }

                double stdDev = Math.Sqrt(variance / count);
                double deviation = Math.Abs(centerPixel - mean);

                if (deviation > _threshold * stdDev)
                {
                    result[centerIdx] = QuickSelect(neighbors, 0, count - 1, count / 2);
                }

                if (centerIdx % progressInterval == 0)
                {
                    progress?.Report(0.1 + (double)centerIdx / totalPixels * 0.9);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }

        progress?.Report(1.0);
        return new ImageData(result, image.Width, image.Height, image.BitDepth);
    }

    private static ushort QuickSelect(ushort[] arr, int left, int right, int k)
    {
        while (left < right)
        {
            int pivotIndex = Partition(arr, left, right);

            if (pivotIndex == k)
                return arr[k];

            if (k < pivotIndex)
                right = pivotIndex - 1;
            else
                left = pivotIndex + 1;
        }

        return arr[left];
    }

    private static int Partition(ushort[] arr, int left, int right)
    {
        ushort pivot = arr[(left + right) / 2];
        int i = left - 1;
        int j = right + 1;

        while (true)
        {
            do { i++; } while (arr[i] < pivot);
            do { j--; } while (arr[j] > pivot);

            if (i >= j)
                return j;

            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }
}
