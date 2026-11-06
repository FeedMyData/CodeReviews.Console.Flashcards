namespace Flashcards;

internal class Flashcard
{
  internal int FlashcardId { get; private set; }
  internal int StackId { get; private set; }
  internal string Recto { get; private set; }
  internal string Verso { get; private set; }

  public Flashcard(int flashcardId, string recto, string verso, int stackId)
  {
    FlashcardId = flashcardId;
    StackId = stackId;
    Recto = recto;
    Verso = verso;
  }
}