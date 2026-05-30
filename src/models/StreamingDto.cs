namespace gasmie.src.models;

public record StreamingDto(
    string Type,
    string Image,
    string Name,
    string Episodes,
    string Genres,
    string Duration,
    string URL)
{
    public static string Status => "To Watch";

    public override string ToString() => "streaming";
}
