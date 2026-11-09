using Spectre.Console;

namespace Flashcards;

internal static class ChoicesPrompt
{
    internal static Stack SelectStack(Database database, string color)
    {
        // Present an "empy" stack if the user needs a new one
        Stack newStack = new(-1, "_New Stack");
        List<Stack> stacks = database.SelectAllFromStacks();
        stacks.Add(newStack);

        var selection = AnsiConsole.Prompt(
                new SelectionPrompt<Stack>()
                .Title("Choose a stack")
                .HighlightStyle(color)
                .UseConverter(stack => stack.Name)
                .AddChoices(stacks));

        return selection;
    }
}