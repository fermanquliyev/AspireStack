using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System.ComponentModel;

public class RetryingSqlServerRetryingExecutionStrategy : NpgsqlRetryingExecutionStrategy
{
    public RetryingSqlServerRetryingExecutionStrategy(ExecutionStrategyDependencies dependencies) : base(dependencies)
    {
    }

    protected override bool ShouldRetryOn(Exception exception)
    {
        if (exception is PostgresException postgresException)
        {
            // EF Core issue logged to consider making this a default https://github.com/dotnet/efcore/issues/33191
            if (postgresException.SqlState == "3D000")
            {
                // Don't retry on login failures associated with default database not existing due to EF migrations not running yet
                return false;
            }
            // Workaround for https://github.com/dotnet/aspire/issues/1023
            else if (postgresException.SqlState == "08000" || (postgresException.SqlState == "08003" && postgresException.InnerException is Win32Exception))
            {
                return true;
            }
        }

        return base.ShouldRetryOn(exception);
    }
}