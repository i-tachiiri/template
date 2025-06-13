using Shared;
using System.Diagnostics;

namespace Shared.Tests;

public class TelemetryTests
{
    [Fact]
    public async Task RunActivityAsyncCreatesActivity()
    {
        var result = await Telemetry.RunActivityAsync("test", _ => Task.FromResult(42));
        Assert.Equal(42, result);
    }
}
