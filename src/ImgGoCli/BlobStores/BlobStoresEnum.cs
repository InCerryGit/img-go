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
    Qiniu = 3,
    
    /// <summary>
    /// 腾讯云 Cos
    /// </summary>
    Tencent = 4,
    
    /// <summary>
    /// 使用Base64将图片嵌入文档
    /// </summary>
    Embed = 5 
}