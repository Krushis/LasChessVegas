namespace VegasBackend.Models
{
    public enum EndGameType
    {
        None,
        Checkmate,
        Stalemate,
        InsufficientMaterial
        //RepetitionDraw
        // I guess 50 move rule would be interesting but no point now
    }
}
