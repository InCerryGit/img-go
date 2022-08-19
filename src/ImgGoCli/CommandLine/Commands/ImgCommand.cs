﻿using System.CommandLine;
using ImgGoCli.BlobStores;
using ImgGoCli.Configs;
using ImgGoCli.Utils;

namespace ImgGoCli.CommandLine.Commands;

public class ImgCommand : Command
{
    public ImgCommand() : base("img", "处理单个图片文件或图片目录")
    {
        var storeOption = CommandOptions.StoreOption();
        var configFileOption = CommandOptions.AppConfigPathOption();
        var outputPathOption = CommandOptions.OutPutPathOption();
        var watermarkOption = CommandOptions.WatermarkOption();
        var compressLevelOption = CommandOptions.CompressLevelOption();
        var filePathArgument = CommandArguments.FileSystemInfoArgument();
        AddOption(storeOption);
        AddOption(configFileOption);
        AddOption(outputPathOption);
        AddOption(watermarkOption);
        AddOption(compressLevelOption);
        AddArgument(filePathArgument);
        this.SetHandler(CommandHandler, storeOption, configFileOption, filePathArgument, outputPathOption,
            watermarkOption, compressLevelOption);
    }

    internal static async Task CommandHandler(BlobStoresEnum? store,
        FileInfo configsFile,
        FileSystemInfo imageDirectoryOrFile,
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

        switch (imageDirectoryOrFile)
        {
            case FileInfo file:
                await ImageFileHandler(file, config, blobStoresAccessor);
                break;
            case DirectoryInfo directoryInfo:
            {
                foreach (var fileInfo in directoryInfo.GetFiles())
                {
                    await ImageFileHandler(fileInfo, config, blobStoresAccessor);
                }

                break;
            }
        }
    }

    internal static async Task<string> ImageFileHandler(
        FileInfo imageFile,
        AppConfigs config,
        BlobStoresAccessor blobStoresAccessor)
    {
        ProcessImageResult? result = null;
        try
        {
            Console.WriteLine($"\n图片开始处理：[{Path.GetFileName(imageFile.FullName)}]");
            var (skip, imgStream, fileName) = result = ImgUtil.ProcessImage(
                config.AddWatermark,
                config.CompressionImage,
                config.ImageConfigs,
                imageFile);
            
            if (skip)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("由于文件类型不支持水印、压缩处理，已跳过");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("图片水印、压缩、转换处理成功");
            }

            var accessUrl = await blobStoresAccessor.DoStoreAsync(config.DefaultBlobStore!, imgStream, fileName);
            
            Console.WriteLine($"图片存储成功，存储路径：{accessUrl}");
            Console.WriteLine($"大小：{imageFile.Length / 1024.0:F}Kb -> {imgStream.Length / 1024.0:F}Kb");
            return accessUrl;
        }
        finally
        {
            if (result?.Stream is not null)
            {
                await result.Stream.DisposeAsync();
            }
        }
    }
}