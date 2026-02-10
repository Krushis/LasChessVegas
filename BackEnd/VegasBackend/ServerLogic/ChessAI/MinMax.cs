using VegasBackend.DTO;
using VegasBackend.Models;

namespace VegasBackend.ServerLogic.ChessAI
{
    public class Minmax
    {
        public int NodesSearched { get; private set; }
        private readonly TranspositionTable _tt;

        public Minmax(TranspositionTable tt = null)
        {
            _tt = tt ?? new TranspositionTable(64); // 64MB default
        }

        public int Search(GameState state, int depth, int alpha, int beta, bool maximizing)
        {
            NodesSearched++;
            int alphaOrig = alpha;
            bool isWhiteTurn = true;

            if (state.Hash == 0)
            {
                isWhiteTurn = state.MoveCount % 2 == 0;
                state.Hash = Hashing.ComputeHash(state.Board, isWhiteTurn);
            }

            if (_tt.TryGetValue(state.Hash, depth, alpha, beta, out int ttScore, out string ttBestMove))
            {
                return ttScore;
            }

            if (depth == 0)
                return Evaluation.EvaluateBoard(state.Board);

            isWhiteTurn = state.MoveCount % 2 == 0;
            var moves = LegalMoveGenerator.GetAllLegalMoves(
                state.Board, state.MadeMoves, isWhiteTurn
            );

            if (!moves.Any())
                return Evaluation.EvaluateBoard(state.Board);

            var orderedMoves = moves.OrderByDescending(m =>
            {
                if (ttBestMove != null && m.Move == ttBestMove)
                    return 100000; // TT move first
                return ScoreMove(state.Board, m);
            });

            string bestMove = null;

            if (maximizing)
            {
                int maxEval = int.MinValue;

                foreach (var move in orderedMoves)
                {
                    var next = GameStateUtils.ApplyMove(state, move);
                    int eval = Search(next, depth - 1, alpha, beta, false);

                    if (eval > maxEval)
                    {
                        maxEval = eval;
                        bestMove = move.Move;
                    }

                    alpha = Math.Max(alpha, eval);

                    if (beta <= alpha)
                        break; // Beta cutoff
                }

                // Store in transposition table
                TTEntryType entryType = maxEval <= alphaOrig ? TTEntryType.UpperBound :
                                        maxEval >= beta ? TTEntryType.LowerBound :
                                        TTEntryType.Exact;
                _tt.Store(state.Hash, depth, maxEval, entryType, bestMove);

                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;

                foreach (var move in orderedMoves)
                {
                    var next = GameStateUtils.ApplyMove(state, move);
                    int eval = Search(next, depth - 1, alpha, beta, true);

                    if (eval < minEval)
                    {
                        minEval = eval;
                        bestMove = move.Move;
                    }

                    beta = Math.Min(beta, eval);

                    if (beta <= alpha)
                        break; // Alpha cutoff
                }

                // Store in transposition table
                TTEntryType entryType = minEval <= alphaOrig ? TTEntryType.UpperBound :
                                        minEval >= beta ? TTEntryType.LowerBound :
                                        TTEntryType.Exact;
                _tt.Store(state.Hash, depth, minEval, entryType, bestMove);

                return minEval;
            }
        }

        private int ScoreMove(string[][] board, DTOLegalMove move)
        {
            int score = 0;

            var from = AnnotationHelper.AlgebraicToIndex(move.Move.Substring(0, 2));
            var to = AnnotationHelper.AlgebraicToIndex(move.Move.Substring(2, 2));

            if (from == null || to == null)
                return 0;

            int fromRow = from.Value.Row;
            int fromCol = from.Value.Col;
            int toRow = to.Value.Row;
            int toCol = to.Value.Col;

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

        public string GetTTStats()
        {
            return _tt.GetStats();
        }
    }

}