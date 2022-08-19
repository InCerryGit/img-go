using System.CommandLine;
using ImgGoCli.BlobStores;

namespace ImgGoCli.CommandLine;

internal static class CommandOptions
{
    internal static Option<BlobStoresEnum?> StoreOption()
    {
        const string padLeft = "\n\t\t";
        var option = new Option<BlobStoresEnum?>(
            aliases: new[] {"--store", "-s"},
            description: @"设置图床，默认使用配置文件值")
        {
            ArgumentHelpName = $"{padLeft}Local: 本地存储{padLeft}AliyunOss: 阿里云对象存储{padLeft}"
        };
        return option;
    }

    internal static Option<FileInfo> MarkdownFilePathOption()
    {
        return new Option<FileInfo>(
            aliases: new[] {"--path", "-p"},
            description: "需要处理的Markdown文件路径")
        {
            IsRequired = true
        }.ExistingOnly();
    }

    internal static Option<FileInfo> AppConfigPathOption()
    {
        return new Option<FileInfo>(
                aliases: new[] {"--config", "-c"},
                description: "自定义的配置文件路径",
                getDefaultValue: () =>
                {
                    if (File.Exists(Constants.DefaultConfigFileName))
                    {
                        return new FileInfo(Constants.DefaultConfigFileName);   
                    }
                    
                    var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var userConfigPath = Path.Combine(docPath, Constants.AppName, Constants.DefaultConfigFileName);
                    if (File.Exists(userConfigPath))
                    {
                        return new FileInfo(userConfigPath);
                    }

                    throw new InvalidCastException($"配置文件不存在，请使用{Constants.AppName} config -h命令创建并配置");
                })
            .ExistingOnly();
    }

    internal static Option<string?> OutPutPathOption()
    {
        return new Option<string?>(
                aliases: new[] {"--output", "-o"},
                description: "自定义的输出文件路径，默认为当前文件夹")
            .LegalFilePathsOnly();
    }

    internal static Option<bool?> WatermarkOption()
    {
        return new Option<bool?>(
            aliases: new[] {"--watermark", "-w"},
            description: "为图片添加水印，水印文字颜色和字体在配置文件[ImageConfigs]中定义"
        );
    }

    internal static Option<bool?> CompressLevelOption()
    {
        return new Option<bool?>(
            aliases: new[] {"--compress", "-cp"},
            description: "启用图片压缩，压缩级别在配置文件[ImageConfigs:CompressionLevel]中定义"
        );
    }
}