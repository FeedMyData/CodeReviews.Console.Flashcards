using Microsoft.Extensions.Configuration;

namespace Flashcards;

class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("Flashcards")
            ?? throw new InvalidOperationException(
                "Connection string 'Flashcards' is missing.");

        var provider = new DbConnectionProvider(connectionString);
        var database = new Database(provider);

        database.CreateTableFlashcards();
        database.CreateTableStacks();

        // Displayer.PrintTitle();
        // UserInterface.MainMenu();
    }
}
