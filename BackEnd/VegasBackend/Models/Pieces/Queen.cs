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

        public override List<string> GetLegalMoves(string[][] board, string lastmove)
        {

            List<string> moves = new();

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

                while (IsWithinBounds(tempRow, tempCol))
                {
                    if (board[tempRow][tempCol] == "-")
                    {
                        moves.Add(AnnotationHelper.MakeMove(col, row, tempCol, tempRow));

                    }
                    else if (board[tempRow][tempCol].Substring(0, 1) != colorLetter)
                    {
                        moves.Add(AnnotationHelper.MakeMove(col, row, tempCol, tempRow));
                        break;
                    }
                    else
                    {
                        break; // own piece is blocking the rook
                    }

                    tempRow += directionRow;
                    tempCol += directionCol;

                }
            }

            return moves;
        }
    }
}
