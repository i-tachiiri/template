namespace Infrastructure;

public interface IKeyVaultService
{
    Task<string?> GetSecretAsync(string name, CancellationToken ct = default);
    Task SetSecretAsync(string name, string value, CancellationToken ct = default);
}
