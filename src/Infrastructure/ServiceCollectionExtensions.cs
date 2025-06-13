using Azure.Storage.Blobs;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Extensions.Hosting;
using Shared;

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

    public static IServiceCollection AddKeyVault(this IServiceCollection services, IConfiguration config)
    {
        var vaultUrl = config["KeyVault:Url"] ?? throw new InvalidOperationException("KeyVault:Url missing");
        services.AddSingleton(new SecretClient(new Uri(vaultUrl), new DefaultAzureCredential()));
        services.AddSingleton<IKeyVaultService, AzureKeyVaultService>();
        return services;
    }

    public static IServiceCollection AddObservability(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService("ServiceStarterKit.Api"))
            .WithTracing(b => b
                .AddSource(Telemetry.ActivitySourceName)
                .AddHttpClientInstrumentation()
                .AddAzureMonitorTraceExporter())
            .WithMetrics(b => b
                .AddRuntimeInstrumentation()
                .AddAzureMonitorMetricExporter());
        return services;
    }
}
