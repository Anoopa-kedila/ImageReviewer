using ImageReviewerApp.Contracts;
using ImageReviewerApp.Models;

namespace ImageReviewerApp.Operations;

public class MedianFilterOperation : IImageOperation
{
    private const int MaxKernelSize = 31; // Reasonable limit for real-time processing
    private readonly int _kernelSize;

    public string Name => "Median Filter";

    public MedianFilterOperation(int kernelSize = 3)
    {
        if (kernelSize > MaxKernelSize)
            throw new ArgumentOutOfRangeException(nameof(kernelSize), $"Kernel size must be <= {MaxKernelSize}");

        _kernelSize = kernelSize % 2 == 1 ? kernelSize : kernelSize + 1;
    }

    public ImageData Execute(ImageData image, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        progress?.Report(0.0);
        cancellationToken.ThrowIfCancellationRequested();

        ushort[] result = new ushort[image.PixelData.Count];
        int radius = _kernelSize / 2;
        int kernelArea = _kernelSize * _kernelSize;
        ushort[] neighbors = new ushort[kernelArea];
        int totalPixels = image.Width * image.Height;
        int progressInterval = Math.Max(1, totalPixels / 100); // Report every 1%

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                int pixelIndex = y * image.Width + x;
                int count = 0;

                for (int ky = -radius; ky <= radius; ky++)
                {
                    for (int kx = -radius; kx <= radius; kx++)
                    {
                        int nx = Math.Clamp(x + kx, 0, image.Width - 1);
                        int ny = Math.Clamp(y + ky, 0, image.Height - 1);

                        neighbors[count++] = image.PixelData[ny * image.Width + nx];
                    }
                }

                result[pixelIndex] = QuickSelect(neighbors, 0, count - 1, count / 2);

                if (pixelIndex % progressInterval == 0)
                {
                    progress?.Report((double)pixelIndex / totalPixels);
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

            // Swap arr[i] and arr[j]
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
    }
}
