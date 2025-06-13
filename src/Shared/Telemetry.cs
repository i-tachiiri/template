using System.Diagnostics;
namespace Shared;

public static class Telemetry
{
    public const string ActivitySourceName = "ServiceStarterKit";
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public static Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
        => ActivitySource.StartActivity(name, kind);

    public static async Task<T> RunActivityAsync<T>(string name, Func<Activity?, Task<T>> func)
    {
        using var activity = StartActivity(name);
        return await func(activity);
    }
}
