using VegasBackend.Models.Pieces;

namespace VegasBackend.Models
{
    public class AnnotationHelper
    {

        /// <summary>
        /// Helps make the Annotation for the move we want to make
        /// </summary>
        /// <param name="sCol"></param>
        /// <param name="sRow"></param>
        /// <param name="eCol"></param>
        /// <param name="eRow"></param>
        /// <returns>a string like, e4e5 (starting row is 4) for the annotation of the move we want to make</returns>
        public static string MakeMove(int sCol, int sRow, int eCol, int eRow)
        {
            string start = IndexToAlgebraic(sCol, sRow);
            string end = IndexToAlgebraic(eCol, eRow);
            return start + end;
        }

        private static string IndexToAlgebraic(int col, int row)
        {
            char file = (char)('a' + col);
            int rank = 8 - row;
            return $"{file}{rank}";
        }

        public static (int Row, int Col)? AlgebraicToIndex(string algebraic)
        {
            if (string.IsNullOrEmpty(algebraic) || algebraic.Length != 2)
                return null;

            char file = algebraic[0];
            char rank = algebraic[1];

            if (file < 'a' || file > 'h' || rank < '1' || rank > '8')
                return null;

            int col = file - 'a';
            int row = 8 - (rank - '0');

            return (row, col);
        }

        public static void SimulateMove(ref string[][] board, string move)
        {
            var from = move.Substring(0, 2);
            var to = move.Substring(2, 2);

            var fromIndex = AlgebraicToIndex(from);
            var toIndex = AlgebraicToIndex(to);

            int fromRow = fromIndex.Value.Row;
            int fromCol = fromIndex.Value.Col;

            int toRow = toIndex.Value.Row;
            int toCol = toIndex.Value.Col;

            board[toRow][toCol] = board[fromRow][fromCol];
            board[fromRow][fromCol] = "-";
        }

        public static bool IsKingInCheck(string[][] board, bool isWhite, string lastMove)
        {
            string kingCode = isWhite ? "wK" : "bK";
            int kingRow = -1, kingCol = -1;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (board[row][col] == kingCode)
                    {
                        kingRow = row;
                        kingCol = col;
                        break;
                    }
                }
            }

            if (kingRow == -1) return true;

            string enemyColor = isWhite ? "b" : "w";

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    string pieceCode = board[row][col];
                    if (pieceCode != "-" && pieceCode.StartsWith(enemyColor))
                    {
                        var enemyPiece = PieceHelper.GetPieceFromCode(pieceCode, col, row);
                        var enemyMoves = enemyPiece.GetLegalMoves(board, lastMove);

                        foreach (var move in enemyMoves)
                        {
                            var to = move.Move.Substring(2, 2);
                            var toIndex = AlgebraicToIndex(to);
                            if (toIndex != null && toIndex.Value.Row == kingRow && toIndex.Value.Col == kingCol)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }


    }
}
