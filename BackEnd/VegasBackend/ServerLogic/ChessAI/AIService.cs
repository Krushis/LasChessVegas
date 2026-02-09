using VegasBackend.ServerLogic.ChessAI.Model;
using System.Diagnostics;

namespace VegasBackend.ServerLogic.ChessAI
{
    public class AIService
    {
        private int _lastNodesSearched = 0;

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
                _lastNodesSearched = minimax.NodesSearched;
                Console.WriteLine($"Depth {depth}: nodes={minimax.NodesSearched}");
            }
            return bestMove;
        }

        public static void Benchmark()
        {
            var board = new string[][]
            {
                new[] { "bR", "bN", "bB", "bQ", "bK", "bB", "bN", "bR" },
                new[] { "bp", "bp", "bp", "bp", "bp", "bp", "bp", "bp" },
                new[] { "-", "-", "-", "-", "-", "-", "-", "-" },
                new[] { "-", "-", "-", "-", "-", "-", "-", "-" },
                new[] { "-", "-", "-", "-", "-", "-", "-", "-" },
                new[] { "-", "-", "-", "-", "-", "-", "-", "-" },
                new[] { "wp", "wp", "wp", "wp", "wp", "wp", "wp", "wp" },
                new[] { "wR", "wN", "wB", "wQ", "wK", "wB", "wN", "wR" }
            };

            var state = new GameState { Board = board, MadeMoves = new List<string>(), MoveCount = 0 };

            Console.WriteLine("\n=== BENCHMARK ===");
            Console.WriteLine("Depth | Nodes      | Time    | Nodes/sec");
            Console.WriteLine("------|------------|---------|----------");

            for (int d = 1; d <= 3; d++)
            {
                var ai = new AIService();
                var sw = Stopwatch.StartNew();
                ai.GetBestMove(state, d);
                sw.Stop();

                int nodes = ai._lastNodesSearched;
                double nps = nodes / (sw.Elapsed.TotalSeconds + 0.001);

                Console.WriteLine($"{d,5} | {nodes,10:N0} | {sw.ElapsedMilliseconds,6}ms | {nps,10:N0}");
            }

            Console.WriteLine("=================\n");
        }
    }
}