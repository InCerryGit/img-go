using System.CommandLine;

namespace ImgGoCli.CommandLine;

internal static class CommandArguments
{
    internal static Argument<FileInfo> FilePathArgument()
    {
        return new Argument<FileInfo>(parse: result =>
        {
            var path = result.Tokens[0];
            return new FileInfo(path.Value);
        })
        {
            Description = "需要处理的文件路径",
            Arity = ArgumentArity.ExactlyOne,
        }.ExistingOnly();
    }

    internal static Argument<FileSystemInfo> FileSystemInfoArgument()
    {
        return new Argument<FileSystemInfo>(parse: result =>
        {
            var path = result.Tokens[0].Value;
            FileSystemInfo value;
            if (Directory.Exists(path))
            {
                value = new DirectoryInfo(path);
            }
            else if (path.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) ||
                     path.EndsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal))
            {
                value = new DirectoryInfo(path);
            }
            else
            {
                value = new FileInfo(path);
            }

            return value;
        })
        {
            Description = "需要处理的文件或目录路径",
            Arity = ArgumentArity.ExactlyOne,
        }.ExistingOnly();
    }
}