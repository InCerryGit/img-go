using ImgGoCli.Configs;
using ImgGoCli.Utils;

namespace ImgGoCli.BlobStores;

public class BlobStoresAccessor
{
    private readonly Dictionary<string, Lazy<IBlobStore>> _uploaderDic;

    public BlobStoresAccessor(AppConfigs appConfigs)
    {
        ArgumentNullException.ThrowIfNull(appConfigs);
        _uploaderDic = new Dictionary<string, Lazy<IBlobStore>>
        {
            [BlobStoresEnum.Local.ToString()] = new(() => new LocalBlobStore(appConfigs)),
            [BlobStoresEnum.AliyunOss.ToString()] = new(() => new AliyunOssBlobStore(appConfigs)),
            [BlobStoresEnum.Qiniu.ToString()] = new(() => new QiniuBlobStore(appConfigs))
        };
    }

    public async ValueTask<string> DoStoreAsync(string blobStore, Stream fileStream, string fileName)
    {
        if (_uploaderDic.TryGetValue(blobStore, out var store) == false)
        {
            throw new ArgumentException($"目标对象存储错误，没有找到[{blobStore}]");
        }

        fileStream.Seek(0, SeekOrigin.Begin);

        var i = 0;
        while (true)
        {
            try
            {
                return await store.Value.StoreAsync(fileStream, fileName);
            }
            catch (Exception ex) when (i < 3)
            {
                LogUtil.Info($"图片存储失败，正在重试{i++ + 1}次，异常原因：{ex.Message}");
            }
        }
    }
}