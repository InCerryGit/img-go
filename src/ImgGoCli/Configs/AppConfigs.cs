using System.Text.Json;
using System.Text.Json.Serialization;
using ImgGoCli.Utils;
using SixLabors.Fonts;
using SixLabors.ImageSharp;

namespace ImgGoCli.Configs;

public class AppConfigs
{
    public const string RawJson = """
// 如何配置请查看下方仓库文档：
// Github：https://github.com/InCerryGit/img-go
// Gitee(国内)：https://gitee.com/InCerryGit/img-go
{
  "AddWatermark": true,
  "CompressionImage": true,
  "DefaultBlobStore": "Local",
  "DefaultOutputPath": ".\\output",
  "BlobStores": { 
    "Local": {
        "SubPath":".\\assets"
    },
    "AliyunOss": {
      "Endpoint": "https://oss-cn-hangzhou.aliyuncs.com",
      "AccessKey": "AccessKey",
      "AccessKeySecret": "AccessKeySecret",
      "BucketName": "BucketName"
    }, 
    "Qiniu":{
      "Zone":"z2",
      "UseHttps":false,
      "UseCdnDomains":false,
      "Bucket":"Bucket",
      "AccessKey":"AccessKey",
      "SecretKey":"SecretKey",
      "AccessUrl":"AccessUrl"
    }
  },
  "ImageConfigs": {
    "SkipNotSupportFormat": true,
    "WatermarkText": "InCerry",
    "WatermarkFontSize": 30,
    "WatermarkFont": "Microsoft Yahei",
    "WatermarkFontColor": "#FFF",
    "CompressionLevel": "Low",
    "ConvertFormatTo": "jpg"
  }
}
""";

    public bool AddWatermark { get; set; } = false;
    public bool CompressionImage { get; set; } = false;
    public string? DefaultBlobStore { get; set; }
    
    public string? DefaultOutputPath { get; set; }
    public BlobStoreConfigs BlobStores { get; set; } = new();
    public ImageConfigs ImageConfigs { get; set; } = new();

    public void Validation()
    {
        BlobStores.Validation();
    }

    internal static async ValueTask<AppConfigs> LoadConfigsAsync(FileInfo configsFile)
    {
        var jsonOptions = new JsonSerializerOptions {ReadCommentHandling = JsonCommentHandling.Skip};
        await using var json = configsFile.OpenRead();
        var configs =
            await JsonSerializer.DeserializeAsync<AppConfigs>(json, new ConfigsJsonContext(jsonOptions).AppConfigs);
        if (configs == null) throw new Exception("配置文件错误");
        configs.Validation();
        LogUtil.Info($"加载配置文件成功，位置：{configsFile.FullName}");
        return configs;
    }
}

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

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    // We only need metadata mode because we only do deserialization.
    GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(AppConfigs))]
internal partial class ConfigsJsonContext : JsonSerializerContext
{
}