using System.CommandLine;
using System.Web;
using ImgGoCli.BlobStores;
using ImgGoCli.Configs;
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
        var markdownFilePathArgument = CommandArguments.FilePathArgument();
        AddOption(storeOption);
        AddOption(configFileOption);
        AddOption(outputPathOption);
        AddOption(watermarkOption);
        AddOption(compressLevel);
        AddArgument(markdownFilePathArgument);
        this.SetHandler(CommandHandler, storeOption, markdownFilePathArgument, configFileOption, outputPathOption,
            watermarkOption, compressLevel);
    }


    internal static async Task CommandHandler(BlobStoresEnum? store,
        FileInfo markdownFile,
        FileInfo configsFile,
        string? outputPath,
        bool? addWatermark,
        bool? compressionImg)
    {
        var config = await AppConfigs.LoadConfigsAsync(configsFile);
        config.DefaultBlobStore = store?.ToString() ?? config.DefaultBlobStore;
        if (config.DefaultBlobStore is null)
        {
            throw new ArgumentException("请使用-s命令或者修改配置文件[DefaultBlobStore]指定默认图床");
        }

        config.AddWatermark = addWatermark ?? config.AddWatermark;
        config.CompressionImage = compressionImg ?? config.CompressionImage;

        var blobStoresAccessor = new BlobStoresAccessor(config);
        var fileDir = markdownFile.DirectoryName;
        var fileContent = await new StreamReader(markdownFile.OpenRead()).ReadToEndAsync();
        var imgPathDic = new Dictionary<string, string>();
        foreach (var img in RegexUtil.ExtractorImgFromMarkdown(fileContent))
        {
            if (img.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Web图片跳过：{img} ");
                continue;
            }

            if (imgPathDic.ContainsKey(img))
            {
                Console.WriteLine($"已上传图片跳过：{img} ");
                continue;
            }

            var imgPhyPath = HttpUtility.UrlDecode(Path.Combine(fileDir!, img));
            if (File.Exists(imgPhyPath) == false)
            {
                throw new FileNotFoundException($"请检查Markdown图片路径是否正确，文件不存在：{imgPhyPath}");
            }
            var imgFile = new FileInfo(imgPhyPath);
            imgPathDic[img] = await ImgCommand.ImageFileHandler(imgFile, config, blobStoresAccessor);
        }

        //替换
        fileContent = imgPathDic.Keys.Aggregate(
            fileContent, (current, key) => current.Replace(key, imgPathDic[key]));

        outputPath ??= $"{markdownFile.FullName[..markdownFile.FullName.LastIndexOf('.')]}-" +
                       $"{config.DefaultBlobStore}{Path.GetExtension(markdownFile.FullName)}";
        await File.WriteAllTextAsync(outputPath, fileContent);

        Console.WriteLine($"Markdown文件处理完成，文件保存在：{outputPath}");
    }
}