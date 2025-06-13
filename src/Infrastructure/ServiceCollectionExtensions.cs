using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlDatabase(this IServiceCollection services, IConfiguration config)
    {
        var connStr = config.GetConnectionString("Default");
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connStr));
        services.AddScoped<ISqlUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        return services;
    }
}
