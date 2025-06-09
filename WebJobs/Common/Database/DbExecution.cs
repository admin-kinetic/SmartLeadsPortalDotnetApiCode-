using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Common.Database;

public class DbExecution
{
    //public static async Task<T> ExecuteWithRetryAsync<T>(
    //string connectionString,
    //Func<SqlConnection, DbTransaction, Task<T>> operation,
    //int maxRetries = 3,
    //CancellationToken cancellationToken = default)
    //{
    //    int retryCount = 0;
    //    var exceptions = new List<Exception>();

    //    while (true)
    //    {
    //        cancellationToken.ThrowIfCancellationRequested();

    //        using (var connection = new SqlConnection(connectionString))
    //        {
    //            await connection.OpenAsync(cancellationToken);
    //            using (var transaction = await connection.BeginTransactionAsync(cancellationToken))
    //            {
    //                try
    //                {
    //                    var result = await operation(connection, transaction);
    //                    await transaction.CommitAsync(cancellationToken);
    //                    return result;
    //                }
    //                catch (SqlException ex) when (ex.Number == 1205) // Deadlock
    //                {
    //                    exceptions.Add(ex);
    //                    await transaction.RollbackAsync(cancellationToken);

    //                    if (retryCount++ >= maxRetries)
    //                    {
    //                        throw new AggregateException(
    //                            $"Failed after {maxRetries} retries", exceptions);
    //                    }

    //                    // Exponential backoff (100ms, 200ms, 400ms, etc.)
    //                    var delay = TimeSpan.FromMilliseconds(100 * Math.Pow(2, retryCount));
    //                    await Task.Delay(delay, cancellationToken);
    //                }
    //                catch
    //                {
    //                    await transaction.RollbackAsync(cancellationToken);
    //                    throw;
    //                }
    //            }
    //        }
    //    }
    //}

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
