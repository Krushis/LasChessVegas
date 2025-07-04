namespace VegasBackend.Models.Pieces
{
    public class Pawn : Piece
    {
        public override string Notation => IsWhite ? "wP" : "bP";
        public int PositionRow =>  IsWhite ? 6 : 1;
        public int PositionCol;

        public Pawn(int positionCol)
        {
            PositionCol = positionCol;
        }

        public override List<string> GetLegalMoves(ChessBoard board)
        {
            int direction = IsWhite ? -1 : 1; // white move direction is 1, black is -1

            List<string> moves = [];

            if (board.board[PositionRow + direction][PositionCol] == "-")
            {
                moves.Add(AnnotationHelper.MakeMove(PositionCol, PositionRow, PositionCol, PositionRow + direction));
                if (board.board[PositionRow + direction * 2][PositionCol] == "-" && (PositionRow == 6 || PositionRow == 1))
                {
                    moves.Add(AnnotationHelper.MakeMove(PositionCol, PositionRow, PositionCol, PositionRow + 2 * direction));
                }
            }

            if (board.board[PositionRow + direction][PositionCol + 1] != "-")
            {
                moves.Add(AnnotationHelper.MakeMove(PositionCol, PositionRow, PositionCol + 1, PositionRow + direction));
            }

            if (board.board[PositionRow + direction][PositionCol - 1] != "-")
            {
                moves.Add(AnnotationHelper.MakeMove(PositionCol, PositionRow, PositionCol - 1, PositionRow + direction));
            }

            // en passante

            // promotion

            return moves;
        }
    }
}
