using System.IdentityModel.Tokens.Jwt;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Net;
using Microsoft.Azure.Functions.Worker.Http;
using Api;

public sealed class JwtValidationMiddleware : IFunctionsWorkerMiddleware
{
    private readonly JwtSecurityTokenHandler _handler = new();
    private readonly ConfigurationManager<OpenIdConnectConfiguration> _configManager;
    private readonly AzureAdB2COptions _options;

    public JwtValidationMiddleware(IOptions<AzureAdB2COptions> options)
    {
        _options = options.Value;
        var metadataAddress = $"https://{_options.Domain}/{_options.Tenant}/v2.0/.well-known/openid-configuration?p={_options.SignUpSignInPolicyId}";
        _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(metadataAddress, new OpenIdConnectConfigurationRetriever());
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var req = await context.GetHttpRequestDataAsync();
        if (req is null)
        {
            await next(context);
            return;
        }

        if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
        {
            var res = req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
            res.WriteString("Missing Authorization header");
            context.GetInvocationResult().Value = res;
            return;
        }

        var token = authHeaders.FirstOrDefault()?.Split(' ').LastOrDefault();
        if (string.IsNullOrEmpty(token))
        {
            var res = req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
            res.WriteString("Invalid token");
            context.GetInvocationResult().Value = res;
            return;
        }

        var config = await _configManager.GetConfigurationAsync(CancellationToken.None);
        var validationParameters = new TokenValidationParameters
        {
            ValidAudience = _options.ClientId,
            ValidIssuer = $"https://{_options.Domain}/{_options.Tenant}/v2.0/",
            IssuerSigningKeys = config.SigningKeys,
            ValidateLifetime = true,
            ValidateAudience = true,
            ValidateIssuer = true
        };

        try
        {
            var principal = _handler.ValidateToken(token, validationParameters, out _);
            context.Items["User"] = principal;
            await next(context);
        }
        catch (Exception)
        {
            var res = req.CreateResponse(System.Net.HttpStatusCode.Unauthorized);
            res.WriteString("Unauthorized");
            context.GetInvocationResult().Value = res;
        }
    }
}
