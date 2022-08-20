using ImgGoCli.Configs;
using ImgGoCli.Extensions;

namespace ImgGoCli.BlobStores;

public class LocalBlobStore : IBlobStore
{
    private readonly AppConfigs _appConfigs;
    private readonly LocalConfig _config;

    public LocalBlobStore(AppConfigs? appConfigs)
    {
        ArgumentNullException.ThrowIfNull(appConfigs?.BlobStores.Local);
        _appConfigs = appConfigs;
        _config = appConfigs.BlobStores.Local;
    }

    public async ValueTask<string> StoreAsync(Stream stream, string fileName)
    {
        if (_appConfigs.DefaultOutputPath.IsNullOrWhiteSpace())
            throw new Exception("输出路径为空，请使用-o指定路径或修改配置文件[DefaultOutputPath]属性");

        var directoryPath = Path.Combine(_appConfigs.DefaultOutputPath, _config.SubPath);
        if (Directory.Exists(directoryPath) == false)
        {
            Directory.CreateDirectory(directoryPath);
        }

        var filePath = Path.Combine(directoryPath, fileName);
        await using var fileStream = File.OpenWrite(filePath);
        await stream.CopyToAsync(fileStream);
        
        // return access path
        return Path.Combine(_config.SubPath, fileName);
    }
}