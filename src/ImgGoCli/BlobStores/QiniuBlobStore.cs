using System.Web;
using ImgGoCli.Configs;
using Qiniu.Storage;
using Qiniu.Util;

namespace ImgGoCli.BlobStores;

public class QiniuBlobStore : IBlobStore
{
    private readonly string _token;
    private readonly FormUploader _target;
    private readonly string _accessUrlPrefix;

    private readonly Dictionary<string, Zone> _regionMap = new()
    {
        ["z0"] = Zone.ZONE_CN_East,
        ["z1"] = Zone.ZONE_CN_North,
        ["z2"] = Zone.ZONE_CN_South,
        ["as0"] = Zone.ZONE_AS_Singapore,
        ["na0"] = Zone.ZONE_US_North
    };

    public QiniuBlobStore(AppConfigs? appConfigs)
    {
        ArgumentNullException.ThrowIfNull(appConfigs?.BlobStores.Qiniu);
        var config = appConfigs.BlobStores.Qiniu;
        config.Validation();
        var mac = new Mac(config.AccessKey, config.SecretKey);
        var putPolicy = new PutPolicy
        {
            Scope = config.Bucket
        };
        putPolicy.SetExpires(3600);
        _token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
        if (_regionMap.TryGetValue(config.Zone, out var zone) == false)
        {
            zone = ZoneHelper.QueryZone(config.AccessKey, config.Bucket);
        }

        var upConfig = new Config
        {
            Zone = zone,
            UseHttps = config.UseHttps,
            UseCdnDomains = config.UseCdnDomains,
            ChunkSize = ChunkUnit.U512K
        };
        _target = new FormUploader(upConfig);
        _accessUrlPrefix = config.AccessUrl.TrimEnd('/');
    }

    public async ValueTask<string> StoreAsync(Stream stream, string fileName)
    {
        // the sdk will be close stream (I don't know why to do it), we must back up the stream.
        using var uploadStream = new MemoryStream((int) stream.Length);
        await stream.CopyToAsync(uploadStream);
        uploadStream.Seek(0, SeekOrigin.Begin);
        var result = _target.UploadStream(uploadStream, fileName, _token, new PutExtra
        {
            // ignore progress
            ProgressHandler = (_, _) => { }
        });
        if (result?.Code != 200)
            throw new Exception($"Code:{result?.Code},{result?.Text}");
        return GetAccessUrl(fileName);
    }

    private string GetAccessUrl(string fileName)
    {
        return $"{_accessUrlPrefix}/{HttpUtility.UrlEncode(fileName)}";
    }
}