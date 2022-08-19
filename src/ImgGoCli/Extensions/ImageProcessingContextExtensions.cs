using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace ImgGoCli.Extensions;

public static class ImageProcessingContextExtensions
{
    public static IImageProcessingContext ApplyScalingWaterMarkSimple(
        this IImageProcessingContext processingContext,
        Font font,
        string text,
        Color color,
        float padding)
    {
        var imgSize = processingContext.GetCurrentSize();

        // measure the text size
        var useFont = font;
        var fontSize = TextMeasurer.Measure(text, new TextOptions(font));
        if (fontSize.Width + padding > imgSize.Width || fontSize.Height + padding > imgSize.Height)
        {
            //find out how much we need to scale the text to fill the space (up or down)
            var scalingFactor = Math.Min(
                imgSize.Width / (fontSize.Width + padding),
                imgSize.Height / (fontSize.Height + padding));
            useFont = new Font(font, scalingFactor * font.Size);
            fontSize = TextMeasurer.Measure(text, new TextOptions(useFont));
        }


        var location = new PointF(
            x: imgSize.Width - fontSize.Width - padding,
            y: imgSize.Height - fontSize.Height - padding);

        var shadow = new PointF(location.X + 2, location.Y + 2);
        processingContext.DrawText(text, useFont, Color.Gray, shadow);
        return processingContext.DrawText(text, useFont, color, location);
    }
}