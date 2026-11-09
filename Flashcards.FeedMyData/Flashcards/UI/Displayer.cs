using Spectre.Console;

namespace Flashcards;

internal class Displayer
{

    internal static void PrintTitle()
    {
        string path = Path.Combine(AppContext.BaseDirectory,
                                    @"UI\DancingFont.flf");
        var font = FigletFont.Load(path);
        Console.Clear();
        var figlet = new FigletText("Flashcards").Color(Color.Yellow);
        AnsiConsole.Write(figlet);
        AnsiConsole.WriteLine();
    }

    internal static void Loading(string title, Action<ProgressTask> work)
    {
        AnsiConsole.MarkupLine($"[bold yellow]{title}[/]");
        AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new ElapsedTimeColumn())
            .Start(context =>
            {
                var task = context.AddTask("Will be overwritten", maxValue: 3);
                work(task);
            });
    }

    internal static void GenericError(string message)
    {
        AnsiConsole.MarkupLine($"[red]{message}[/]");
        AnsiConsole.MarkupLine($"[grey]Press any key to go back to the Main Menu.[/]");
        Console.ReadKey();
    }

    internal static void CrudSuccess(string objectType, string action)
    {
        AnsiConsole.MarkupLine($"[green]The {objectType} has been {action}.[/]");
        AnsiConsole.WriteLine();
    }

    internal static void CrudDiscard(string objectType, string action)
    {
        AnsiConsole.MarkupLine($"[red]The {objectType} has been {action}.[/]");
        AnsiConsole.WriteLine();
    }
}