using System.Net;
using Aliyun.OSS;
using ImgGoCli.Configs;

namespace ImgGoCli.BlobStores;

public class AliyunOssBlobStore : IBlobStore
{
    private readonly OssClient _ossClient;
    private readonly AliyunOssConfig _config;
    private readonly string _accessUrlPrefix;

    public AliyunOssBlobStore(AppConfigs? appConfigs)
    {
        ArgumentNullException.ThrowIfNull(appConfigs?.BlobStores.AliyunOss);
        _config = appConfigs.BlobStores.AliyunOss;
        _config.Validation();
        _accessUrlPrefix = _config.Endpoint.Replace("https://", $"https://{_config.BucketName}.");
        _ossClient = new OssClient(_config.Endpoint, _config.AccessKey, _config.AccessKeySecret);
    }

    public ValueTask<string> StoreAsync(Stream stream, string fileName)
    {
        var result = _ossClient.PutObject(_config.BucketName, fileName, stream);
        if (result.HttpStatusCode != HttpStatusCode.OK)
            throw new Exception("oss上传失败");
        return ValueTask.FromResult(GetAccessUrl(fileName));
    }

    private string GetAccessUrl(string fileName)
    {
        return $"{_accessUrlPrefix}/{fileName}";
    }
}