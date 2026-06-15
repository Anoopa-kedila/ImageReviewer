using ImageReviewerApp.Contracts;
using ImageReviewerApp.Models;
using System;

namespace ImageReviewerApp.Operations;

public class WindowLevelOperation : IImageOperation
{
    private readonly double _window;
    private readonly double _level;

    public string Name => "Window/Level";

    public WindowLevelOperation(double window = 65536, double level = 32768)
    {
        _window = window > 0 ? window : 65536;
        _level = level;
    }

    public ImageData Execute(ImageData image, IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        if (image == null)
            throw new ArgumentNullException(nameof(image));

        progress?.Report(0.1);
        cancellationToken.ThrowIfCancellationRequested();

        ushort[] result = new ushort[image.PixelData.Count];

        double minValue = _level - (_window / 2.0);
        double maxValue = _level + (_window / 2.0);
        double inverseWindow = 1.0 / _window;

        progress?.Report(0.3);
        cancellationToken.ThrowIfCancellationRequested();

        ushort[] lut = new ushort[65536];
        for (int val = 0; val < 65536; val++)
        {
            if (val <= minValue)
            {
                lut[val] = 0;
            }
            else if (val >= maxValue)
            {
                lut[val] = ushort.MaxValue;
            }
            else
            {
                double normalized = (val - minValue) * inverseWindow;
                lut[val] = (ushort)(normalized * ushort.MaxValue);
            }
        }

        progress?.Report(0.5);
        cancellationToken.ThrowIfCancellationRequested();

        int progressInterval = image.PixelData.Count > 10 ? image.PixelData.Count / 10 : int.MaxValue;
        for (int i = 0; i < image.PixelData.Count; i++)
        {
            result[i] = lut[image.PixelData[i]];

            if (progressInterval < int.MaxValue && i % progressInterval == 0)
            {
                progress?.Report(0.5 + (double)i / image.PixelData.Count * 0.5);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        progress?.Report(1.0);
        return new ImageData(result, image.Width, image.Height, image.BitDepth);
    }
}
