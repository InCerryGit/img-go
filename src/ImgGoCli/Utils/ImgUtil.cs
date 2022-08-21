using ImgGoCli.Configs;
using ImgGoCli.Extensions;
using ImgGoCli.Shared;

namespace ImgGoCli.Utils;

public static class ImgUtil
{
    public static ProcessImageResult ProcessImage(
        bool addWatermark,
        bool compressImg,
        ImageConfigs imageConfig,
        FileInfo imgFile)
    {
        var imgStream = imgFile.OpenRead();
        var fileName = Path.GetFileName(imgFile.FullName);
        if (addWatermark == false && compressImg == false && imageConfig.ConvertFormatTo.NotNullOrEmpty())
            return new ProcessImageResult(false, imgStream,  fileName);

        try
        {
            var processor = new ImageProcessor(imgStream);
            if (addWatermark)
                processor.AddWatermark(imageConfig.GetFont(),
                    imageConfig.WatermarkText ?? throw new ArgumentException("水印文字为空，请配置[ImageConfigs:WatermarkText]"),
                    imageConfig.GetWatermarkFontColor());
            
            if (imageConfig.ConvertFormatTo is not null)
            {
                processor.ConvertFormat(imageConfig.ConvertFormatTo);
                fileName = Path.ChangeExtension(fileName, imageConfig.ConvertFormatTo);
            }
            
            return new ProcessImageResult(
                false, 
                compressImg
                ? processor.GetCompressedResult(imageConfig.GetCompressionLevel())
                : processor.GetResult(),
                fileName);
        }
        catch (SixLabors.ImageSharp.UnknownImageFormatException) when (imageConfig.SkipNotSupportFormat)
        {
            return new ProcessImageResult(true, imgStream, fileName);
        }
    }
}

public record ProcessImageResult(bool SkipNotSupport, Stream Stream, string FileName);