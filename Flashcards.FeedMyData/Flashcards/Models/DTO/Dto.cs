namespace Flashcards;

internal record FlaschcardDto(int displayId, string recto, string verso);
internal record StackDto(int displayId, string name);

// public class FlaschcardDto
// {
//     string Recto {get; set;}
//     string Verso {get; set;}
// }

// public class StackDto
// {
//     string Name;
// }