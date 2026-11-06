namespace Flashcards;

public record FlaschcardDto(int displayId, string recto, string verso);
public record StackDto(int displayId, string name);

// public class FlaschcardDto
// {
//     string Recto {get; set;}
//     string Verso {get; set;}
// }

// public class StackDto
// {
//     string Name;
// }