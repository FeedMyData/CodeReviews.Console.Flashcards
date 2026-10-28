using Dapper;

namespace Flashcards;

public sealed class Database
{
    private readonly DbConnectionProvider _connectionProvider;

    public Database(DbConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    internal void CreateTableFlashcards()
    {
        bool tableExisted;

        using var Connection = _connectionProvider.GetConnection();
        {
            string tableName = "flashcards";

            if (!TableExisted(tableName))
            {
                string createSql =
                    @$"CREATE TABLE IF NOT EXISTS @tableName(
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    recto TEXT,
                    verso TEXT
                    stack_id INTEGER FOREIGN KEY)";

                Connection.Execute(createSql, new { tableName });
            }
        }
    }

    internal void CreateTableStacks()
    {
        bool tableExisted;

        using var Connection = _connectionProvider.GetConnection();
        {
            string tableName = "stacks";

            if (!TableExisted(tableName))
            {
                string createSql =
                    @$"CREATE TABLE IF NOT EXISTS @tableName(
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    recto TEXT,
                    verso TEXT
                    stack_id INTEGER FOREIGN KEY)";

                Connection.Execute(createSql, new { tableName });
            }
        }
        // if (!tableExisted)
        //     DebugTool.SeedData();
    }

    private bool TableExisted(string tableName)
    {
        using var Connection = _connectionProvider.GetConnection();

        const string existsSql =
        $@"SELECT EXISTS (SELECT name
            FROM sqlite_master
            WHERE type = 'table'
            AND name = '@tableName')";

        return Connection.ExecuteScalar<bool>(existsSql, new { tableName = tableName });
    }
}