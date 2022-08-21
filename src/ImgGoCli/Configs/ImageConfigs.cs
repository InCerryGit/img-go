using System.Text.Json.Serialization;
using SixLabors.Fonts;
using SixLabors.ImageSharp;

namespace ImgGoCli.Configs;

public class ImageConfigs
{
    public bool SkipNotSupportFormat { get; set; } = true;
    public string? WatermarkText { get; set; }

    public int WatermarkFontSize { get; set; } = 60;

    public string WatermarkFont { get; set; } = "Microsoft Yahei";

    public string WatermarkFontColor { get; set; } = "#FFF";

    public Color GetWatermarkFontColor() => Color.ParseHex(WatermarkFontColor);

    public string CompressionLevel { get; set; } = "Medium";

    public string? ConvertFormatTo { get; set; }


    public CompressionLevel GetCompressionLevel()
    {
        if (Enum.TryParse<CompressionLevel>(CompressionLevel, out var level) == false)
        {
            throw new ArgumentException("[CompressionLevel]配置错误");
        }

        return level;
    }

    [JsonIgnore] private object? _waterMarkFont;

    public Font GetFont() => (Font) (_waterMarkFont ??= SystemFonts.CreateFont(WatermarkFont, WatermarkFontSize));
}

public enum CompressionLevel
{
    /// <summary>
    /// 低圧缩比-图片大小稍小
    /// </summary>
    Low,

    /// <summary>
    /// 中压缩比-图片大小中等
    /// </summary>
    Medium,

    /// <summary>
    /// 高压缩比-图片大小最小
    /// </summary>
    High,
}