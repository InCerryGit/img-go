using COSXML;
using COSXML.Auth;
using COSXML.CosException;
using COSXML.Model.Object;
using ImgGoCli.Configs;
using ImgGoCli.Utils;

namespace ImgGoCli.BlobStores;

public class TencentBlobStore : IBlobStore
{
    private readonly TencentConfig _config;
    private readonly CosXmlServer _server;

    public TencentBlobStore(AppConfigs? configs)
    {
        _config = Guard.ArgumentNotNull(configs?.BlobStores.Tencent);
        var cred = new DefaultQCloudCredentialProvider(_config.SecretId, _config.SecretKey, 3600);
        var cosConfig = new CosXmlConfig.Builder()
            .SetAppid(_config.AppId)
            .SetRegion(_config.Region)
            .Build();
        _server = new CosXmlServer(cosConfig, cred);
    }

    public ValueTask<string> StoreAsync(Stream stream, string fileName)
    {
        var request = new PutObjectRequest(_config.Bucket, fileName, stream);
        try
        {
            var result = _server.PutObject(request);
            if (result.IsSuccessful() == false)
                throw new Exception($"存储文件失败:{result.httpMessage}");
        }
        catch (CosServerException ex)when (ex.statusCode == 404)
        {
            LogUtil.Error("上传返回404, 可能是Bucket名称不正确");
            throw;
        }

        var url = _server.GetObjectUrl(_config.Bucket, fileName);
        return ValueTask.FromResult(url);
    }
}