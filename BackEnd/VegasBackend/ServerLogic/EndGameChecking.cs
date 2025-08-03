using VegasBackend.Models;

namespace VegasBackend.ServerLogic
{
    public class EndGameChecking
    {
        public static EndGameResult CheckEndGame(GameState gameState)
        {
            var board = gameState.Board;
            var moveHistory = gameState.MadeMoves;
            bool isWhiteTurn = gameState.MoveCount % 2 == 0;
            string playerColor = isWhiteTurn ? "w" : "b";

            var legalMoves = LegalMoveGenerator.GetAllLegalMoves(board, moveHistory, isWhiteTurn);

            bool inCheck = AnnotationHelper.IsKingInCheck(board, isWhiteTurn, moveHistory);

            if (legalMoves.Count == 0)
            {
                if (inCheck)
                {
                    return new EndGameResult
                    {
                        Type = EndGameType.Checkmate,
                        Winner = isWhiteTurn ? "b" : "w"
                    };
                }
                else
                {
                    return new EndGameResult
                    {
                        Type = EndGameType.Stalemate,
                        Winner = null
                    };
                }
            }

            if (IsInsufficientMaterial(board))
            {
                return new EndGameResult
                {
                    Type = EndGameType.InsufficientMaterial,
                    Winner = null
                };
            }

            return new EndGameResult
            {
                Type = EndGameType.None,
                Winner = null
            };
        }

        private static bool IsInsufficientMaterial(string[][] board)
        {
            var pieces = new List<string>();

            foreach (var row in board)
            {
                foreach (var cell in row)
                {
                    if (cell != "-")
                        pieces.Add(cell.Substring(1)); // just the piece type
                }
            }

            if (pieces.All(p => p == "K")) return true;

            if (pieces.Count == 2 && pieces.Contains("K") && pieces.Contains("B")) return true;
            if (pieces.Count == 2 && pieces.Contains("K") && pieces.Contains("N")) return true;

            return false;


        }
    }
}
