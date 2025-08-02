namespace VegasBackend.DTO
{
    public class MoveDTO
    {
        public string GameId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string PromotionPiece { get; set; } = "none";
    }

}
