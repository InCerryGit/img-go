using System.CommandLine;
using System.Web;
using ImgGoCli.BlobStores;
using ImgGoCli.Configs;
using ImgGoCli.Extensions;
using ImgGoCli.Utils;

namespace ImgGoCli.CommandLine.Commands;

internal class MarkdownCommand : Command
{
    public MarkdownCommand() : base("md", "处理Markdown文件内所有图片")
    {
        var storeOption = CommandOptions.StoreOption();
        var configFileOption = CommandOptions.AppConfigPathOption();
        var outputPathOption = CommandOptions.OutPutPathOption();
        var watermarkOption = CommandOptions.WatermarkOption();
        var compressLevel = CommandOptions.CompressLevelOption();
        var skipProcessFailFile = CommandOptions.SkipFileWhenException();
        var markdownFilePathArgument = CommandArguments.FilePathArgument();
        AddOption(storeOption);
        AddOption(configFileOption);
        AddOption(outputPathOption);
        AddOption(watermarkOption);
        AddOption(compressLevel);
        AddOption(skipProcessFailFile);
        AddArgument(markdownFilePathArgument);
        this.SetHandler(CommandHandler, storeOption, markdownFilePathArgument, configFileOption, outputPathOption,
            watermarkOption, compressLevel, skipProcessFailFile);
    }


    internal static async Task CommandHandler(BlobStoresEnum? store,
        FileInfo markdownFile,
        FileInfo configsFile,
        string? outputPath,
        bool? addWatermark,
        bool? compressionImg,
        bool? skipProcessFailFile)
    {
        var config = await AppConfigs.LoadConfigsAsync(configsFile);
        config.DefaultBlobStore = store?.ToString() ?? config.DefaultBlobStore;
        config.DefaultOutputPath = outputPath ?? config.DefaultOutputPath;
        config.AddWatermark = addWatermark ?? config.AddWatermark;
        config.CompressionImage = compressionImg ?? config.CompressionImage;
        config.SkipFileWhenException = skipProcessFailFile ?? config.SkipFileWhenException;
        config.BasicConfigValidation();
        
        LogUtil.Notify(config.ToString());

        var blobStoresAccessor = new BlobStoresAccessor(config);
        var fileDir = markdownFile.DirectoryName;
        var fileContent = await new StreamReader(markdownFile.OpenRead()).ReadToEndAsync();
        var imgPathDic = new Dictionary<string, string>();
        foreach (var img in RegexUtil.ExtractorImgFromMarkdown(fileContent))
        {
            if (img.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                LogUtil.Info($"Web图片跳过：{img} ");
                continue;
            }

            if (imgPathDic.ContainsKey(img))
            {
                LogUtil.Info($"已上传图片跳过：{img} ");
                continue;
            }

            try
            {
                var imgPhyPath = HttpUtility.UrlDecode(Path.Combine(fileDir!, img));
                if (File.Exists(imgPhyPath) == false)
                {
                    throw new FileNotFoundException($"请检查Markdown图片路径是否正确，文件不存在：{imgPhyPath}");
                }

                var imgFile = new FileInfo(imgPhyPath);
                imgPathDic[img] = await ImgCommand.ImageFileHandler(imgFile, config, blobStoresAccessor);
            }
            catch (Exception ex) when(config.SkipFileWhenException)
            {
                LogUtil.Error($"跳过图片[{img}]，异常原因：处理失败-{ex.Message}");
            }
        }

        //替换
        fileContent = imgPathDic.Keys.Aggregate(
            fileContent, (current, key) => current.Replace(key, imgPathDic[key]));

        string fileFullPath;
        if (config.DefaultOutputPath.IsNullOrWhiteSpace())
        {
            fileFullPath = $"{markdownFile.FullName[..markdownFile.FullName.LastIndexOf('.')]}-" +
                           $"{config.DefaultBlobStore}{Path.GetExtension(markdownFile.FullName)}";
        }
        else
        {
            var fileName = $"{Path.GetFileNameWithoutExtension(markdownFile.Name)}-{config.DefaultBlobStore}" +
                           $"{Path.GetExtension(markdownFile.Name)}";

            var fileDirectory = Directory.Exists(config.DefaultOutputPath) == false
                ? Directory.CreateDirectory(config.DefaultOutputPath).FullName
                : new DirectoryInfo(config.DefaultOutputPath).FullName;

            fileFullPath = Path.Combine(fileDirectory, fileName);
        }

        await File.WriteAllTextAsync(fileFullPath, fileContent);

        LogUtil.Info($"Markdown文件处理完成，文件保存在：{fileFullPath}");
    }
}