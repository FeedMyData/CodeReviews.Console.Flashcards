namespace Flashcards;

internal class Stack
{
  internal int Id { get; private set; }
  internal int Id_flashcard { get; private set; }
  internal string Name { get; private set; }

  public Stack(int id, int id_flashcard, string name)
  {
    Id = id;
    Id_flashcard = id_flashcard;
    Name = name;
  }
}

