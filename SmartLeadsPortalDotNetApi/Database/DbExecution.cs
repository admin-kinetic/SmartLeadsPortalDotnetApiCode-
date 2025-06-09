using System.Data.Common;
using Microsoft.Data.SqlClient;
using SmartLeadsPortalDotNetApi.BackgroundTasks;

namespace SmartLeadsPortalDotNetApi.Database;

public class DbExecution
{
    private readonly WebhookBackgroundTaskQueue webhookBackgroundTaskQueue;
    private readonly ILogger<DbExecution> logger;

    public DbExecution(ILogger<DbExecution> logger, WebhookBackgroundTaskQueue webhookBackgroundTaskQueue)
    {
        this.webhookBackgroundTaskQueue = webhookBackgroundTaskQueue;
        this.logger = logger;
    }

    public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3)
    {
        int retryCount = 0;
        while (true)
        {
            try
            {
                return await operation();
            }
            catch (SqlException ex) when (ex.Number == 1205) // Deadlock
            {
                logger.LogWarning(ex, "Deadlock detected. Retrying...");
                if (retryCount++ >= maxRetries)
                    throw;

                var delay = TimeSpan.FromMilliseconds(Math.Pow(2, retryCount) * 100);
                logger.LogInformation("Retrying after {Delay} ms", delay.TotalMilliseconds);
                await Task.Delay(delay);
            }
            catch (SqlException ex) when (ex.Number == 10928 || ex.Number == -2) // request limit exceeded or timeout
            {
                logger.LogWarning(ex, "Request limit exceeded. Adding to background task queue for retry.");
                this.webhookBackgroundTaskQueue.QueueBackgroundWorkItem(async cancellationToken =>
                {
                    await operation();
                }, Guid.NewGuid());

                return default(T); // Return default value if operation is queued
            }
        }
    }

    public void ExecuteInsideBackgroundTaskAsync(Func<Task> operation)
    {
        this.webhookBackgroundTaskQueue.QueueBackgroundWorkItem(async cancellationToken =>
            {
                await operation();
            }, Guid.NewGuid());
    }


    public static async Task<T> ExecuteWithRetryAsync<T>(
    string connectionString,
    Func<SqlConnection, DbTransaction, Task<T>> operation,
    int maxRetries = 3,
    CancellationToken cancellationToken = default)
    {
        int retryCount = 0;
        var exceptions = new List<Exception>();

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                using (var transaction = await connection.BeginTransactionAsync(cancellationToken))
                {
                    try
                    {
                        var result = await operation(connection, transaction);
                        await transaction.CommitAsync(cancellationToken);
                        return result;
                    }
                    catch (SqlException ex) when (ex.Number == 1205) // Deadlock
                    {
                        exceptions.Add(ex);
                        await transaction.RollbackAsync(cancellationToken);

                        if (retryCount++ >= maxRetries)
                        {
                            throw new AggregateException(
                                $"Failed after {maxRetries} retries", exceptions);
                        }

                        // Exponential backoff (100ms, 200ms, 400ms, etc.)
                        var delay = TimeSpan.FromMilliseconds(100 * Math.Pow(2, retryCount));
                        await Task.Delay(delay, cancellationToken);
                    }
                    catch
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }
                }
            }
        }
    }
}
