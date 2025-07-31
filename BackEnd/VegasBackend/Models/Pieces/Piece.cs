

using System.Xml;

namespace VegasBackend.Models.Pieces
{
    public abstract class Piece
    {
        public bool IsWhite { get; set; }
        public abstract string Notation { get; }

        public abstract List<string> GetLegalMoves(string[][] board);

        public virtual List<string> GetLegalMoves(string[][] board, string? lastMove)
        {
            return GetLegalMoves(board); // fallback to regular
        }

        protected bool IsWithinBounds(int row, int col) // protected so that only child classes runs it
        {
            return row >= 0 && row < 8 && col >= 0 && col < 8;
        }
    }
}
