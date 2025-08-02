using System.ComponentModel;
using VegasBackend.DTO;

namespace VegasBackend.Models.Pieces
{
    public class Knight : Piece
    {
        public override string Notation => IsWhite ? "wP" : "bP";
        public int PositionRow;
        public int PositionCol;

        public Knight(int positionCol)
        {
            PositionCol = positionCol;
        }

        public Knight(int positionCol, int positionRow)
        {
            PositionCol = positionCol;
            PositionRow = positionRow;
        }

        public override List<LegalMoveDTO> GetLegalMoves(string[][] board, string lastmove)
        {
            List<LegalMoveDTO> moves = new();

            int direction = IsWhite ? -1 : 1;
            string colorLetter = Notation.Substring(0, 1);

            int row = PositionRow;
            int col = PositionCol;

            // left([0]) is row, right([1]) is col, 
            int[][] moveDirections = [ [2, 1],
            [2, -1], [1, 2], [-1, 2], [1, -2], [-1, -2], [-2, 1], [-2, -1] ];

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

            // TODO: dont allow to move onto field of view piece, castling, cant take defended piece

            return moves;
        }
    }
}
