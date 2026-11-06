using Dapper;

namespace Flashcards;

public sealed class Database
{
    private readonly DbConnectionProvider _connection;
    private const string flashcardsTableName = "flash.flashcards";
    private const string stacksTableName = "flash.stacks";

    public Database(DbConnectionProvider connectionProvider)
    {
        _connection = connectionProvider;
    }

    internal void CreateDB()
    {
        string sql = @$"IF db_ID('tcsa') IS NULL 
                            EXEC(N'CREATE DATABASE [tcsa]')";

        _connection.Execute(connection => connection.Execute(sql));
    }

    internal void CreateSchema()
    {
        string sql = @$"IF SCHEMA_id(N'flash') IS NULL
                            EXEC(N'CREATE SCHEMA [flash]')";

        _connection.Execute(connection => connection.Execute(sql));
    }

    internal int GetAppTableCount()
    {
        // const string sql = """
        // SELECT
        //     CASE WHEN OBJECT_ID(N'flash.stacks', N'U') IS NOT NULL THEN 1 ELSE 0 END
        //   + CASE WHEN OBJECT_ID(N'flash.flashcards', N'U') IS NOT NULL THEN 1 ELSE 0 END;
        // """;

        const string sql = """
                    SELECT COUNT(*) AS TableCount
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_SCHEMA = 'flash'
                    AND TABLE_TYPE = 'BASE TABLE';
                    """;

        return _connection.Execute(connection => connection.ExecuteScalar<int>(sql));
    }

    internal void CreateTables()
    {
        CreateTableStacks();
        CreateTableFlashcards();
    }

    internal void DropAll()
    {
        // This fails if the schema contains non-table objects. 
        // To drop all objects (tables, views, procedures) in a schema, you must generate DROP statements for each object type
        string dropStack = "DROP TABLE IF EXISTS flash.flashcards";

        _connection.Execute(connection => connection.Execute(dropStack));

        string sql = """
            DECLARE @SchemaName NVARCHAR(128) = 'flash';
        DECLARE @DropTables NVARCHAR(MAX) = '';

        --Drop Tables
        SELECT @DropTables += 'DROP TABLE [' + TABLE_SCHEMA + '].[' + TABLE_NAME + ']; '
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_SCHEMA = @SchemaName
            AND TABLE_TYPE = 'BASE TABLE';

        EXEC sp_executesql @DropTables;
        """;

        _connection.Execute(connection => connection.Execute(sql));
    }

    private void CreateTableStacks()
    {
        string sql = """
            IF OBJECT_ID(N'tcsa.flash.stacks', N'U') IS NULL
                CREATE TABLE tcsa.flash.stacks(
                    stackId INT IDENTITY(1, 1) PRIMARY KEY,
                    stackName NVARCHAR(200) NOT NULL)
            """;

        _connection.Execute(connection => connection.Execute(sql));
    }

    private void CreateTableFlashcards()
    {
        string sql = """
            IF OBJECT_ID(N'flash.flashcards', N'U') IS NULL
                CREATE TABLE flash.flashcards (
                    flashcardId INT IDENTITY(1,1) PRIMARY KEY,
                    recto NVARCHAR(MAX),
                    verso NVARCHAR(MAX),
                    stackId INT NOT NULL,
                    FOREIGN KEY (stackId) REFERENCES flash.stacks(stackId))
            """;

        _connection.Execute(connection => connection.Execute(sql));
    }

    internal void AddFlashcard(int stackId, string recto, string verso)
    {
        string sql = @$"INSERT INTO {flashcardsTableName} (stackId, recto, verso) VALUES (@stackId, @recto, @verso )";

        _connection.Execute(connection => connection.Execute(sql, new { stackId, recto, verso, }));
    }

    internal void AddStack(string stackName)
    {
        string sql = @$"INSERT INTO {stacksTableName} (stackName) VALUES (@stackName)";

        _connection.Execute(connection => connection.Execute(sql, new { stackName }));
    }

    internal void DeleteStack(string stackName)
    {
        string sql = @$"DELETE FROM {stacksTableName} WHERE stackName = @stackName";

        _connection.Execute(connection => connection.Execute(sql, new { stackName }));
    }

    internal void DeleteFlashcard(string flashcardName)
    {
        string sql = @$"DELETE FROM {flashcardsTableName} WHERE flashcardName = @flashcardName";

        _connection.Execute(connection => connection.Execute(sql, new { flashcardName }));
    }

    internal void UpdateStack(Stack stack)
    {
        string sql = @$"UPDATE {stacksTableName} (stackName) SET name = @name WHERE stackId = @stackId";

        _connection.Execute(connection => connection.Execute(sql, new { stack.Name, stack.StackId }));
    }

    internal void UpdateFlashcard(Flashcard f)
    {
        string sql = @$"UPDATE{flashcardsTableName} SET recto = @recto, verso = @verso";

        _connection.Execute(connection => connection.Execute(sql, new { f.Recto, f.Verso }));
    }

    internal List<Flashcard> SelectAllFromFlashCard()
    {
        string sql = @$"SELECT * FROM {flashcardsTableName} ORDER BY recto ASC";
        return _connection.Execute(connection => connection.Query<Flashcard>(sql).ToList());
    }
}