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
        _config.Validation();
    }

    public async ValueTask<string> StoreAsync(Stream stream, string fileName)
    {
        var directoryPath = Path.Combine(_appConfigs.DefaultOutputPath.IsNullOrWhiteSpace()
                ? Directory.GetCurrentDirectory()
                : _appConfigs.DefaultOutputPath,
            _config.SubPath);
        
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