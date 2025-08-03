using System.ComponentModel;
using VegasBackend.DTO;

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

        public override List<LegalMoveDTO> GetLegalMoves(string[][] board, List<string> MadeMoves, bool skipCastle)
        {

            List<LegalMoveDTO> moves = new();

            int direction = IsWhite ? -1 : 1;
            string colorLetter = Notation.Substring(0, 1);

            int row = PositionRow;
            int col = PositionCol;

            // left([0]) is row, right([1]) is col, 
            int[][] moveDirections = [ [1, 0],
            [-1, 0], [0, 1], [0, -1], [1, 1], [1, -1], [-1, 1], [-1, -1] ];

            foreach (var moveDir in moveDirections)
            {
                int directionRow = moveDir[0];
                int directionCol = moveDir[1];

                int tempRow = row + directionRow;
                int tempCol = col + directionCol;

                if (IsWithinBounds(tempRow, tempCol))
                {
                    if (board[tempRow][tempCol] == "-")
                    {
                        moves.Add(new LegalMoveDTO { Move = AnnotationHelper.MakeMove(col, row, tempCol, tempRow), IsEnPassant = false, IsPawnPromotion = false });

                    }
                    else if (board[tempRow][tempCol].Substring(0, 1) != colorLetter)
                    {
                        moves.Add(new LegalMoveDTO { Move = AnnotationHelper.MakeMove(col, row, tempCol, tempRow), IsEnPassant = false, IsPawnPromotion = false });
                    }
                }
            }

            // castling

            bool kingHasMoved = HasKingMoved(MadeMoves, IsWhite);
            if (!skipCastle && !kingHasMoved && !AnnotationHelper.IsKingInCheck(board, IsWhite, MadeMoves))
            {
                if (CanCastle(board, MadeMoves, IsWhite, true))
                {
                    moves.Add(new LegalMoveDTO
                    {
                        Move = AnnotationHelper.MakeMove(col, row, col + 2, row),
                        IsCastle = true
                    });
                }

                if (CanCastle(board, MadeMoves, IsWhite, false))
                {
                    moves.Add(new LegalMoveDTO
                    {
                        Move = AnnotationHelper.MakeMove(col, row, col - 2, row),
                        IsCastle = true
                    });
                }
            }
            // CODE BROKEN REVISIT TOMORROW
            return moves;
        }

        private bool HasKingMoved(List<string> madeMoves, bool isWhite)
        {
            if (madeMoves.Count == 0) return false;
            string kingStartPos = isWhite ? "wK" : "bK";
            return madeMoves.Any(move => move.StartsWith(kingStartPos));
        }

        private bool CanCastle(string[][] board, List<string> madeMoves, bool isWhite, bool kingside)
        {
            if (madeMoves == null || madeMoves.Count == 0) return false;
            int row = isWhite ? 7 : 0;
            int rookCol = kingside ? 7 : 0;
            int kingCol = 4;

            int[] betweenSquaresCols = kingside ? new int[] { 5, 6 } : new int[] { 1, 2, 3 };

            if (betweenSquaresCols.Any(square => board[row][square] != "-") )
            {
                return false;
            }

            string rookCode = (isWhite ? "w" : "b") + "R";
            if (board[row][rookCol] != rookCode)
                return false;

            if (HasRookMoved(madeMoves, isWhite, kingside))
                return false;

            int step = kingside ? 1 : -1;
            for (int i = 1; i <= 2; i++)
            {
                var simulatedBoard = ChessBoard.CloneBoard(board);
                AnnotationHelper.SimulateMove(ref simulatedBoard,
                    AnnotationHelper.MakeMove(kingCol, row, kingCol + step * i, row));
                if (AnnotationHelper.IsKingInCheck(simulatedBoard, isWhite, madeMoves))
                    return false;
            }

            return true;
        }
        private bool HasRookMoved(List<string> madeMoves, bool isWhite, bool kingside)
        {
            if (madeMoves == null || madeMoves.Count == 0) return false;
            string rookCode = isWhite ? "wR" : "bR";

            string rookStartPos = isWhite ? (kingside ? "h1" : "a1") : (kingside ? "h8" : "a8");

            return madeMoves.Any(move =>
                move.StartsWith(rookCode) &&
                move.Length >= 6 &&
                move.Substring(2, 2) == rookStartPos
            );
        }
    }
}
