using System.Runtime.CompilerServices;

namespace VegasBackend.Models
{
    public class ChessBoard
    {
        public string[][] board {  get; set;} 
        public ChessBoard() { }

        public void InitializeBoard()
        {
            board = new string[][]
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
        }
        
    }
}
