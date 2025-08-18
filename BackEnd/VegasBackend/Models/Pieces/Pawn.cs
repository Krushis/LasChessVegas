using VegasBackend.DTO;

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

        public override List<DTOLegalMove> GetLegalMoves(string[][] board, List<string> MadeMoves, bool skipCastle)
        {
            string lastMove = null;
            if (MadeMoves.Count == 0)
            {
                
            }
            else
            {
                lastMove = MadeMoves.LastOrDefault();
            }
                
            List<DTOLegalMove> moves = new();

            int direction = IsWhite ? -1 : 1;
            int startRow = IsWhite ? 6 : 1; // white is on row 6, black is on row 1
            string colorLetter = Notation.Substring(0, 1);

            int row = PositionRow;
            int col = PositionCol;

            bool IsPromotionSquare = (IsWhite && row == 1) || (!IsWhite && row == 6);


            // en pessante
            if (!string.IsNullOrEmpty(lastMove)) // d7d5
            {
                var from = AnnotationHelper.AlgebraicToIndex(lastMove.Substring(0, 2)); // d7
                var to = AnnotationHelper.AlgebraicToIndex(lastMove.Substring(2, 2)); // d5

                if (from != null && to != null) // c5 - 2 ir 3
                {
                    int movedFromRow = from.Value.Row; // 7
                    int movedFromCol = from.Value.Col; // 3
                    int movedToRow = to.Value.Row; // 5
                    int movedToCol = to.Value.Col; // 3

                    string movedPiece = board[movedToRow][movedToCol]; // 
                    bool isEnemyPawn = movedPiece == (IsWhite ? "bp" : "wp");
                    bool movedTwoForward = Math.Abs(movedFromRow - movedToRow) == 2;

                    if (isEnemyPawn && movedTwoForward && Math.Abs(movedToCol - col) == 1 && movedToRow == row)
                    {
                        int moveRow = row + direction;
                        
                        moves.Add(new DTOLegalMove { Move = AnnotationHelper.MakeMove(col, row, movedToCol, moveRow), Piece = Notation, IsEnPassant = true, IsPawnPromotion = IsPromotionSquare });
                          
                    }
                }
            }

            // Move forward 1
            int nextRow = row + direction;
            if (IsWithinBounds(nextRow, col) && board[nextRow][col] == "-")
            {
                moves.Add(new DTOLegalMove { Move = AnnotationHelper.MakeMove(col, row, col, nextRow), IsEnPassant = false, Piece = Notation, IsPawnPromotion = IsPromotionSquare });
                //moves.Add(new LegalMoveDTO { Move = AnnotationHelper.MakeMove(col, row, col, nextRow), IsEnPassant = false });

                // Move forward 2
                int twoForward = row + 2 * direction;
                if (row == startRow && board[twoForward][col] == "-")
                {
                    moves.Add(new DTOLegalMove { Move = AnnotationHelper.MakeMove(col, row, col, twoForward), Piece = Notation, IsEnPassant = false, IsPawnPromotion = false });
                }
            }

            // Capture right
            int rightCol = col + 1;
            if (IsWithinBounds(nextRow, rightCol) && board[nextRow][rightCol] != "-" && board[nextRow][rightCol].Substring(0, 1) != colorLetter)
            {

                moves.Add(new DTOLegalMove { Move = AnnotationHelper.MakeMove(col, row, rightCol, nextRow), Piece = Notation, IsEnPassant = false, IsPawnPromotion = IsPromotionSquare });


            }

            // Capture left
            int leftCol = col - 1;
            if (IsWithinBounds(nextRow, leftCol) && board[nextRow][leftCol] != "-" && board[nextRow][leftCol].Substring(0, 1) != colorLetter)
            {
                moves.Add(new DTOLegalMove { Move = AnnotationHelper.MakeMove(col, row, leftCol, nextRow), Piece = Notation, IsEnPassant = false, IsPawnPromotion = IsPromotionSquare });
            }

            return moves;
        }
    }
}
