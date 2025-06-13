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

    [Fact]
    public async Task RunActivityAsyncVoidWorks()
    {
        int called = 0;
        await Telemetry.RunActivityAsync("void", _ => { called++; return Task.CompletedTask; });
        Assert.Equal(1, called);
    }

    [Fact]
    public void RunActivityWorks()
    {
        int called = 0;
        Telemetry.RunActivity("sync", _ => called++);
        Assert.Equal(1, called);
    }
}
