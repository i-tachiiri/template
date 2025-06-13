using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlobStorage(this IServiceCollection services, IConfiguration config)
    {
        var conn = config["Storage:ConnectionString"];
        services.AddSingleton(new BlobServiceClient(conn));
        services.AddSingleton<IBlobStorageService, AzureBlobStorageService>();
        return services;
    }
}
