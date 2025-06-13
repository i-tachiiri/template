using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

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

    public static IServiceCollection AddSqlDatabase(this IServiceCollection services, IConfiguration config)
    {
        var connStr = config.GetConnectionString("Default");
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connStr));
        services.AddScoped<ISqlUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        return services;
    }
}
