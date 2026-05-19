namespace gasmie.src.data
{
    public record StreamingDto(
        string Type,
        string Image,
        string Name,
        string Episodes,
        string Genres,
        string Duration,
        string URL)
    {
        public string Status => "To Watch";
        
        public override string ToString() => "streaming";
    }
}