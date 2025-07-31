namespace VegasBackend.DTO
{
    public class LegalMovesDTO
    {
        public string[][] Board {  get; set; }

        public string? LastMove { get; set; } = null;
        public int MoveCount { get; set; }
    }
}
