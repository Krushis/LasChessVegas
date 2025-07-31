namespace VegasBackend.DTO
{
    public class MoveDTO
    {
        public string From { get; set; }
        public string To { get; set; }
        public string[][] Board {  get; set; }
        public int MoveCount {  get; set; }
        public string? LastMove { get; set; } = null;

    }
}
