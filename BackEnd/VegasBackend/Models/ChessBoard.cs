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

        public static string[][] CloneBoard(string[][] board)
        {
            var newBoard = new string[8][];
            for (int i = 0; i < 8; i++)
            {
                newBoard[i] = new string[8];
                for (int j = 0; j < 8; j++)
                {
                    newBoard[i][j] = board[i][j];
                }
            }
            return newBoard;
        }
        
    }
}
