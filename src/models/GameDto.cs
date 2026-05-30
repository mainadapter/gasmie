namespace gasmie.src.models;

public record GameDto(
    string Status,
    string Name,
    string Image,
    string MainDuration,
    string CompletionistDuration,
    string Genres,
    string Developers,
    string Release,
    string URL)
{
    public override string ToString() => "games";
}
