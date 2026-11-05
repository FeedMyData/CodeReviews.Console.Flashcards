using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Spectre.Console;

namespace Flashcards;

class Program
{
    static void Main(string[] args)
    {

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        try
        {
            var masterConnectionString = GetRequiredConnectionString(configuration, "master");
            var masterProvider = new DbConnectionProvider(masterConnectionString);
            var masterDatabase = new Database(masterProvider);

            var tcsaConnectionString = GetRequiredConnectionString(configuration, "tcsa");
            var tcsaProvider = new DbConnectionProvider(tcsaConnectionString);
            var tcsaDatabase = new Database(tcsaProvider);

            masterDatabase.CreateDB();
            tcsaDatabase.DropAll();
            int tableCount = tcsaDatabase.GetAppTableCount();

            if (tableCount == 0)
                FirstStartup(tcsaDatabase);

            else if (tableCount != 2)
                ResetDatabase(tcsaDatabase);

            // --------------------
            var test = tcsaDatabase.SelectAllFromFlashCard();
            foreach (Flashcard flashcard in test)
                Console.WriteLine(flashcard.Recto + " => " + flashcard.Verso);

            // UserInterface.MainMenu();
        }
        catch (SqlException ex)
        {
            AnsiConsole.MarkupLine("[red]Database operation failed. The application cannot continue startup.[/]");
            AnsiConsole.MarkupLine($"[grey]{Markup.Escape(ex.Message)}[/]");
        }
        catch (InvalidOperationException ex)
        {
            AnsiConsole.MarkupLine("[red]Operation failed. The application cannot continue startup.[/]");
            AnsiConsole.MarkupLine($"[grey]{Markup.Escape(ex.Message)}[/]");
        }

        Console.WriteLine("Press any key to quit.");
        Console.ReadLine();
    }

    private static string GetRequiredConnectionString(IConfiguration configuration, string name)
    {
        // ?? => if null
        return configuration.GetConnectionString(name)
                ?? throw new InvalidOperationException($"Connection string '{name}' is missing.");
    }

    private static void FirstStartup(Database database)
    {
        database.CreateSchema();
        database.CreateTables();
        Seeder.Seed(database);
    }

    private static void ResetDatabase(Database database)
    {
        AnsiConsole.MarkupLine("[red]The database has either one or more than the two required tables.[/]");
        AnsiConsole.MarkupLine("[grey]Erase all data or inspect the database manually.[/]");
        AnsiConsole.MarkupLine("[grey]'n' will quit the app.[/]");
        bool retry = AnsiConsole.Confirm("[red]Erase all data and start a fresh database?[/]");

        if (retry)
        {
            database.DropAll();
            FirstStartup(database);
        }

        else
            throw new InvalidOperationException();
    }
}
