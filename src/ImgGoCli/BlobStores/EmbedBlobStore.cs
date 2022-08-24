using ImgGoCli.Utils;

namespace ImgGoCli.BlobStores;

public class EmbedBlobStore : IBlobStore
{
    public ValueTask<string> StoreAsync(Stream stream, string fileName)
    {
        var bytes = new byte[stream.Length];
        _ = stream.Read(bytes);
        var base64Str = Convert.ToBase64String(bytes);
        return ValueTask.FromResult($"data:{MimeTypeUtil.GetMimeType(Path.GetExtension(fileName))};base64,{base64Str}");
    }
}