using VegasBackend.ServerLogic.ChessAI.Model;

namespace VegasBackend.ServerLogic.ChessAI
{
    public class AIService
    {
        public AIMove GetBestMove(GameState state, int maxDepth)
        {
            AIMove bestMove = null;
            bool isWhiteTurn = state.MoveCount % 2 == 0;

            for (int depth = 1; depth <= maxDepth; depth++)
            {
                var minimax = new Minimax();
                int bestScore = isWhiteTurn ? int.MinValue : int.MaxValue;

                var moves = LegalMoveGenerator.GetAllLegalMoves(
                    state.Board, state.MadeMoves, isWhiteTurn
                );

                foreach (var move in moves)
                {
                    var next = GameStateUtils.ApplyMove(state, move);
                    int score = minimax.Search(
                        next, depth - 1,
                        int.MinValue, int.MaxValue,
                        !isWhiteTurn
                    );

                    bool isBetter = isWhiteTurn ? (score > bestScore) : (score < bestScore);
                    if (isBetter)
                    {
                        bestScore = score;
                        bestMove = new AIMove(
                            $"{move.Move[0]}{move.Move[1]}",
                            $"{move.Move[2]}{move.Move[3]}"
                        );
                    }
                }
                Console.WriteLine($"Depth {depth}: nodes={minimax.NodesSearched}");
            }
            return bestMove;
        }
    }

}
