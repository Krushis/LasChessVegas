namespace VegasBackend.Models
{
    public class EndGameResult
    {
        public EndGameType Type { get; set; }
        public string Winner { get; set; } // "w", "b", or null if draw
    }
}
