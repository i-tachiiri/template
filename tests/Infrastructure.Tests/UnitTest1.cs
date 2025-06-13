using Infrastructure;
using Azure.Storage.Blobs;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Tests;

public class UnitTest1
{
    [Fact]
    public async Task SasUriContainsBlobName()
    {
        var client = new BlobServiceClient("UseDevelopmentStorage=true");
        var service = new AzureBlobStorageService(client);
        var uri = await service.GetReadUriAsync("assets", "test.txt", TimeSpan.FromMinutes(5));
        Assert.Contains("test.txt", uri.ToString());
    }
}
