using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using ImgGoCli.CommandLine.Commands;

namespace ImgGoCli;

static class Program
{
    private const string Logo = @"
     _____ __  __  _____         _____  ____  
    |_   _|  \/  |/ ____|       / ____|/ __ \ 
      | | | \  / | |  __ ______| |  __| |  | |
      | | | |\/| | | |_ |______| | |_ | |  | |
     _| |_| |  | | |__| |      | |__| | |__| |
    |_____|_|  |_|\_____|       \_____|\____/
作者：InCerry 项目GitHub：https://github.com/InCerryGit/img-go";
    public static async Task<int> Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{Logo}\n");
        Console.ResetColor();
        
        var rootCommand = new RootCommand(
            "提取Markdown文件内的图片上传到图床，并且生成替换后的Markdown文件。目前支持本地存储、阿里云OSS（大佬们可以自行扩展）\n" +
            "图片水印支持：PNG、JPG、Webp、Gif、Tiff、BMP格式\n" +
            "图片压缩支持：PNG、JPG、Webp、Tiff格式\n")
        {
            new MarkdownCommand(),
            new ImgCommand(),
            new ConfigCommand()
        };

        var parser = new CommandLineBuilder(rootCommand)
            .UseHelp()
            .UseVersionOption("-v", "--version")
            .UseSuggestDirective()
            .UseParseErrorReporting()
            .UseExceptionHandler()
            .Build();

        if (args.Length == 0)
        {
            var helpBuilder = new HelpBuilder(LocalizationResources.Instance, Console.WindowWidth);
            helpBuilder.Write(rootCommand, Console.Out);
            return 0;
        }

        return await parser.InvokeAsync(args);
    }
}