namespace ImgGoCli.Configs;

public abstract class BlobStoreConfig
{
    public abstract void Validation();
}

public class BlobStoreConfigs
{
    public AliyunOssConfig? AliyunOss { get; set; }

    public LocalConfig Local { get; set; } = new();

    public void Validation()
    {
        AliyunOss?.Validation();
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

public class LocalConfig : BlobStoreConfig
{
    public string DirectoryPath { get; set; } = @".\output";

    public override void Validation()
    {
        if (string.IsNullOrWhiteSpace(DirectoryPath))
            throw new Exception("Local:DirectoryPath不合法");
    }
}