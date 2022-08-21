using System.Text.Json;
using System.Text.Json.Serialization;
using ImgGoCli.Utils;

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
      "AccessKey": "",
      "AccessKeySecret": "",
      "BucketName": ""
    }, 
    "Qiniu":{
      "Zone":"z2",
      "UseHttps":false,
      "UseCdnDomains":false,
      "Bucket":"",
      "AccessKey":"",
      "SecretKey":"",
      "AccessUrl":""
    },
    "Tencent":{ 
      "Region":"ap-nanjing",
      "AppId":"",
      "SecretId":"",
      "SecretKey":"",
      "Bucket":""
    }
  },
  "ImageConfigs": {
    "SkipNotSupportFormat": true,
    "WatermarkText": "InCerry",
    "WatermarkFontSize": 30,
    "WatermarkFont": "Microsoft Yahei",
    "WatermarkFontColor": "#FFF",
    "CompressionLevel": "Low",
    "ConvertFormatTo": null
  }
}
""";

    public bool AddWatermark { get; set; } = false;
    public bool CompressionImage { get; set; } = false;
    public string? DefaultBlobStore { get; set; }
    
    public string? DefaultOutputPath { get; set; }
    public BlobStoreConfigs BlobStores { get; set; } = new();
    public ImageConfigs ImageConfigs { get; set; } = new();

    public void BasicConfigValidation()
    {
        if (DefaultBlobStore is null)
        {
            throw new ArgumentException("请使用-s命令或者修改配置文件[DefaultBlobStore]指定默认图床");
        }
    }

    internal static async ValueTask<AppConfigs> LoadConfigsAsync(FileInfo configsFile)
    {
        var jsonOptions = new JsonSerializerOptions {ReadCommentHandling = JsonCommentHandling.Skip};
        await using var json = configsFile.OpenRead();
        var configs =
            await JsonSerializer.DeserializeAsync<AppConfigs>(json, new ConfigsJsonContext(jsonOptions).AppConfigs);
        if (configs == null) throw new Exception("配置文件错误");
        LogUtil.Info($"加载配置文件成功，位置：{configsFile.FullName}");
        return configs;
    }
}


[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    // We only need metadata mode because we only do deserialization.
    GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(AppConfigs))]
internal partial class ConfigsJsonContext : JsonSerializerContext
{
}