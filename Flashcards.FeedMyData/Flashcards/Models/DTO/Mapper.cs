namespace Flashcards;

internal static class ModelsMapper
{
    public static FlaschcardDto FlashcardToDto(Flashcard flashcard, int displayId)
    {
        return new FlaschcardDto(displayId, flashcard.Recto, flashcard.Verso);
    }

    public static StackDto StackToDto(Stack stack, int displayId)
    {
        return new StackDto(displayId, stack.Name);
    }
}