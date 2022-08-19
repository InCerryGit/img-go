namespace ImgGoCli.BlobStores;

public interface IBlobStore
{
    ValueTask<string> StoreAsync(Stream stream, string fileName);
}