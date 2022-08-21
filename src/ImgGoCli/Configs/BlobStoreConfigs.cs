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

    public void Validation()
    {
        AliyunOss?.Validation();
        Qiniu?.Validation();
        Tencent?.Validation();
        Local.Validation();
    }
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
            throw new Exception("AliyunOss:AccessKey不合法");
        if (string.IsNullOrWhiteSpace(AccessKeySecret))
            throw new Exception("AliyunOss:AccessKeySecret不合法");
        if (string.IsNullOrWhiteSpace(BucketName))
            throw new Exception("AliyunOss:BucketName不合法");
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
            throw new Exception("Qiniu:Zone不合法");
        if (string.IsNullOrWhiteSpace(Bucket))
            throw new Exception("Qiniu:Bucket不合法");
        if (string.IsNullOrWhiteSpace(AccessKey))
            throw new Exception("Qiniu:AccessKey不合法");
        if (string.IsNullOrWhiteSpace(AccessUrl))
            throw new Exception("Qiniu:AccessUrl不合法");
        if (string.IsNullOrWhiteSpace(SecretKey))
            throw new Exception("Qiniu:SecretKey不合法");
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
            throw new Exception("Tencent:Region不合法");
        if (string.IsNullOrWhiteSpace(AppId))
            throw new Exception("Tencent:AppId不合法");
        if (string.IsNullOrWhiteSpace(SecretId))
            throw new Exception("Tencent:SecretId不合法");
        if (string.IsNullOrWhiteSpace(SecretKey))
            throw new Exception("Tencent:SecretKey不合法");
        if (string.IsNullOrWhiteSpace(Bucket))
            throw new Exception("Tencent:Bucket不合法");
    }
}

public class LocalConfig : BlobStoreConfig
{
    public string SubPath { get; set; } = null!;

    public override void Validation()
    {
        if (string.IsNullOrWhiteSpace(SubPath))
            throw new Exception("Local:SubPath不合法");
    }
}