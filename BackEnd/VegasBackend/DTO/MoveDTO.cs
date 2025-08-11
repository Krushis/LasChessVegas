namespace VegasBackend.DTO
{
    public class MoveDTO
    {
        public Guid GameId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string PromotionPiece { get; set; } = "none";
    }

}
