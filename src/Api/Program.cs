using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Api;
using Infrastructure;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true)
              .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.Configure<AzureAdB2COptions>(context.Configuration.GetSection("AzureAdB2C"));
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(context.Configuration.GetSection("AzureAdB2C"));
        services.AddBlobStorage(context.Configuration);
        services.AddSqlDatabase(context.Configuration);
        services.AddKeyVault(context.Configuration);
    })
    .ConfigureFunctionsWorkerDefaults(worker =>
    {
        worker.UseMiddleware<JwtValidationMiddleware>();
    })
    .Build();

host.Run();
