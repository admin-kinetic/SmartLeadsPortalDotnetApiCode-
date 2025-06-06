using System;

namespace SmartLeadsPortalDotNetApi.Helper;

public class RetryHelper
{
    public static async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation,
        int maxRetries = 3,
        CancellationToken cancellationToken = default)
    {
        int retryCount = 0;
        var exceptions = new List<Exception>();

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                return await operation();
            }
            catch (HttpRequestException ex)
            {
                exceptions.Add(ex);
            }

            if (retryCount++ >= maxRetries)
            {
                throw new AggregateException(
                    $"Failed after {maxRetries} retries", exceptions);
            }

            // Exponential backoff (100ms, 200ms, 400ms, etc.)
            var delay = TimeSpan.FromMilliseconds(100 * Math.Pow(2, retryCount));
            await Task.Delay(delay, cancellationToken);
        }
    }
}
