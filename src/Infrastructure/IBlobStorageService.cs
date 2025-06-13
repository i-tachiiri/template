namespace Infrastructure;

public interface IBlobStorageService
{
    Task UploadAsync(string container, string blobName, Stream content, CancellationToken ct = default);
    Task<Uri> GetReadUriAsync(string container, string blobName, TimeSpan ttl, CancellationToken ct = default);
}
