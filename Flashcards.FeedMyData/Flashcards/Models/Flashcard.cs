namespace Flashcards;

internal class Flashcard
{
  internal int Id { get; private set; }
  internal int Id_stack { get; private set; }
  internal string Recto { get; private set; }
  internal string Verso { get; private set; }

  public Flashcard(int id, int id_stack, string recto, string verso)
  {
    Id = id;
    Id_stack = id_stack;
    Recto = recto;
    Verso = verso;
  }
}