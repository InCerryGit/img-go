namespace ImgGoCli.Configs;

public abstract class BlobStoreConfig
{
    public abstract void Validation();
}

public class BlobStoreConfigs
{
    public LocalConfig Local { get; set; } = new();

    public AliyunOssConfig? AliyunOss { get; set; }

    public QiniuConfig? Qiniu { get; set; }

    public TencentConfig? Tencent { get; set; }
}

public class AliyunOssConfig : BlobStoreConfig
{
    public string Endpoint { get; set; } = null!;
    public string AccessKey { get; set; } = null!;
    public string AccessKeySecret { get; set; } = null!;
    public string BucketName { get; set; } = null!;

    public override void Validation()
    {
        if (string.IsNullOrWhiteSpace(AccessKey))
            throw new Exception("配置文件BlobStores:AliyunOss:AccessKey属性不能为空");
        if (string.IsNullOrWhiteSpace(AccessKeySecret))
            throw new Exception("配置文件BlobStores:AliyunOss:AccessKeySecret属性不能为空");
        if (string.IsNullOrWhiteSpace(BucketName))
            throw new Exception("配置文件BlobStores:AliyunOss:BucketName属性不能为空");
    }
}

public class QiniuConfig : BlobStoreConfig
{
    public string Zone { get; set; } = null!;

    public bool UseHttps { get; set; } = false;

    public bool UseCdnDomains { get; set; } = false;

    public string Bucket { get; set; } = null!;

    public string AccessKey { get; set; } = null!;

    public string SecretKey { get; set; } = null!;

    public string AccessUrl { get; set; } = null!;

    public override void Validation()
    {
        if (string.IsNullOrWhiteSpace(Zone))
            throw new Exception("配置文件BlobStores:Qiniu:Zone属性不能为空");
        if (string.IsNullOrWhiteSpace(Bucket))
            throw new Exception("配置文件BlobStores:Qiniu:Bucket属性不能为空");
        if (string.IsNullOrWhiteSpace(AccessKey))
            throw new Exception("配置文件BlobStores:Qiniu:AccessKey属性不能为空");
        if (string.IsNullOrWhiteSpace(AccessUrl))
            throw new Exception("配置文件BlobStores:Qiniu:AccessUrl属性不能为空");
        if (string.IsNullOrWhiteSpace(SecretKey))
            throw new Exception("配置文件BlobStores:Qiniu:SecretKey属性不能为空");
    }
}

public class TencentConfig : BlobStoreConfig
{
    public string Region { get; set; } = null!;
    public string AppId { get; set; } = null!;
    public string SecretId { get; set; } = null!;

    public string SecretKey { get; set; } = null!;

    public string Bucket { get; set; } = null!;

    public override void Validation()
    {
        if (string.IsNullOrWhiteSpace(Region))
            throw new Exception("配置文件BlobStores:Tencent:Region属性不能为空");
        if (string.IsNullOrWhiteSpace(AppId))
            throw new Exception("配置文件BlobStores:Tencent:AppId属性不能为空");
        if (string.IsNullOrWhiteSpace(SecretId))
            throw new Exception("配置文件BlobStores:Tencent:SecretId属性不能为空");
        if (string.IsNullOrWhiteSpace(SecretKey))
            throw new Exception("配置文件BlobStores:Tencent:SecretKey属性不能为空");
        if (string.IsNullOrWhiteSpace(Bucket))
            throw new Exception("配置文件BlobStores:Tencent:Bucket属性不能为空");
    }
}

public class LocalConfig : BlobStoreConfig
{
    public string SubPath { get; set; } = null!;

    public override void Validation()
    {
        if (string.IsNullOrWhiteSpace(SubPath))
            throw new Exception("配置文件BlobStores:Local:SubPath属性不能为空");
    }
}