namespace ImgGoCli.BlobStores;

public enum BlobStoresEnum
{
    /// <summary>
    /// 本地存储
    /// </summary>
    Local = 1,

    /// <summary>
    /// 阿里云Oss
    /// </summary>
    AliyunOss = 2,
    
    /// <summary>
    /// 七牛云Kodo
    /// </summary>
    Qiniu = 3
}