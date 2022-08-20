using System.CommandLine;
using System.Text;
using ImgGoCli.Configs;
using ImgGoCli.Utils;

namespace ImgGoCli.CommandLine.Commands;

public class ConfigCommand : Command
{
    public ConfigCommand() : base("config", "配置文件的相关处理")
    {
        const string padLeft = "\n\t\t";
        var createConfigOption = new Option<string>(
            aliases: new[] {"-c", "-create"},
            description: "创建配置文件",
            getDefaultValue: () => "Current"
        )
        {
            ArgumentHelpName = $"{padLeft}User：在用户目录创建配置文件{padLeft}Current: 在当前目录创建配置文件{padLeft}",
            Arity = ArgumentArity.ExactlyOne
        };
        AddOption(createConfigOption);
        this.SetHandler((location) =>
        {
            string configPath;
            if (string.Equals(location, "User", StringComparison.InvariantCultureIgnoreCase))
            {
                var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var directoryPath = Path.Combine(docPath, Constants.AppName);
                if (Directory.Exists(directoryPath) == false)
                {
                    Directory.CreateDirectory(directoryPath);
                }

                configPath = Path.Combine(directoryPath, Constants.DefaultConfigFileName);
                File.WriteAllText(configPath, AppConfigs.RawJson, Encoding.UTF8);
            }
            else
            {
                configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.DefaultConfigFileName);
                File.WriteAllText(configPath, AppConfigs.RawJson, Encoding.UTF8);
            }

            LogUtil.Info($"默认配置文件创建成功，路径为：{configPath}");
        }, createConfigOption);
    }
}