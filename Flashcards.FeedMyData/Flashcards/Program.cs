using Microsoft.Extensions.Configuration;

namespace Flashcards;

// fix probleme with CSV
// Make the Config cleaner

class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var masterConnectionString = GetRequiredConnectionString(configuration, "master");
        var masterProvider = new DbConnectionProvider(masterConnectionString);
        var masterDatabase = new Database(masterProvider);

        masterDatabase.CreateDB();

        var tcsaConnectionString = GetRequiredConnectionString(configuration, "tcsa");
        var tcsaProvider = new DbConnectionProvider(tcsaConnectionString);
        var tcsaDatabase = new Database(tcsaProvider);

        int tableCount = tcsaDatabase.GetAppTableCount();

        if (tableCount == 0)
        {
            tcsaDatabase.CreateSchema();
            tcsaDatabase.CreateTables();
            Seeder.Seed(tcsaDatabase);
        }
        else if (tableCount != 2)
        {
            throw new InvalidOperationException("The database has only one of the two required tables.");
        }

        //--------------------
        // var test = tcsaDatabase.SelectAllFromFlashCard();
        // foreach (Flashcard flashcard in test)
        //     Console.WriteLine(flashcard.Recto + " => " + flashcard.Verso);

        // Console.WriteLine("--");
        // Console.ReadLine();

        // Displayer.PrintTitle();
        // UserInterface.MainMenu();
    }

    private static string GetRequiredConnectionString(IConfiguration configuration, string name)
    {
        return configuration.GetConnectionString(name)
                    ?? throw new InvalidOperationException(
                         $"Connection string '{name}' is missing.");
    }
}
