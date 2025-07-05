namespace VegasBackend.Models.Pieces
{
    public class Pawn : Piece
    {
        public override string Notation => IsWhite ? "wP" : "bP";
        public int PositionRow;
        public int PositionCol;

        public Pawn(int positionCol)
        {
            PositionCol = positionCol;
        }

        public Pawn(int positionCol, int positionRow)
        {
            PositionCol = positionCol;
            PositionRow = positionRow;
        }

        public override List<string> GetLegalMoves(ChessBoard board)
        {
            List<string> moves = new();

            int direction = IsWhite ? -1 : 1; 
            int startRow = IsWhite ? 6 : 1; // white is on row 6, black is on row 1

            int row = PositionRow;
            int col = PositionCol;

            // Move forward 1
            int nextRow = row + direction;
            if (IsWithinBounds(nextRow, col) && board.board[nextRow][col] == "-")
            {
                moves.Add(AnnotationHelper.MakeMove(col, row, col, nextRow));

                // Move forward 2
                int twoForward = row + 2 * direction;
                if (row == startRow && board.board[twoForward][col] == "-")
                {
                    moves.Add(AnnotationHelper.MakeMove(col, row, col, twoForward));
                }
            }

            // Capture right
            int rightCol = col + 1;
            if (IsWithinBounds(nextRow, rightCol) && board.board[nextRow][rightCol] != "-")
            {
                moves.Add(AnnotationHelper.MakeMove(col, row, rightCol, nextRow));
            }

            // Capture left
            int leftCol = col - 1;
            if (IsWithinBounds(nextRow, leftCol) && board.board[nextRow][leftCol] != "-")
            {
                moves.Add(AnnotationHelper.MakeMove(col, row, leftCol, nextRow));
            }

            // en passant, promotion

            return moves;
        }

        private bool IsWithinBounds(int row, int col)
        {
            return row >= 0 && row < 8 && col >= 0 && col < 8;
        }

    }
}
