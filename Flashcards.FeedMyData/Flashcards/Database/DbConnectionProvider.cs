using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Flashcards;

public sealed class DbConnectionProvider
{
    private readonly string _connectionString;

    public DbConnectionProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }
}