using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace Infrastructure;

public sealed class AzureBlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _client;

    public AzureBlobStorageService(BlobServiceClient client)
    {
        _client = client;
    }

    public async Task UploadAsync(string container, string blobName, Stream content, CancellationToken ct = default)
    {
        var containerClient = _client.GetBlobContainerClient(container);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);
        await containerClient.UploadBlobAsync(blobName, content, ct);
    }

    public Task<Uri> GetReadUriAsync(string container, string blobName, TimeSpan ttl, CancellationToken ct = default)
    {
        var containerClient = _client.GetBlobContainerClient(container);
        var blobClient = containerClient.GetBlobClient(blobName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = container,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.Add(ttl)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var uri = blobClient.GenerateSasUri(sasBuilder);
        return Task.FromResult(uri);
    }
}
