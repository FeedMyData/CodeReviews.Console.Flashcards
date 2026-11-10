using System.Reflection.Metadata.Ecma335;
using Microsoft.Data.SqlClient;
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
                Displayer.CleanConsole();

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<Enums.MenuChoice>()
                .Title("What do you want to do?")
                .HighlightStyle(Color.Yellow)
                .UseConverter(choice => choice.GetDisplayName())
                .AddChoices(Enum.GetValues<Enums.MenuChoice>()));

            try
            {
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
            catch (SqlException ex)
            {
                AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
                AnsiConsole.MarkupLine("[grey]Press any key to return to the main menu.[/]");
                Console.ReadKey();
            }
        }
        return exitApp;
    }

    private void CrudMenu(string itemType, Action addItem, Action editItem, Action deleteItem)
    {
        bool goBack = false;

        while (!goBack)
        {
            Displayer.CleanConsole();

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<Enums.CrudChoice>()
                .Title($"[yellow]Manage {itemType}[/]")
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
                    goBack = true;
                    break;
            }
        }
    }

    void AddFlashcard()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[green]New [bold]FLASHCARD[/][/]");
            AnsiConsole.WriteLine();

            // Display an "empy" stack if the user needs a new one
            Stack newStack = new(-1, "_New Stack");
            List<Stack> stacks = _database.QueryAllFromStacks();
            stacks.Add(newStack);
            stacks.Sort();

            var stack = ChoicesPrompt.Select(stacks, "Choose a stack", "green");
            if (stack.StackId == -1)
            {
                AddStack();
                stack = _database.QueryNewestStack();
            }
            AnsiConsole.MarkupLine($"Choosen stack: {stack.Name}");

            var recto = AnsiConsole.Ask<string>("Add a recto :");
            var verso = AnsiConsole.Ask<string>("Add a verso :");

            AnsiConsole.WriteLine();
            bool confirm = AnsiConsole.Confirm("Confirm and add the flashcard?");

            if (confirm)
            {
                _database.AddFlashcard(stack.StackId, recto, verso);
                Displayer.CrudSuccess("flashcard", "added");
            }
            else
                Displayer.CrudDiscard("flashcard", "discarded");

            if (AnsiConsole.Confirm("Add another flashcard?"))
                break;

            AnsiConsole.WriteLine();
        }
    }

    void AddStack()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[green]New [bold]STACK[/][/]");
            AnsiConsole.WriteLine();
            var name = AnsiConsole.Ask<string>("Add a name :");

            bool confirm = AnsiConsole.Confirm("Add the stack?");

            if (confirm)
            {
                _database.AddStack(name);
                Displayer.CrudSuccess("stack", "added");
            }
            else
                Displayer.CrudDiscard("stack", "discarded");

            if (!AnsiConsole.Confirm("Add another Stack?"))
                break;

            AnsiConsole.WriteLine();
        }
    }

    void EditFlashcard()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[blue]Edit [bold]Flashcard[/][/]");
            AnsiConsole.WriteLine();
            List<Flashcard> flashcards = FilterSelectIfNeeded();
            AnsiConsole.WriteLine();

            var selected = ChoicesPrompt.Select(flashcards, "Choose a Flaschard", "blue");
            string newRecto = selected.Recto;
            string newVerso = selected.Verso;

            string[] subChoice = { "recto", "verso" };
            List<string> dataToEdit = ChoicesPrompt.SelectMulti(subChoice, "What do you want to Edit?", "blue");
            AnsiConsole.MarkupLine($"Currently editing {selected.Display}");


            if (dataToEdit.Count == 0)
                return;
            if (dataToEdit.Contains("recto"))
                selected.EditRecto(AnsiConsole.Ask<string>("Enter a new recto: "));
            if (dataToEdit.Contains("verso"))
                selected.EditVerso(AnsiConsole.Ask<string>("Enter a new verso: "));

            if (AnsiConsole.Confirm("Confirm?"))
                _database.UpdateFlashcard(selected);
            else
                Displayer.CrudDiscard("edit", "canceled");

            if (!AnsiConsole.Confirm("Edit other flashcards?"))
                break;

            AnsiConsole.WriteLine();
        }
    }

    void EditStack()
    {
        while (true)
        {
            AnsiConsole.MarkupLine("[blue]Edit [bold]Stack[/][/]");
            AnsiConsole.WriteLine();
            List<Stack> stacks = _database.QueryAllFromStacks();

            var selected = ChoicesPrompt.Select(stacks, "Choose a stack", "blue");

            AnsiConsole.MarkupLine($"Currently editing '{selected.Name}'");

            selected.EditName(AnsiConsole.Ask<string>("Enter a new name: "));

            if (AnsiConsole.Confirm("Confirm?"))
                _database.UpdateStack(selected);

            else
                Displayer.CrudDiscard("edit", "canceled");

            if (!AnsiConsole.Confirm("Edit other stacks?"))
                break;

            AnsiConsole.WriteLine();
        }
    }

    void DeleteFlashcard()
    {
        while (true)
        {
            AnsiConsole.MarkupLine($"[red]Delete [bold]Flashcard[/][/]");
            AnsiConsole.WriteLine();
            List<Flashcard> flashcards = FilterSelectIfNeeded();

            var toDelete = ChoicesPrompt.SelectMulti(flashcards, "red");
            if (toDelete.Count == 0)
                return;

            foreach (Flashcard f in toDelete)
                AnsiConsole.MarkupLine($"Recto -> {f.Recto}");

            AnsiConsole.WriteLine();

            if (AnsiConsole.Confirm($"[red]Delete {toDelete.Count} Flashcards?[/]"))
            {
                foreach (Flashcard f in toDelete)
                    _database.DeleteFlashcard(f);

                Displayer.CrudSuccess("deletion", "successful");
            }
            else
                Displayer.CrudDiscard("deletion", "canceled");

            if (!AnsiConsole.Confirm("Delete other Flashcard?"))
                break;

            AnsiConsole.WriteLine();
        }
    }

    void DeleteStack()
    {
        bool doIt = true;
        while (doIt)
        {
            AnsiConsole.MarkupLine("[red]Delete [bold]STACKS[/][/]");
            AnsiConsole.WriteLine();

            List<Stack> stacks = _database.QueryAllFromStacks();
            var toDelete = ChoicesPrompt.SelectMulti(stacks, "red");
            if (toDelete.Count == 0)
                break;

            foreach (Stack stack in toDelete)
                AnsiConsole.MarkupLine($"[red]Delete -> {stack.Name}[/]");

            AnsiConsole.WriteLine();

            if (AnsiConsole.Confirm("Confirm?"))
            {
                foreach (Stack stack in toDelete)
                    _database.DeleteStack(stack);

                Displayer.CrudSuccess("deletion", "successful");
            }
            else
                Displayer.CrudDiscard("deletion", "canceled");

            doIt = AnsiConsole.Confirm("Delete other Stacks?");
            AnsiConsole.WriteLine();
        }
    }

    private List<Flashcard> FilterSelectIfNeeded()
    {
        var filter = AnsiConsole.Confirm("Filter per stack?");

        if (filter)
        {
            List<Stack> stacks = _database.QueryAllFromStacks();
            Stack stack = ChoicesPrompt.Select(stacks, "Choose a stack", "red");
            return _database.QueryFlashcardsFromStack(stack);
        }

        else
            return _database.QueryAllFromFlashcards();
    }
}
