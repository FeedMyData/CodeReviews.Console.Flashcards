using Microsoft.Data.SqlClient;
using Spectre.Console;

namespace Flashcards;

public sealed class DbConnectionProvider
{
    private readonly string _connectionString;

    internal DbConnectionProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    internal SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }

    internal T Execute<T>(Func<SqlConnection, T> action)
    {
        while (true)
        {
            try
            {
                using var connection = GetConnection();
                return action(connection);
            }
            catch (SqlException ex)
            {
                AnsiConsole.MarkupLine("[red]The app cannot access its database.[/]");
                AnsiConsole.MarkupLine($"[grey]{ex.Message}[/]");
                AnsiConsole.MarkupLine("[grey]Ensure the local docker server is running before retrying.[/]");
                AnsiConsole.MarkupLine("[grey]'n' will quit the app.[/]");
                bool retry = AnsiConsole.Confirm("Do you want to retry?");

                if (!retry)
                    throw;
            }
        }
    }
}