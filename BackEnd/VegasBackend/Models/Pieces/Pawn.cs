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

        public override List<string> GetLegalMoves(string[][] board, string lastMove)
        {
            List<string> moves = new();

            int direction = IsWhite ? -1 : 1;
            int startRow = IsWhite ? 6 : 1; // white is on row 6, black is on row 1
            string colorLetter = Notation.Substring(0, 1);

            int row = PositionRow;
            int col = PositionCol;

            // en pessante
            if (!string.IsNullOrEmpty(lastMove))
            {
                var from = AnnotationHelper.AlgebraicToIndex(lastMove.Substring(0, 2));
                var to = AnnotationHelper.AlgebraicToIndex(lastMove.Substring(2, 2));

                if (from != null && to != null)
                {
                    int movedFromRow = from.Value.Row;
                    int movedFromCol = from.Value.Col;
                    int movedToRow = to.Value.Row;
                    int movedToCol = to.Value.Col;

                    string movedPiece = board[movedToRow][movedToCol];
                    bool isEnemyPawn = movedPiece == (IsWhite ? "bP" : "wP");
                    bool movedTwoForward = Math.Abs(movedFromRow - movedToRow) == 2;

                    if (isEnemyPawn && movedTwoForward && Math.Abs(movedToCol - col) == 1 && movedToRow == row)
                    {
                        int moveRow = row + direction;
                        moves.Add(AnnotationHelper.MakeMove(col, row, movedToCol, moveRow));
                    }
                }
            }

            // Move forward 1
            int nextRow = row + direction;
            if (IsWithinBounds(nextRow, col) && board[nextRow][col] == "-")
            {
                moves.Add(AnnotationHelper.MakeMove(col, row, col, nextRow));

                // Move forward 2
                int twoForward = row + 2 * direction;
                if (row == startRow && board[twoForward][col] == "-")
                {
                    moves.Add(AnnotationHelper.MakeMove(col, row, col, twoForward));
                }
            }

            // Capture right
            int rightCol = col + 1;
            if (IsWithinBounds(nextRow, rightCol) && board[nextRow][rightCol] != "-" && board[nextRow][rightCol].Substring(0, 1) != colorLetter)
            {
                moves.Add(AnnotationHelper.MakeMove(col, row, rightCol, nextRow));
            }

            // Capture left
            int leftCol = col - 1;
            if (IsWithinBounds(nextRow, leftCol) && board[nextRow][leftCol] != "-" && board[nextRow][leftCol].Substring(0, 1) != colorLetter)
            {
                moves.Add(AnnotationHelper.MakeMove(col, row, leftCol, nextRow));
            }

            // TODO: promotion

            return moves;
        }

        public override List<string> GetLegalMoves(string[][] board)
        {
            return GetLegalMoves(board, null);
        }
    }
}
