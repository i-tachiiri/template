using Shared;

namespace Shared.Tests;

public class RetryHelperTests
{
    [Fact]
    public async Task RetryAsyncRetriesUntilSuccess()
    {
        int attempts = 0;
        int result = await RetryHelper.RetryAsync<int>(async attempt =>
        {
            attempts++;
            if (attempts < 3)
                throw new InvalidOperationException();
            await Task.Delay(10);
            return 42;
        }, maxAttempts: 5);

        Assert.Equal(3, attempts);
        Assert.Equal(42, result);
    }
}
