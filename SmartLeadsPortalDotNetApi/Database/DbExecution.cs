using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace SmartLeadsPortalDotNetApi.Database;

public static class DbExecution
{

    private static Serilog.ILogger _logger;

    // Initialize the logger once
    public static void Initialize(Serilog.ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public static async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3)
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
                _logger.Warning(ex, "Deadlock detected. Retrying...");
                if (retryCount++ >= maxRetries)
                    throw;

                var delay = TimeSpan.FromMilliseconds(Math.Pow(2, retryCount) * 100);
                _logger.Information("Retrying after {Delay} ms", delay.TotalMilliseconds);
                await Task.Delay(delay);
            }
        }
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
