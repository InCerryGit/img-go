using ImgGoCli.Configs;
using ImgGoCli.Extensions;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Compression.Zlib;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace ImgGoCli.Shared;

public class ImageProcessor : IDisposable
{
    private class ImageRawData
    {
        public ImageRawData(Image image, IImageFormat format)
        {
            Image = image;
            Format = format;
        }

        public Image Image { get; set; }
        public IImageFormat Format { get; set; }
    }


    private readonly Lazy<ImageRawData> _imageLazy;
    private readonly Stream _rawStream;
    private Image? _newImage;
    private string? _newFormatExtension;


    public ImageProcessor(Stream imgStream)
    {
        _rawStream = imgStream;
        _imageLazy = new Lazy<ImageRawData>(() =>
        {
            var image = Image.Load(_rawStream, out var format);
            return new ImageRawData(image, format);
        });
    }

    public ImageProcessor AddWatermark(Font font, string text, Color color, float padding = 5)
    {
        var oldImage = _newImage ?? _imageLazy.Value.Image;
        _newImage = oldImage.Clone(ctx => ctx.ApplyScalingWaterMarkSimple(font, text, color, padding));
        oldImage.Dispose();
        return this;
    }

    public ImageProcessor ConvertFormat(string format)
    {
        _newFormatExtension = format;
        return this;
    }

    public Stream GetCompressedResult(CompressionLevel level)
    {
        var stream = new MemoryStream();
        var image = _newImage ?? _imageLazy.Value.Image;
        var format = _newFormatExtension is null
            ? _imageLazy.Value.Format
            : _newImage.GetConfiguration().ImageFormatsManager.FindFormatByFileExtension(_newFormatExtension);
        var encoder = GetEncoder(format, level);
        if (encoder is null)
        {
            image.Save(stream, _imageLazy.Value.Format);
        }
        else
        {
            image.Save(stream, encoder);
        }

        return stream;
    }

    public Stream GetResult()
    {
        if (_newImage == null && _newFormatExtension is null)
        {
            return _rawStream;
        }
        
        var stream = new MemoryStream();
        _newImage.Save(stream,
            _newFormatExtension is null
                ? _imageLazy.Value.Format
                : _newImage.GetConfiguration().ImageFormatsManager.FindFormatByFileExtension(_newFormatExtension));

        return stream;
    }


    private static IImageEncoder? GetEncoder(IImageFormat format, CompressionLevel level)
    {
        return format.Name switch
        {
            // JPEG
            "JPEG" when level == CompressionLevel.Low => new JpegEncoder {Quality = 55},
            "JPEG" when level == CompressionLevel.Medium => new JpegEncoder {Quality = 35},
            "JPEG" when level == CompressionLevel.High => new JpegEncoder {Quality = 15},

            // PNG
            "PNG" when level == CompressionLevel.Low => new PngEncoder
            {
                CompressionLevel = PngCompressionLevel.Level7
            },
            "PNG" when level == CompressionLevel.Medium => new PngEncoder
            {
                CompressionLevel = PngCompressionLevel.Level8
            },
            "PNG" when level == CompressionLevel.High => new PngEncoder
            {
                CompressionLevel = PngCompressionLevel.Level9
            },

            // Webp
            "Webp" when level == CompressionLevel.Low => new WebpEncoder {Quality = 55},
            "Webp" when level == CompressionLevel.Medium => new WebpEncoder {Quality = 35},
            "Webp" when level == CompressionLevel.High => new WebpEncoder {Quality = 15},

            // TIFF
            "TIFF" when level == CompressionLevel.Low => new TiffEncoder
            {
                CompressionLevel = DeflateCompressionLevel.Level7
            },
            "TIFF" when level == CompressionLevel.Medium => new TiffEncoder
            {
                CompressionLevel = DeflateCompressionLevel.Level8
            },
            "TIFF" when level == CompressionLevel.High => new TiffEncoder
            {
                CompressionLevel = DeflateCompressionLevel.Level9
            },
            _ => null
        };
    }

    public void Dispose()
    {
        _imageLazy.Value.Image.Dispose();
        _newImage?.Dispose();
    }
}