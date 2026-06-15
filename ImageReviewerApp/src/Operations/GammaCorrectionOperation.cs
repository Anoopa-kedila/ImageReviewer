using ImageReviewerApp.Contracts;
using ImageReviewerApp.Models;

namespace ImageReviewerApp.Operations;

public class GammaCorrectionOperation : IImageOperation
{
    private readonly double _gamma;

    public string Name => "Gamma Correction";

    public GammaCorrectionOperation(double gamma = 2.2)
    {
        _gamma = gamma > 0 ? gamma : 2.2;
    }

    public ImageData Execute(ImageData image, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        progress?.Report(0.1);
        cancellationToken.ThrowIfCancellationRequested();

        ushort[] result = new ushort[image.PixelData.Count];

        ushort[] lut = new ushort[65536];
        for (int i = 0; i < 65536; i++)
        {
            double normalized = i / 65535.0;
            double corrected = Math.Pow(normalized, _gamma);
            lut[i] = (ushort)(corrected * 65535.0);
        }

        progress?.Report(0.3);
        cancellationToken.ThrowIfCancellationRequested();

        int progressInterval = image.PixelData.Count > 10 ? image.PixelData.Count / 10 : int.MaxValue;
        for (int i = 0; i < image.PixelData.Count; i++)
        {
            result[i] = lut[image.PixelData[i]];

            if (progressInterval < int.MaxValue && i % progressInterval == 0)
            {
                progress?.Report(0.3 + (double)i / image.PixelData.Count * 0.7);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        progress?.Report(1.0);
        return new ImageData(result, image.Width, image.Height, image.BitDepth);
    }
}
