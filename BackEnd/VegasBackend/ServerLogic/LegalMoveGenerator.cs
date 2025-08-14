using VegasBackend.DTO;
using VegasBackend.Models;

namespace VegasBackend.ServerLogic
{
    public class LegalMoveGenerator
    {
        public static List<DTOLegalMove> GetAllLegalMoves(string[][] board, List<string> madeMoves, bool isWhiteTurn)
        {
            var legalMoves = new List<DTOLegalMove>();
            string colorLetter = isWhiteTurn ? "w" : "b";

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var pieceCode = board[row][col];
                    if (pieceCode != "-" && pieceCode.StartsWith(colorLetter))
                    {
                        var piece = PieceHelper.GetPieceFromCode(pieceCode, col, row);
                        var pseudoMoves = piece.GetLegalMoves(board, madeMoves, false);

                        foreach (var moveDTO in pseudoMoves)
                        {
                            var clonedBoard = ChessBoard.CloneBoard(board);
                            AnnotationHelper.SimulateMove(ref clonedBoard, moveDTO.Move);

                            if (!AnnotationHelper.IsKingInCheck(clonedBoard, isWhiteTurn, madeMoves))
                            {
                                legalMoves.Add(moveDTO);
                            }
                        }
                    }
                }
            }

            return legalMoves;
        }
    }
}
