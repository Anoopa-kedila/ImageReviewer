using ImageReviewerApp.Contracts;
using ImageReviewerApp.Models;

namespace ImageReviewerApp.Operations;

public class ThresholdingOperation : IImageOperation
{
    private readonly ushort _threshold;

    public string Name => "Thresholding";

    public ThresholdingOperation(ushort threshold = 32768)
    {
        _threshold = threshold;
    }

    public ImageData Execute(ImageData image, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        progress?.Report(0.0);
        cancellationToken.ThrowIfCancellationRequested();

        ushort[] result = new ushort[image.PixelData.Count];

        int progressInterval = image.PixelData.Count > 10 ? image.PixelData.Count / 10 : int.MaxValue;
        for (int i = 0; i < image.PixelData.Count; i++)
        {
            result[i] = image.PixelData[i] >= _threshold ? ushort.MaxValue : (ushort)0;

            if (progressInterval < int.MaxValue && i % progressInterval == 0)
            {
                progress?.Report((double)i / image.PixelData.Count);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        progress?.Report(1.0);
        return new ImageData(result, image.Width, image.Height, image.BitDepth);
    }
}
