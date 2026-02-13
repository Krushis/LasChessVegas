using VegasBackend.DTO;
using VegasBackend.Models;
using VegasBackend.ServerLogic;
using VegasBackend.ServerLogic.ChessAI;

public class Minmax
{
    public int NodesSearched { get; private set; }
    private readonly TranspositionTable _tt;
    private Func<string[][], List<string>, bool, List<DTOLegalMove>> _moveGenerator;
    private Func<string[][], List<string>, bool, List<DTOLegalMove>> _rootMoveGenerator; // ADD THIS

    public Minmax(
        TranspositionTable tt = null,
        Func<string[][], List<string>, bool, List<DTOLegalMove>> moveGenerator = null,
        Func<string[][], List<string>, bool, List<DTOLegalMove>> rootMoveGenerator = null) // ADD THIS
    {
        _tt = tt ?? new TranspositionTable(64);
        _moveGenerator = moveGenerator ?? LegalMoveGenerator.GetAllLegalMoves;
        _rootMoveGenerator = rootMoveGenerator; // ADD THIS
    }

    public int Search(GameState state, int depth, int alpha, int beta, bool maximizing, bool isRoot = false) // ADD isRoot
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

        // USE RESTRICTED GENERATOR ONLY AT ROOT, OTHERWISE USE NORMAL
        var generator = (isRoot && _rootMoveGenerator != null) ? _rootMoveGenerator : _moveGenerator;
        var moves = generator(state.Board, state.MadeMoves, isWhiteTurn);

        if (!moves.Any())
            return Evaluation.EvaluateBoard(state.Board);

        var orderedMoves = moves.OrderByDescending(m =>
        {
            if (ttBestMove != null && m.Move == ttBestMove)
                return 100000;
            return ScoreMove(state.Board, m);
        });

        string bestMove = null;

        if (maximizing)
        {
            int maxEval = int.MinValue;

            foreach (var move in orderedMoves)
            {
                var next = GameStateUtils.ApplyMove(state, move);
                int eval = Search(next, depth - 1, alpha, beta, false, false); // isRoot = false

                if (eval > maxEval)
                {
                    maxEval = eval;
                    bestMove = move.Move;
                }

                alpha = Math.Max(alpha, eval);

                if (beta <= alpha)
                    break;
            }

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
                int eval = Search(next, depth - 1, alpha, beta, true, false); // isRoot = false

                if (eval < minEval)
                {
                    minEval = eval;
                    bestMove = move.Move;
                }

                beta = Math.Min(beta, eval);

                if (beta <= alpha)
                    break;
            }

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