using System.ComponentModel.DataAnnotations;

namespace Flashcards;

internal class Enums
{
    internal enum MenuChoice
    {
        [Display(Name = "Study")]
        study,

        [Display(Name = "View study sessions")]
        studyView,

        [Display(Name = "Manage Stacks")]
        manageStacks,

        [Display(Name = "Manage Flashcards")]
        manageFlashcards,

        [Display(Name = "Exit")]
        exit,
    }

    internal enum CrudChoice
    {
        [Display(Name = "[green]Add[/] an item")]
        addItem,

        [Display(Name = "[blue]Edit[/] an item")]
        editItem,

        [Display(Name = "[red]Delete[/] an item")]
        deleteItem,

        [Display(Name = "Go back to Main Menu")]
        mainMenu,
    }
}