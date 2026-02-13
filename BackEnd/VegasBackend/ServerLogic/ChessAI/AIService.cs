using System.Diagnostics;
using System.Runtime.CompilerServices;
using VegasBackend.DTO;
using VegasBackend.ServerLogic.ChessAI.Model;

namespace VegasBackend.ServerLogic.ChessAI
{
    public class AIService
    {
        private int _lastNodesSearched = 0;
        private readonly Minmax _minimax = new Minmax();

        public string GetTTStats() => _minimax.GetTTStats();

        public AIMove GetBestMove(GameState state, int maxDepth, bool useIterativeDeepening = true)
        {
            AIMove bestMove = null;
            bool isWhiteTurn = state.MoveCount % 2 == 0;

            int startDepth = useIterativeDeepening ? 1 : maxDepth;

            for (int depth = startDepth; depth <= maxDepth; depth++)
            {
                int nodesBefore = _minimax.NodesSearched;

                int bestScore = isWhiteTurn ? int.MinValue : int.MaxValue;
                var moves = LegalMoveGenerator.GetAllLegalMoves(state.Board, state.MadeMoves, isWhiteTurn);

                foreach (var move in moves)
                {
                    var next = GameStateUtils.ApplyMove(state, move);
                    int score = _minimax.Search(next, depth - 1, int.MinValue, int.MaxValue, !isWhiteTurn);

                    if (isWhiteTurn ? (score > bestScore) : (score < bestScore))
                    {
                        bestScore = score;
                        bestMove = new AIMove(move.Move.Substring(0, 2), move.Move.Substring(2, 2));
                    }
                }

                _lastNodesSearched = _minimax.NodesSearched - nodesBefore;
            }
            return bestMove;
        }

        public AIMove GetBestMoveFromAllowedMoves(GameState state, List<DTOLegalMove> allowedMoves, int maxDepth)
        {
            if (allowedMoves == null || !allowedMoves.Any())
                return null;

            AIMove bestMove = null;
            bool isWhiteTurn = state.MoveCount % 2 == 0;

            // Create a function that returns only allowed moves (for root only)
            Func<string[][], List<string>, bool, List<DTOLegalMove>> rootMoveGen =
                (board, madeMoves, whiteTurn) => allowedMoves;

            for (int depth = 1; depth <= maxDepth; depth++)
            {
                // Pass unrestricted generator for lookahead, restricted for root
                var minimax = new Minmax(
                    tt: null,
                    moveGenerator: LegalMoveGenerator.GetAllLegalMoves,  // Unrestricted for lookahead
                    rootMoveGenerator: rootMoveGen                        // Restricted at root
                );

                int bestScore = isWhiteTurn ? int.MinValue : int.MaxValue;

                foreach (var move in allowedMoves)
                {
                    var next = GameStateUtils.ApplyMove(state, move);

                    // Call with isRoot = false because we're already past root
                    int score = minimax.Search(
                        next, depth - 1,
                        int.MinValue, int.MaxValue,
                        !isWhiteTurn,
                        isRoot: false  // We're evaluating from opponent's perspective
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
                Console.WriteLine($"AI Depth {depth}: nodes={minimax.NodesSearched}, score={bestScore}");
            }

            return bestMove;
        }

        public static void Benchmark()
        {
            var board = new string[][]
            {
                new[] { "bR", "-", "bB", "bQ", "bK", "-", "-", "bR" },
                new[] { "bp", "bp", "-", "-", "bp", "bp", "bp", "bp" },
                new[] { "-", "-", "bN", "-", "-", "bN", "-", "-" },
                new[] { "-", "-", "bp", "bp", "-", "-", "-", "-" },
                new[] { "-", "-", "wB", "wp", "wp", "-", "-", "-" },
                new[] { "-", "-", "wN", "-", "-", "wN", "-", "-" },
                new[] { "wp", "wp", "wp", "-", "-", "wp", "wp", "wp" },
                new[] { "wR", "-", "wB", "wQ", "wK", "-", "-", "wR" }
            };

            var state = new GameState { Board = board, MadeMoves = new List<string>(), MoveCount = 0 };

            // Create the AI service ONCE
            var ai = new AIService();

            Console.WriteLine("\n=== BENCHMARK ===");
            Console.WriteLine("Depth | Nodes (New) | Time    | Nodes/sec");
            Console.WriteLine("------|-------------|---------|----------");

            var sw = new Stopwatch();

            for (int d = 1; d <= 5; d++)
            {
                sw.Restart();
                ai.GetBestMove(state, d, useIterativeDeepening: false);
                sw.Stop();

                int nodes = ai._lastNodesSearched;
                double nps = nodes / (sw.Elapsed.TotalSeconds + 0.001);

                string ttStats = ai.GetTTStats();

                Console.WriteLine($"{d,5} | {nodes,10:N0} | {sw.ElapsedMilliseconds,6}ms | {nps,10:N0}");
                Console.WriteLine($"      ┗━ TT Stats: {ttStats}");
            }
            Console.WriteLine("=================\n");
        }
    }
}