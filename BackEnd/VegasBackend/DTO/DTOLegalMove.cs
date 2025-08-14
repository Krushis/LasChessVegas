namespace VegasBackend.DTO
{
    public class DTOLegalMove
    {
        public string Move { get; set; }        // e.g., "e5d6"
        public bool IsEnPassant { get; set; }
        public bool IsPawnPromotion {get; set; }
        public bool IsCastle {  get; set; }
    }
}
