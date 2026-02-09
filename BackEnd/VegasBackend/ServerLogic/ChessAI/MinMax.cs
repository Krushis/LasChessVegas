using VegasBackend.DTO;

namespace VegasBackend.ServerLogic.ChessAI
{
    public class Minimax
    {
        public int NodesSearched { get; private set; }

        public int Search(GameState state, int depth, int alpha, int beta, bool maximizing)
        {
            NodesSearched++;

            if (depth == 0)
                return Evaluation.EvaluateBoard(state.Board);

            bool isWhiteTurn = state.MoveCount % 2 == 0;
            var moves = LegalMoveGenerator.GetAllLegalMoves(
                state.Board, state.MadeMoves, isWhiteTurn
            );

            if (!moves.Any())
                return Evaluation.EvaluateBoard(state.Board);

            // ORDER MOVES - Check captures first
            var orderedMoves = moves.OrderByDescending(m => ScoreMove(state.Board, m));

            if (maximizing)
            {
                int maxEval = int.MinValue;

                foreach (var move in orderedMoves)
                {
                    var next = GameStateUtils.ApplyMove(state, move);
                    int eval = Search(next, depth - 1, alpha, beta, false);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);

                    if (beta <= alpha)
                        break;
                }

                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;

                foreach (var move in orderedMoves)
                {
                    var next = GameStateUtils.ApplyMove(state, move);
                    int eval = Search(next, depth - 1, alpha, beta, true);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);

                    if (beta <= alpha)
                        break;
                }

                return minEval;
            }
        }
        private int ScoreMove(string[][] board, DTOLegalMove move)
        {
            int score = 0;
            int fromRow = 8 - (move.Move[1] - '0');
            int fromCol = move.Move[0] - 'a';
            int toRow = 8 - (move.Move[3] - '0');
            int toCol = move.Move[2] - 'a';

            string capturedPiece = board[toRow][toCol];

            if (capturedPiece != "-")
            {
                score += 10;

                char victim = char.ToLower(capturedPiece[1]);
                char attacker = char.ToLower(board[fromRow][fromCol][1]);
                score += (Evaluation.GetPieceValue(victim) - Evaluation.GetPieceValue(attacker)) / 10;
            }

            if (move.IsPawnPromotion)
                score += 8;

            if (move.IsCastle)
                score += 5;

            return score;
        }
    }

}