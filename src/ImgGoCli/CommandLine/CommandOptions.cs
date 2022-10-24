using System.CommandLine;
using ImgGoCli.BlobStores;
using ImgGoCli.Shared;
using ImgGoCli.Utils;

namespace ImgGoCli.CommandLine;

internal static class CommandOptions
{
    internal static Option<BlobStoresEnum?> StoreOption()
    {
        const string pad = "\n\t\t";
        var option = new Option<BlobStoresEnum?>(
            aliases: new[] {"--store", "-s"},
            description: @"设置图床，默认使用配置文件值")
        {
            ArgumentHelpName = $"{pad}Local: 本地存储{pad}" +
                               $"Embed: 使用Base64方式将图片嵌入Markdown文件中{pad}" +
                               $"AliyunOss: 阿里云Oss存储{pad}" +
                               $"Qiniu: 七牛云kodo存储{pad}" +
                               $"Tencent: 腾讯云Cos存储{pad}"
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

                    LogUtil.Error($"配置文件不存在，请使用[{Constants.AppName} config -h]命令查看帮助、创建并配置");

                    throw new FileNotFoundException(Constants.DefaultConfigFileName);
                })
            .ExistingOnly();
    }

    internal static Option<string?> OutPutPathOption()
    {
        return new Option<string?>(
                aliases: new[] {"--output", "-o"},
                description: "自定义的输出文件路径，默认使用配置文件[DefaultOutputPath]属性值")
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
    
    internal static Option<bool?> SkipFileWhenException()
    {
        return new Option<bool?>(
            aliases: new[] {"--skip-exception-file", "-sf"},
            description: "是否跳过处理异常的文件，默认使用配置文件[SkipFileWhenException]属性值"
        )
        {
            ArgumentHelpName = "true：跳过该文件，继续处理其它文件 false：抛出异常，终止处理"
        };
    }
}