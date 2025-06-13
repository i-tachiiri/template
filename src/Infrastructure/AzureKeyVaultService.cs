using Azure.Security.KeyVault.Secrets;
using Azure;

namespace Infrastructure;

public sealed class AzureKeyVaultService : IKeyVaultService
{
    private readonly SecretClient _client;

    public AzureKeyVaultService(SecretClient client)
    {
        _client = client;
    }

    public async Task<string?> GetSecretAsync(string name, CancellationToken ct = default)
    {
        try
        {
            KeyVaultSecret secret = await _client.GetSecretAsync(name, cancellationToken: ct);
            return secret.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public Task SetSecretAsync(string name, string value, CancellationToken ct = default)
    {
        return _client.SetSecretAsync(name, value, cancellationToken: ct);
    }
}
