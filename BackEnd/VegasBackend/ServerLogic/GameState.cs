namespace VegasBackend.ServerLogic
{
    public class GameState
    {
        public string[][] Board { get; set; } = 
            new string[][]
                {
                    new string[] { "bR", "bN", "bB", "bQ", "bK", "bB", "bN", "bR" }, // Black back rank
                    new string[] { "bp", "bp", "bp", "bp", "bp", "bp", "bp", "bp" }, // Black pawns
                    new string[] { "-", "-", "-", "-", "-", "-", "-", "-" },
                    new string[] { "-", "-", "-", "-", "-", "-", "-", "-" },
                    new string[] { "-", "-", "-", "-", "-", "-", "-", "-" }, // [0] [0] is bR, [0][1] is bN and so on
                    new string[] { "-", "-", "-", "-", "-", "-", "-", "-" },
                    new string[] { "wp", "wp", "wp", "wp", "wp", "wp", "wp", "wp" }, // White pawns
                    new string[] { "wR", "wN", "wB", "wQ", "wK", "wB", "wN", "wR" }  // White back rank
                };
        public int MoveCount { get; set; } = 0;
        public string LastMove { get; set; } = "";
    }
}
