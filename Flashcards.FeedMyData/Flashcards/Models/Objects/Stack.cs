namespace Flashcards;

internal class Stack
{
  internal int StackId { get; private set; }
  internal int FlashcardID { get; private set; }
  internal string Name { get; private set; }

  public Stack(int stackid, int flashcardID, string name)
  {
    StackId = stackid;
    FlashcardID = FlashcardID;
    Name = name;
  }
}

