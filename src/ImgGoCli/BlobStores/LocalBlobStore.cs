using ImgGoCli.Configs;

namespace ImgGoCli.BlobStores;

public class LocalBlobStore : IBlobStore
{
    private readonly LocalConfig _config;

    public LocalBlobStore(BlobStoreConfigs? blobStores)
    {
        ArgumentNullException.ThrowIfNull(blobStores?.Local);
        _config = blobStores.Local;
    }

    public async ValueTask<string> StoreAsync(Stream stream, string fileName)
    {
        if (Directory.Exists(_config.DirectoryPath) == false)
        {
            Directory.CreateDirectory(_config.DirectoryPath);
        }

        var path = Path.Combine(_config.DirectoryPath, fileName);
        await using var fileStream = File.OpenWrite(path);
        await stream.CopyToAsync(fileStream);
        return path;
    }
}