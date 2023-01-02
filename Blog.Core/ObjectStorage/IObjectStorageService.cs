namespace Blog.Core.ObjectStorage;

public interface IObjectStorageService
{
    Task<PutInfo> PutInfo(string key, TimeSpan duration, Dictionary<int, string>? extraInfo = null);

    Task<GetInfo> GetInfo(string key, TimeSpan duration, Dictionary<int, string>? extraInfo = null);

    Task<ImageGetInfoDto> ImageGetInfo(string key, TimeSpan duration, Dictionary<int, string>? extraInfo = null);
}