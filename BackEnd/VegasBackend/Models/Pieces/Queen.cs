using System.ComponentModel;

namespace VegasBackend.Models.Pieces
{
    public class Queen : Piece
    {
        public override string Notation => IsWhite ? "wP" : "bP";
        public int PositionRow;
        public int PositionCol;

        public Queen(int positionCol)
        {
            PositionCol = positionCol;
        }

        public Queen(int positionCol, int positionRow)
        {
            PositionCol = positionCol;
            PositionRow = positionRow;
        }

        public override List<string> GetLegalMoves(ChessBoard board)
        {

            return new List<string>();
        }
    }
}
