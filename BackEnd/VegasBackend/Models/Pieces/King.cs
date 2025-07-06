using System.ComponentModel;

namespace VegasBackend.Models.Pieces
{
    public class King : Piece
    {
        public override string Notation => IsWhite ? "wP" : "bP";
        public int PositionRow;
        public int PositionCol;

        public King(int positionCol)
        {
            PositionCol = positionCol;
        }

        public King(int positionCol, int positionRow)
        {
            PositionCol = positionCol;
            PositionRow = positionRow;
        }

        public override List<string> GetLegalMoves(string[][] board)
        {

            return new List<string>();
        }
    }
}
