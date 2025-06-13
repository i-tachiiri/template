using Shared;

namespace Shared.Tests;

public class DateTimeProviderTests
{
    [Fact]
    public void UtcNowCloseToSystemTime()
    {
        IDateTimeProvider provider = new SystemDateTimeProvider();
        var now = DateTime.UtcNow;
        var diff = (provider.UtcNow - now).TotalSeconds;
        Assert.True(Math.Abs(diff) < 1);
    }
}
