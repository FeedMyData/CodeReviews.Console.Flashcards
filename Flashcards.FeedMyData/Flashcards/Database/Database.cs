using System.Linq.Expressions;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Flashcards;

public sealed class Database
{
    private DbConnectionProvider _connectionProvider;
    private const string flashcardsTableName = "poop.flashcards";
    private const string stacksTableName = "poop.stacks";

    public Database(DbConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    internal T Execute<T>(Func<DbConnectionProvider, T> action)
    {
        try
        {
            _connectionProvider.GetConnection();
            return action(_connectionProvider);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Database access failed.", ex);
        }
    }

    internal void CreateDB()
    {
        using var connection = _connectionProvider.GetConnection();
        string sql = @$"IF db_ID('tcsa') IS NULL 
                            EXEC(N'CREATE DATABASE [tcsa]')";

        connection.Execute(sql);
    }

    internal void CreateSchema()
    {
        using var connection = _connectionProvider.GetConnection();
        string sql = @$"IF SCHEMA_id(N'poop') IS NULL
                            EXEC(N'CREATE SCHEMA [poop]')";

        connection.Execute(sql);
    }

    internal int GetAppTableCount()
    {
        using var connection = _connectionProvider.GetConnection();

        const string sql = """
        SELECT
            CASE WHEN OBJECT_ID(N'poop.stacks', N'U') IS NOT NULL THEN 1 ELSE 0 END
          + CASE WHEN OBJECT_ID(N'poop.flashcards', N'U') IS NOT NULL THEN 1 ELSE 0 END;
        """;

        return connection.ExecuteScalar<int>(sql);
    }

    internal void CreateTables()
    {
        CreateTableStacks();
        CreateTableFlashcards();
    }

    private void CreateTableStacks()
    {
        using var connection = _connectionProvider.GetConnection();

        string sql = """
            IF OBJECT_ID(N'tcsa.poop.stacks', N'U') IS NULL
                CREATE TABLE tcsa.poop.stacks (
                    stackId INT IDENTITY(1,1) PRIMARY KEY,
                    stackName NVARCHAR(200) NOT NULL)
            """;

        connection.Execute(sql);
    }

    private void CreateTableFlashcards()
    {
        using var connection = _connectionProvider.GetConnection();

        string sql = """
            IF OBJECT_ID(N'poop.flashcards', N'U') IS NULL
                CREATE TABLE poop.flashcards (
                    id INT IDENTITY(1,1) PRIMARY KEY,
                    recto NVARCHAR(MAX),
                    verso NVARCHAR(MAX),
                    stackId INT NOT NULL,
                    FOREIGN KEY (stackId) REFERENCES poop.stacks(stackId))
            """;

        connection.Execute(sql);
    }

    internal void AddFlashcard(int stackId, string recto, string verso)
    {
        using var connection = _connectionProvider.GetConnection();
        string sql = @$"INSERT INTO {flashcardsTableName} (stackId, recto, verso) VALUES (@stackId, @recto, @verso )";

        connection.Execute(sql, new { stackId, recto, verso, });
    }

    internal void AddStack(string stackName)
    {
        using var connection = _connectionProvider.GetConnection();
        string sql = @$"INSERT INTO {stacksTableName} (stackName) VALUES (@stackName)";

        connection.Execute(sql, new { stackName });
    }

    internal List<Flashcard> SelectAllFromFlashCard()
    {
        using var connection = _connectionProvider.GetConnection();
        string sql = @$"SELECT * FROM {flashcardsTableName} ORDER BY recto ASC";
        return connection.Query<Flashcard>(sql).ToList();
    }
}