﻿namespace VegasBackend.DTO
{
    public class LegalMoveDTO
    {
        public string Move { get; set; }        // e.g., "e5d6"
        public bool IsEnPassant { get; set; }
        public bool IsPawnPromotion {get; set; }
    }
}
