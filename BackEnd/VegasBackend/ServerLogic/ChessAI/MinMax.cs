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

            if (maximizing)
            {
                int maxEval = int.MinValue;

                foreach (var move in moves)
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

                foreach (var move in moves)
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
    }

}
