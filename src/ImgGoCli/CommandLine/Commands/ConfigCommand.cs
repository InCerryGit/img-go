using System.CommandLine;
using System.Diagnostics;
using System.Text;
using ImgGoCli.Configs;
using ImgGoCli.Shared;
using ImgGoCli.Utils;

namespace ImgGoCli.CommandLine.Commands;

public class ConfigCommand : Command
{
    public ConfigCommand() : base("config", "配置文件的相关处理")
    {
        const string pad = "\n\t\t";
        var createConfigOption = new Option<string?>(
            aliases: new[] {"-c", "-create"},
            description: "创建配置文件"
        )
        {
            ArgumentHelpName = $"{pad}默认: 在用户目录创建配置文件{pad}" +
                               $"p: 程序根目录创建配置文件{pad} " +
                               $"c: 在当前命令行工作目录创建配置文件{pad}",
            Arity = ArgumentArity.ZeroOrOne
        };
        AddOption(createConfigOption);
        this.SetHandler((location) =>
        {
            string configPath;
            switch (location?.ToLower())
            {
                case "p":
                    configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.DefaultConfigFileName);
                    File.WriteAllText(configPath, AppConfigs.RawJson, Encoding.UTF8);
                    break;
                case "c":
                    configPath = Path.Combine(Directory.GetCurrentDirectory(), Constants.DefaultConfigFileName);
                    File.WriteAllText(configPath, AppConfigs.RawJson, Encoding.UTF8);
                    break;
                default:
                {
                    var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    var directoryPath = Path.Combine(docPath, Constants.AppName);
                    if (Directory.Exists(directoryPath) == false)
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    configPath = Path.Combine(directoryPath, Constants.DefaultConfigFileName);
                    File.WriteAllText(configPath, AppConfigs.RawJson, Encoding.UTF8);
                    break;
                }
            }

            LogUtil.Info($"默认配置文件创建成功，路径为：{configPath}");
        }, createConfigOption);
    }
}