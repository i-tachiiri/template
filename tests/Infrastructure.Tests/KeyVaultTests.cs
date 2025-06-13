using Infrastructure;

namespace Infrastructure.Tests;

public class KeyVaultTests
{
    private sealed class FakeKeyVaultService : IKeyVaultService
    {
        private readonly Dictionary<string, string> _store = new();
        public Task<string?> GetSecretAsync(string name, CancellationToken ct = default)
            => Task.FromResult(_store.TryGetValue(name, out var value) ? value : null);
        public Task SetSecretAsync(string name, string value, CancellationToken ct = default)
        {
            _store[name] = value;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task CanSetAndGetSecret()
    {
        IKeyVaultService svc = new FakeKeyVaultService();
        await svc.SetSecretAsync("foo", "bar");
        var val = await svc.GetSecretAsync("foo");
        Assert.Equal("bar", val);
    }
}
