namespace Shared;

public static class RetryHelper
{
    public static async Task<T> RetryAsync<T>(Func<int, Task<T>> action, int maxAttempts = 3, TimeSpan? delay = null)
    {
        delay ??= TimeSpan.FromMilliseconds(200);
        for (int attempt = 1; ; attempt++)
        {
            try
            {
                return await action(attempt);
            }
            catch when (attempt < maxAttempts)
            {
                await Task.Delay(delay.Value);
            }
        }
    }
}
