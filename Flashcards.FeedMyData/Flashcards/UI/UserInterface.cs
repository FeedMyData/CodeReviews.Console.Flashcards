using Spectre.Console;

namespace Flashcards;

internal class UserInterface
{
    private Database _database;
    private int displayCount;

    internal UserInterface(Database database)
    {
        _database = database;
    }

    internal bool MainMenu()
    {
        // Ensure loading stay displayed on first launch
        bool exitApp = false;

        while (!exitApp)
        {
            displayCount++;
            if (displayCount != 1)
            {
                Console.Clear();
                Displayer.PrintTitle();
            }

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<Enums.MenuChoice>()
                .Title("What do you want to do?")
                .HighlightStyle(Color.Yellow)
                .UseConverter(choice => choice.GetDisplayName())
                .AddChoices(Enum.GetValues<Enums.MenuChoice>()));

            switch (selection)
            {
                case Enums.MenuChoice.study:
                    Displayer.GenericError("Not implemented.");
                    break;
                case Enums.MenuChoice.studyView:
                    Displayer.GenericError("Not implemented.");
                    break;
                case Enums.MenuChoice.manageFlashcards:
                    CrudMenu("Flashcard", AddFlashcard, EditFlashcard, DeleteFlashcard);
                    break;
                case Enums.MenuChoice.manageStacks:
                    CrudMenu("Stack", AddStack, EditStack, DeleteStack);
                    break;
                case Enums.MenuChoice.exit:
                    return exitApp == true;
            }
        }
        return exitApp;
    }

    private void CrudMenu(string itemType, Action addItem, Action editItem, Action deleteItem)
    {
        AnsiConsole.MarkupLine($"[yellow]Manage {itemType}[/]");

        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<Enums.CrudChoice>()
            .Title("What do you want to do?")
            .HighlightStyle(Color.Yellow)
            .UseConverter(choice => choice.GetDisplayName())
            .AddChoices(Enum.GetValues<Enums.CrudChoice>()));

        switch (selection)
        {
            case Enums.CrudChoice.addItem:
                addItem();
                break;
            case Enums.CrudChoice.editItem:
                editItem();
                break;
            case Enums.CrudChoice.deleteItem:
                deleteItem();
                break;
            case Enums.CrudChoice.mainMenu:
                MainMenu();
                break;
        }
    }

    void AddFlashcard()
    {
        bool addingCard = true;
        while (addingCard)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[green]New [bold]FLASHCARD[/][/]");
            AnsiConsole.WriteLine();
            var stack = ChoicesPrompt.SelectStack(_database, "green");
            if (stack.StackId == -1)
            {
                AddStack();
                stack = _database.SelectNewestStack();
            }
            AnsiConsole.MarkupLine($"Choosen stack: {stack.Name}");

            var recto = AnsiConsole.Ask<string>("Add a recto :");
            var verso = AnsiConsole.Ask<string>("Add a verso :");

            AnsiConsole.WriteLine();
            bool confirm = AnsiConsole.Confirm("Confirm and add the flashcard?");

            if (confirm)
            {
                _database.AddFlashcard(stack.StackId, recto, verso);
                AnsiConsole.MarkupLine("[green]The flashcard has been added.[/]");
            }
            else
                AnsiConsole.MarkupLine("[red]The flashcard has been discarded.[/]");

            AnsiConsole.WriteLine();
            addingCard = AnsiConsole.Confirm("Add another flashcard?");
        }
        MainMenu();
    }

    void EditFlashcard()
    {
        Displayer.GenericError("Not implemented.");
    }

    void DeleteFlashcard()
    {
        Displayer.GenericError("Not implemented.");
    }

    void AddStack()
    {
        bool addingStack = true;
        while (addingStack)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[green]New [bold]STACK[/][/]");
            var name = AnsiConsole.Ask<string>("Add a name :");

            bool confirm = AnsiConsole.Confirm("Add the stack?");

            if (confirm)
            {
                _database.AddStack(name);
                Displayer.CrudSuccess("stack", "added");
            }
            else
                Displayer.CrudDiscard("stack", "discarded");

            addingStack = AnsiConsole.Confirm("Add another Stack?");
        }
        MainMenu();
    }

    void EditStack()
    {
        Displayer.GenericError("Not implemented.");
    }

    void DeleteStack()
    {
        Displayer.GenericError("Not implemented.");
    }
}
